using System;
using System.Collections.Generic;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera
{
    public abstract class CameraBase : DeviceBase
    {
        protected const string ExceptionTimeOut = "Timeout during image acquisition";

        public override DeviceFamily Family => DeviceFamily.Camera;

        public CameraConfigBase Config;

        /// <summary>
        /// Register here to get notified (background thread!) of a new image ready to be processed,
        /// or in case of an error.
        /// </summary>
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        /// <summary>
        /// Tells the camera to work in simulated mode, without any connection to the hardware.
        /// </summary>
        public bool IsSimulated;

        protected int _isAcquiring;

        public virtual bool IsAcquiring
        {
            get => _isAcquiring != 0;
            protected set => _isAcquiring = value ? 1 : 0;
        }

        public CameraBase(CameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
            Config = config;
            Name = config.Name;
            DeviceID = config.DeviceID;
            ImageResolution = Config.DefaultImageResolution;
            // FIXME: this property is specific to the VieworksVt camera, and thus should be moved there.
            ImageSize_bytes = CalculatePixelDepthBufferBytes(8) * Width * Height;
        }

        #region LIFECYCLE

        public virtual void Init()
        {
            Logger.Information($"Init {Family}-{Name}" + (IsSimulated ? "-simulated" : ""));
        }

        public abstract void Shutdown();

        public virtual void InitSettings()
        {
            throw new NotImplementedException("InitSettings is not implemented");
        }

        #endregion LIFECYCLE

        #region CAMERA PROPERTIES

        public virtual string Model { get; protected set; }
        public virtual string SerialNumber { get; protected set; }
        public virtual string Version { get; protected set; }

        #endregion CAMERA PROPERTIES

        #region AOI

        public virtual Rect GetAOI()
        {
            throw new NotImplementedException("AOI is not implemented");
        }

        public virtual void SetAOI(Rect aoi)
        {
            throw new NotImplementedException("AOI is not implemented");
        }

        #endregion AOI

        #region IMAGE RESOLUTION

        /// <summary>
        /// Image size, in bytes. Corresponds to the Width and Height parameters: the actual buffer
        /// size may be bigger, due to line stride / pitch.
        /// </summary>
        public int ImageSize_bytes { get; protected set; }

        public virtual int Width { get; protected set; }
        public virtual int Height { get; protected set; }

        public Size ImageResolution { get; protected set; }

        public virtual void SetImageResolution(Size imageResolution)
        {
            throw new NotImplementedException("SetImageResolution is not implemented");
        }

        #endregion IMAGE RESOLUTION

        #region COLOR MODE

        public virtual List<string> ColorModes { get; protected set; } // FIXME: duplicated definition here. Why is there a property and a dedicated method refering to ColorModes?

        public virtual List<string> GetColorModes()
        {
            return new List<string>();
        }

        public virtual string GetColorMode()
        {
            return string.Empty;
        }

        public virtual void SetColorMode(string colorMode)
        {
            return;
        }

        #endregion COLOR MODE

        #region EXPOSURE TIME

        public abstract double GetExposureTimeMs();

        public abstract void SetExposureTimeMs(double exposureTime_ms);

        public virtual double MinExposureTimeMs { get; protected set; } // FIXME: prefer dedicated methods over properties since calls are addressed to the hardware.

        public virtual double MaxExposureTimeMs { get; protected set; } // FIXME: prefer dedicated methods over properties since calls are addressed to the hardware.

        #endregion EXPOSURE TIME

        #region FRAMERATE

        public virtual double GetRealFramerate()
        {
            throw new NotImplementedException("GetRealFramerate is not implemented");
        }

        public virtual double GetFrameRate()
        {
            return double.NaN;
        }

        public virtual void SetFrameRate(double frameRate)
        {
            return;
        }

        public virtual double MinFrameRate { get; protected set; } // FIXME: prefer dedicated methods over properties since calls are addressed to the hardware.
        public virtual double MaxFrameRate { get; protected set; } // FIXME: prefer dedicated methods over properties since calls are addressed to the hardware.

        /// <summary>
        /// Ask the camera to change its internal configuration so that the framerate is maximum.
        /// </summary>
        /// <returns>The value of the currently set framerate.</returns>
        public virtual double ConfigureForOptimalFramerate()
        {
            throw new NotImplementedException();
        }

        #endregion FRAMERATE

        #region GAIN

        public abstract double GetGain();

        public abstract void SetGain(double gain);

        public virtual double MinGain { get; protected set; } // FIXME: prefer dedicated methods over properties since calls are addressed to the hardware.
        public virtual double MaxGain { get; protected set; } // FIXME: prefer dedicated methods over properties since calls are addressed to the hardware.

        #endregion GAIN

        #region TRIGGER

        public enum TriggerMode
        {
            Software,
            Hardware,
            Off,
        };

        public abstract void SetTriggerMode(TriggerMode mode);

        public abstract void SoftwareTrigger();

        /// <summary>
        /// Permet de synchroniser le software trigger. Attend que l'image triggée ait été acquise.
        /// In case of processing pipelining -ex: CyberOptics 3d-, the actual image may be sent to
        /// the messenger later.
        /// </summary>
        public abstract void WaitForSoftwareTriggerGrabbed();

        public virtual void TriggerOutEmitSignal(int pulseDuration_ms = 1, bool logTriggerStartStop = false)
        {
            throw new NotImplementedException();
        }

        #endregion TRIGGER

        #region IMAGE PROCESSING

        public abstract void StartContinuousGrab();

        /// <summary>
        /// Sequential grab will acquire the provided number images
        /// </summary>
        /// <param name="imageCount">Number of image to capture</param>
        /// <exception cref="NotSupportedException"></exception>
        public virtual void StartSequentialGrab(uint imageCount)
        {
            throw new NotSupportedException();
        }

        public abstract void StopContinuousGrab();

        #endregion IMAGE PROCESSING

        #region HELPERS

        /// <summary>
        /// Une fonction d'aide pour encapsuler la gestion des erreurs
        /// </summary>
        protected void Invoke(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                SetError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Une fonction d'aide pour encapsuler la gestion des erreurs
        /// </summary>
        protected T Invoke<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception e)
            {
                SetError(e.Message);
                throw;
            }
        }

        protected void SetError(string message)
        {
            if (message == ExceptionTimeOut)
                State = new DeviceState(DeviceStatus.Warning, message);
            else
                State = new DeviceState(DeviceStatus.Error, message);

            Logger.Error(message);

            Messenger.Send(new CameraMessage() { Camera = this, Error = true });
        }

        /// <summary>
        ///  Method for defining image size as a function of depth.
        /// </summary>
        public void SetImageSizeWithDepth(int depth)
        {
            ImageSize_bytes = CalculatePixelDepthBufferBytes(depth) * Width * Height;
        }

        /// <summary>
        /// Buffer size per pixel, including unused bits if dynamic is lower.
        /// </summary>
        public Int32 CalculatePixelDepthBufferBytes(int depth)
        {
            Int32 ret = depth >> 3;
            if ((ret << 3) != depth)
            {
                return ret + 1;
            }
            return ret;
        }

        #endregion HELPERS
    }
}
