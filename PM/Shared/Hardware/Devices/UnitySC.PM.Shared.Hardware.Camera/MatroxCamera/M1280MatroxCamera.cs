using System;
using System.Diagnostics;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class M1280MatroxCamera : MatroxCameraBase
    {
        public override string SerialNumber { get => MdigInquireStringFeature("DeviceSerialNumber"); }

        private Stopwatch _stopwatch = new Stopwatch();

        private MIL_DIG_HOOK_FUNCTION_PTR _milAsynchronousGrabCallback = new MIL_DIG_HOOK_FUNCTION_PTR(MILAsynchronousGrabCallBack);

        public M1280MatroxCamera(MatroxCameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        public override void Init(MilSystem milSystem, int devNumber)
        {
            Invoke(() =>
            {
                base.Init(milSystem, devNumber);
                Logger?.Information("******************************** M1280 CAMERA STARTUP ********************************");
                lock (DigitizerId_lock)
                {
                    MIL.MdigAlloc(milSystem, devNumber, "M_DEFAULT", MIL.M_DEFAULT, ref DigitizerId_lock.Value);

                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_GRAB_TIMEOUT, Config.GrabTimeout * 1000);
                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_CORRUPTED_FRAME_ERROR, MIL.M_ENABLE);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "Timed");

                    // Lecture des infos de la caméra
                    //...............................
                    Model = MdigInquireStringFeature("DeviceModelName");
                    // SerialNumber = MdigInquireStringFeature("DeviceSerialNumber"); on le lit différement
                    Version = MdigInquireStringFeature("DeviceVersion");

                    MaxExposureTimeMs = MdigInquireInt64Feature("ExposureTime", MIL.M_FEATURE_MAX) / 1000;
                    MinExposureTimeMs = MdigInquireInt64Feature("ExposureTime", MIL.M_FEATURE_MIN) / 1000;

                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "DigitalAll");
                    MaxGain = MdigInquireDoubleFeature("Gain", MIL.M_FEATURE_MAX);
                    MinGain = MdigInquireDoubleFeature("Gain", MIL.M_FEATURE_MIN);

                    Width = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_X);
                    Height = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_Y);

                    ColorModes = GetColorModes();

                    MaxFrameRate = MdigInquireDoubleFeature("AcquisitionFrameRate", MIL.M_FEATURE_MAX);
                    MinFrameRate = MdigInquireDoubleFeature("AcquisitionFrameRate", MIL.M_FEATURE_MIN);
                }

                // Allocation des buffers
                //.......................
                AllocateGrabBuffers(20);

                State = new Service.Interface.DeviceState(Service.Interface.DeviceStatus.Ready);
            });
        }

        /// <param name="exposureTime_ms">In Millisecond</param>
        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            lock (DigitizerId_lock)
            {
                var exposureTimeUs = exposureTime_ms * 1000; // Convertion milliseconde en microseconde
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureTime", MIL.M_TYPE_DOUBLE, ref exposureTimeUs);
            }
        }

        public override double GetExposureTimeMs()
        {
            double exposureTimeUs = MdigInquireDoubleFeature("ExposureTime");
            return exposureTimeUs / 1000;
        }

        public override void SetGain(double gain)
        {
            lock (DigitizerId_lock)
            {
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "DigitalAll");
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref gain);
            }
        }

        public override double GetGain()
        {
            double gain = double.NaN;
            lock (DigitizerId_lock)
            {
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "DigitalAll");
                MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref gain);
            }
            return gain;
        }

        public override void SetTriggerMode(TriggerMode mode)
        {
            string enable;
            string source;

            switch (mode)
            {
                case TriggerMode.Off:
                    enable = "Off";
                    source = null;
                    break;

                case TriggerMode.Software:
                    enable = "On";
                    source = "Software";
                    break;

                case TriggerMode.Hardware:
                    enable = "On";
                    source = "External";
                    break;

                default:
                    throw new ApplicationException("unknown trigger mode: " + mode);
            }

            MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "FrameStart");
            lock (DigitizerId_lock)
            {
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                if (source != null)
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, source);
            }
        }

        public override void SoftwareTrigger()
        {
#if DEBUG
            string mode = MdigInquireStringFeature("TriggerMode");
            if (mode != "On")
                return;
            //throw new ApplicationException("Parameter TriggerMode is Off");
#endif
            Logger.Debug("SoftwareTrigger");
            GrabEvent.Reset();

            lock (DigitizerId_lock)
            {
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_EXECUTE, "TriggerSoftware", MIL.M_TYPE_COMMAND, MIL.M_NULL);
            }
        }

        public override void StartContinuousGrab()
        {
            try
            {
                if (IsAcquiring)
                    throw new ApplicationException("Acquisition already started");

                Logger.Information("Acquisition starting (Continuous)");
                ImageCount = 0;

                // Init de l'acquisition
                //......................
                Invoke(() =>
                {
                    _milGrabBufferIDs = _grabImages.Select(i => i.GetMilImage().MilId).ToArray();

                    lock (DigitizerId_lock)
                    {
                        State = new Service.Interface.DeviceState(Service.Interface.DeviceStatus.Ready);

                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "AcquisitionMode", MIL.M_TYPE_STRING, "Continuous");
                        MIL.MdigProcess(DigitizerId_lock.Value, _milGrabBufferIDs, _milGrabBufferIDs.Length, MIL.M_START, MIL.M_ASYNCHRONOUS, _milAsynchronousGrabCallback, (IntPtr)_handleToThis);
                    }

                    IsAcquiring = true;
                    _stopwatch.Start();
                    Logger.Debug("Acquisition started (Continuous)");
                });
            }
            catch
            {
                // Gestion des erreurs
                //....................
                Logger.Information("Acquisition stops (Continuous)");
                IsAcquiring = false;
                throw;
            }
        }

        public override void StopContinuousGrab()
        {
            if (!IsAcquiring)
                throw new ApplicationException("StopContinuousGrab without StartContinousGrab");

            Invoke(() =>
            {
                Logger.Information("Acquisition stopping (Continuous)");

                _stopRequested_ts = true;
                lock (DigitizerId_lock)
                {
                    MIL.MdigProcess(DigitizerId_lock.Value, _milGrabBufferIDs, _milGrabBufferIDs.Length, MIL.M_STOP, MIL.M_DEFAULT, _milAsynchronousGrabCallback, (IntPtr)_handleToThis);
                }

                Logger.Information("Acquisition stopped (Continuous)");
                IsAcquiring = false;
            });
        }

        public override USPImageMil SingleGrab()
        {
            return Invoke(() =>
            {
                bool wasAcquiring = false;
                if (IsAcquiring)
                {
                    wasAcquiring = true;
                    StopContinuousGrab();
                }

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
                    procimg.AddRef();

                    return procimg;
                }
                finally
                {
                    Logger.Information("Acquisition stopped (SingleGrab)");
                    IsAcquiring = false;
                    if (wasAcquiring)
                        StartContinuousGrab();
                }
            });
        }
    }
}
