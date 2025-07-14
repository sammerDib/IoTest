using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    /// <summary>
    /// Sends CameraMessage (includes errors) messenger callbacks.
    /// </summary>
    public abstract class MatroxCameraBase : USPCameraMilBase//CameraBase, IDisposable
    {
        protected new MatroxCameraConfigBase Config => (MatroxCameraConfigBase)base.Config;

        /// <summary>
        /// Caution: lock this before using the digitizerId! (as it is not thread safe, and is being used for buffer overflow detection during acquisition).
        /// </summary>
        protected readonly Boxing<MIL_ID> DigitizerId_lock = new Boxing<MIL_ID>();// Mil doc: "Unless otherwise specified, all MIL functions are considered to be re-entrant." (and not thread safe).

        protected MilSystem ParentMilSystem;

        /// <summary> MIL Callback MIL for asynchronous grab, called each time a buffer is acquired </summary>
        private MIL_DIG_HOOK_FUNCTION_PTR _milAsynchronousGrabCallback = new MIL_DIG_HOOK_FUNCTION_PTR(MILAsynchronousGrabCallBack);

        protected GCHandle _handleToThis;

        // counter for debugging purposes
        public int ImageCount { get; protected set; }

        protected ManualResetEvent GrabEvent = new ManualResetEvent(false);

        /// <summary>
        /// Unpaged MIL Buffers for grab
        /// </summary>
        protected USPImageMil[] _grabImages;

        /// <summary>
        /// MIL Buffer IDs for milGrabImages
        /// </summary>
        protected MIL_ID[] _milGrabBufferIDs;

        /// <summary>
        /// Set when acquisition is requested to stop.
        /// </summary>
        protected volatile bool _stopRequested_ts;

        /// <summary>
        ///
        /// </summary>
        /// <param name="digitizerID"></param>
        /// <param name="config"></param>
        protected MatroxCameraBase(MatroxCameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        public int MaxWidth { get; protected set; }
        public int MaxHeight { get; protected set; }
        public int MinWidth { get; protected set; }
        public int MinHeight { get; protected set; }
        public int WidthIncrement { get; protected set; }
        public int HeightIncrement { get; protected set; }
        public Int32 PixelDepthBuffer { get; protected set; } = 1;

        /// <summary>
        /// Size in bytes of one buffer line. May be bigger than the actual line size in bytes (the right part of the buffer being unused).
        /// Can only be called after AllocateGrabBuffers(...).
        /// </summary>
        public Int32 Pitch_byte => _grabImages[0].GetMilImage().Pitch;

        /// <summary>
        /// Camera initialization
        /// </summary>
        public virtual void Init(MilSystem milSystem, int digNumber)
        {
            base.Init();
            ParentMilSystem = milSystem;
            _handleToThis = GCHandle.Alloc(this);
        }

        protected void InvokeWithStopGrab(Action action)
        {
            bool wasAcquiring = IsAcquiring;
            StopContinuousGrab();
            try
            {
                action.Invoke();
            }
            finally
            {
                if (wasAcquiring)
                {
                    StartContinuousGrab();
                }
            }
        }

        protected T InvokeWithStopGrab<T>(Func<T> func)
        {
            bool wasAcquiring = IsAcquiring;
            StopContinuousGrab();
            try
            {
                return func.Invoke();
            }
            finally
            {
                if (wasAcquiring)
                {
                    StartContinuousGrab();
                }
            }
        }

        public override void Shutdown()
        {
            Invoke(() =>
            {
                if (IsAcquiring)
                    throw new ApplicationException("Camera is still acquiring");

                if (_handleToThis.IsAllocated)
                    _handleToThis.Free();

                if (_grabImages != null)
                {
                    for (int i = 0; i < _grabImages.Count(); i++)
                        _grabImages[i].Dispose();
                    _grabImages = null;
                }

                lock (DigitizerId_lock)
                {
                    if (DigitizerId_lock.Value != 0)
                    {
                        MIL.MdigFree(DigitizerId_lock.Value);
                        DigitizerId_lock.Value = 0;
                    }
                }
                State = new DeviceState(DeviceStatus.Unknown, "Camera has been shut down");
                Logger.Information(State.StatusMessage);
            });
        }

        /// <summary>
        /// Allocates grab buffers
        /// </summary>
        protected void AllocateGrabBuffers(int nb)
        {
            InvokeWithStopGrab(() =>
            {
                _grabImages = new USPImageMil[nb];
                for (int i = 0; i < nb; i++)
                {
                    _grabImages[i] = new USPImageMil();
                    MilImage milImage = _grabImages[i].GetMilImage();
                    //milImage.Alloc2d(ParentMilSystem, Width, Height, (Config.PixelDepthBuffer_bytes << 3) + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_GRAB + MIL.M_NON_PAGED);
                    milImage.Alloc2d(ParentMilSystem, Width, Height, (PixelDepthBuffer << 3) + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_GRAB);

                    milImage.Clear(127);
                }
            });
        }

        /// <summary>
        /// Buffer disposal and disconnection.
        /// </summary>
        public override void Dispose()
        {
            Shutdown();
        }

        /// <summary>
        /// Singel image acquisition
        /// </summary>
        public override USPImageMil SingleGrab()
        {
            return Invoke(() =>
            {
                int wasAcquiring = Interlocked.Exchange(ref _isAcquiring, 1);
                if (wasAcquiring != 0)
                    throw new ApplicationException("Acquisition already started");

                try
                {
                    Logger.Information("Acquisition starting (SingleGrab)");

                    MilImage milGrabImage = _grabImages[0].GetMilImage();
                    lock (DigitizerId_lock)
                    {
                        MIL.MdigGrab(DigitizerId_lock.Value, milGrabImage.MilId);
                    }

                    Messenger.Send(new CameraMessage() { Camera = this, Image = _grabImages[0] });

                    USPImageMil procimg = CopyGrabbedImageToNewProcessingImage(_grabImages[0]);

                    return procimg;
                }
                finally
                {
                    Logger.Information("Acquisition stopped (SingleGrab)");
                    IsAcquiring = false;
                }
            });
        }

        /// <summary>
        /// Start continuous acquisition
        /// </summary>
        public override void StartContinuousGrab()
        {
            try
            {
                int wasAcquiring = Interlocked.Exchange(ref _isAcquiring, 1);
                if (wasAcquiring != 0)
                    throw new ApplicationException("Acquisition already started");

                Logger.Information("Acquisition starting (Continuous)");
                ImageCount = 0;
                _stopRequested_ts = false;

                // Init
                //.....
                Invoke(() =>
                {
                    _milGrabBufferIDs = _grabImages.Select(i => i.GetMilImage().MilId).ToArray();

                    lock (DigitizerId_lock)
                    {
                        State = new DeviceState(DeviceStatus.Ready);
                        MIL.MdigProcess(DigitizerId_lock.Value, _milGrabBufferIDs, _milGrabBufferIDs.Length, MIL.M_START, MIL.M_ASYNCHRONOUS, _milAsynchronousGrabCallback, (IntPtr)_handleToThis);
                    }

                    Logger.Debug("Acquisition started (Continuous)");
                });
            }
            catch
            {
                // Error handling
                //...............
                Logger.Information("Acquisition stops (Continuous)");
                IsAcquiring = false;
                throw;
            }
        }

        /// <summary>
        /// Create a MIL Sequence. It acquires the given count of images then stops.
        /// </summary>
        /// <param name="imageCount"> Number of Images to acquire </param>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="Exception"></exception>
        public override void StartSequentialGrab(uint imageCount)
        {
            try
            {
                int wasAcquiring = Interlocked.Exchange(ref _isAcquiring, 1);
                if (wasAcquiring != 0)
                    throw new ApplicationException("Acquisition already started");

                Logger.Information("Acquisition starting (Sequential)");
                ImageCount = 0;
                _stopRequested_ts = false;

                MIL.MdigProcess(DigitizerId_lock.Value, _milGrabBufferIDs, _milGrabBufferIDs.Length, MIL.M_SEQUENCE + MIL.M_COUNT(imageCount), MIL.M_ASYNCHRONOUS, _milAsynchronousGrabCallback, (IntPtr)_handleToThis);
                IsAcquiring = true;
            }
            catch (Exception e)
            {
                Logger.Information("Acquisition stops (Sequential)");
                IsAcquiring = false;
                throw new Exception("StartSequentialGrab failed with: ", e);
            }
        }

        /// <summary>
        /// Stop a continuous acquisition. Acquisition will be stopped after acquiring the current image
        /// </summary>
        public override void StopContinuousGrab()
        {
            Invoke(() =>
            {
                Logger.Information("Acquisition stopping (Continuous)");

                _stopRequested_ts = true;

                lock (DigitizerId_lock)
                {
                    if (!IsAcquiring)
                    {
                        return;
                    }

                    MIL.MdigProcess(DigitizerId_lock.Value, _milGrabBufferIDs, _milGrabBufferIDs.Length, MIL.M_STOP, MIL.M_DEFAULT, _milAsynchronousGrabCallback, (IntPtr)_handleToThis);
                }

                Logger.Information("Acquisition stopped (Continuous)");
                IsAcquiring = false;
            });
        }

        /// <summary>
        /// Wait for an acquisition following a software trigger
        /// </summary>
        public override void WaitForSoftwareTriggerGrabbed()
        {
            Invoke(() =>
            {
                int ms = (int)(Config.GrabTimeout * 1000);
                bool b = GrabEvent.WaitOne(ms);
                if (!b)
                {
                    string mode = MdigInquireStringFeature("TriggerMode");
                    string source = MdigInquireStringFeature("TriggerSource");
                    Logger.Error("Timeout during image acquisition, TriggerMode:" + mode + " " + source);

                    throw new ApplicationException(ExceptionTimeOut);
                }
            });
        }

        /// <summary>
        /// Callback MIL pour le grab Asynchrone, appelée à chaque buffer acquis
        /// </summary>
        protected static MIL_INT MILAsynchronousGrabCallBack(MIL_INT hookType, MIL_ID hookId, IntPtr hookDataPtr)
        {
            //-------------------------------------------------------------
            // Get UserData / this
            //-------------------------------------------------------------
            if (IntPtr.Zero.Equals(hookDataPtr))    // this is how to check if the user data is null
            {
                // Nothing we can really do
                return -1;
            }

            // Get the handle to the DigHookUserData object back from the IntPtr
            GCHandle hUserData = (GCHandle)hookDataPtr;

            // Get a reference to the DigHookUserData object
            MatroxCameraBase This = (MatroxCameraBase)hUserData.Target;

            This.Logger.Debug($"MILAsynchronousGrabCallBack Start count={This.ImageCount}");
            //-------------------------------------------------------------
            // Error handling
            //-------------------------------------------------------------
            if (This.State.Status == Service.Interface.DeviceStatus.Error)
            {
                This.Logger.Debug("Camera is already in error state");
                This.GrabEvent.Set();
                return -1;
            }

            if (hookType != MIL.M_MODIFIED_BUFFER)
            {
                This.Logger.Debug("Camera hookType!=MIL.M_MODIFIED_BUFFER");
                // This is not really an error, need to investigate further
                This.State = new Service.Interface.DeviceState(Service.Interface.DeviceStatus.Warning, "Unknown HookType");
                This.Logger.Warning(This.State.StatusMessage);
                This.GrabEvent.Set();
                return -1;
            }

            long isCorrupted = 1234567890;
            MIL.MdigGetHookInfo(hookId, MIL.M_CORRUPTED_FRAME, ref isCorrupted);
            if (isCorrupted != 0)
            {
                This.SetError("Corrupted frame");
                return -1;
            }

            //-------------------------------------------------------------
            // Camera buffer reading
            //-------------------------------------------------------------

            // Retrieve the MIL_ID of the grabbed buffer
            //..........................................
            This.ImageCount++;
            long idx = 0;
            MIL.MdigGetHookInfo(hookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_INDEX, ref idx);

            // Send the acquired image to interested parties
            //..............................................
            This.Logger.Debug($"MILAsynchronousGrabCallBack count={This.ImageCount} / buf={idx}");

            Messenger.Send<CameraMessage>(new CameraMessage() { Camera = This, Image = This._grabImages[idx] });

            while (!Monitor.TryEnter(This.DigitizerId_lock))
            {
                // If the owner of the object is locking the digitizerId, it may be when stopping acquisition, and in that case MIL waits for this function to exit.
                // Check if this is the case.
                if (This._stopRequested_ts)
                {
                    return 0;   // Discard the image.
                }
            }
            try
            {
                if (MIL.MdigInquire(This.DigitizerId_lock.Value, MIL.M_PROCESS_PENDING_GRAB_NUM) <= 0)
                {
                    This.SetError("Buffer underrun");
                    return -1;
                }
            }
            finally
            {
                Monitor.Exit(This.DigitizerId_lock);
            }

            This.GrabEvent.Set();   // Unlock all threads

            return 0;
        }

        protected string MdigInquireStringFeature(string featureName)
        {
            MIL_INT size = 0;
            lock (DigitizerId_lock)
            {
                MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, featureName, MIL.M_TYPE_MIL_INT, ref size);
                StringBuilder sb = new StringBuilder((int)size);
                MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, featureName, MIL.M_TYPE_STRING, sb);
                return sb.ToString();
            }
        }

        protected double MdigInquireDoubleFeature(string featureName, long inquireType = MIL.M_FEATURE_VALUE)
        {
            double val = double.NaN;
            lock (DigitizerId_lock)
            {
                MIL.MdigInquireFeature(DigitizerId_lock.Value, inquireType, featureName, MIL.M_TYPE_DOUBLE, ref val);
            }
            return val;
        }

        protected long MdigInquireInt64Feature(string featureName, long inquireType = MIL.M_FEATURE_VALUE)
        {
            long val = 0xFDFDFDFDFDFDFD;
            lock (DigitizerId_lock)
            {
                MIL.MdigInquireFeature(DigitizerId_lock.Value, inquireType, featureName, MIL.M_TYPE_INT64, ref val);
            }
            return val;
        }

        public override void SetFrameRate(double frameRate)
        {
            lock (DigitizerId_lock)
            {
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "AcquisitionFrameRate", MIL.M_TYPE_DOUBLE, ref frameRate);
            }
        }

        public override double GetFrameRate()
        {
            return MdigInquireDoubleFeature("AcquisitionFrameRate");
        }

        public override void SetColorMode(string colorMode)
        {
            if (!ColorModes.Contains(colorMode))
                return; //TODO : throw exception ?
            lock (DigitizerId_lock)
            {
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "PixelFormat", MIL.M_TYPE_STRING, colorMode);
            }
        }

        public override string GetColorMode()
        {
            return MdigInquireStringFeature("PixelFormat");
        }

        public override List<string> GetColorModes()
        {
            var colorModes = new List<string>();
            lock (DigitizerId_lock)
            {
                long pixelFormatStringSize = 0;
                MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_ENUM_ENTRY_COUNT, "PixelFormat", MIL.M_TYPE_MIL_INT, ref pixelFormatStringSize);

                for (int i = 0; i < pixelFormatStringSize; i++)
                {
                    var temp = new StringBuilder();
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_ENUM_ENTRY_NAME + i, "PixelFormat", MIL.M_TYPE_STRING, temp);
                    colorModes.Add(temp.ToString());
                }
            }
            return colorModes;
        }
    }
}
