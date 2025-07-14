using System;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

using static UnitySC.PM.Shared.Hardware.Camera.MatroxCamera.MatroxCameraConfigBase;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class VC155MXCamera : MatroxCameraBase
    {
        public override string SerialNumber => MdigInquireStringFeature("DeviceSerialNumber");

        public VC155MXCamera(MatroxCameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        public override void Init(MilSystem milSystem, int devNumber)
        {
            Invoke(() =>
            {
                // Init de base
                //.............
                base.Init(milSystem, devNumber);

                // Init MIL
                //.........
                lock (DigitizerId_lock)
                {
                    MIL.MdigAlloc(milSystem, devNumber, "M_DEFAULT", MIL.M_DEFAULT, ref DigitizerId_lock.Value);

                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_GRAB_TIMEOUT, Config.GrabTimeout * 1000);
                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_CORRUPTED_FRAME_ERROR, MIL.M_ENABLE);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "Timed");

                    SetGain(Config.Gain);

                    // Lecture des infos de la caméra
                    //...............................
                    Model = MdigInquireStringFeature("DeviceModelName");
                    // SerialNumber = MdigInquireStringFeature("DeviceSerialNumber"); on ne le lit pas maintenant, pour pouvoir hériter de cette classe
                    Version = MdigInquireStringFeature("DeviceVersion");

                    MaxExposureTimeMs = MdigInquireInt64Feature("ExposureTime", MIL.M_FEATURE_MAX) / 1000.0;
                    MinExposureTimeMs = MdigInquireInt64Feature("ExposureTime", MIL.M_FEATURE_MIN) / 1000.0;
                    Width = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_X);
                    Height = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_Y);
                }

                // Allocation des buffers
                //.......................
                AllocateGrabBuffers(20);

                State = new DeviceState(DeviceStatus.Ready);
            });
        }

        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            Invoke(() =>
            {
                Logger.Information($"{Name} ExposureTime:{exposureTime_ms}");
                double exposureTimeUs = exposureTime_ms * 1000;
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureTime", MIL.M_TYPE_DOUBLE, ref exposureTimeUs);
                }
            });
        }

        public override double GetExposureTimeMs()
        {
            return Invoke(() =>
            {
                double exposureTimeUs = MdigInquireDoubleFeature("ExposureTime");
                return exposureTimeUs / 1000;
            });
        }

        public override void SetGain(double gain)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "DigitalGain", MIL.M_TYPE_DOUBLE, ref gain);
                }
            });
        }

        public override double GetGain()
        {
            return Invoke(() => MdigInquireDoubleFeature("DigitalGain"));
        }

        public void SetPixelDepth(PixelFormat pixelDepth)
        {
            Invoke(() =>
            {
                string value;
                switch (pixelDepth)
                {
                    case PixelFormat.Mono8:
                        value = "Mono8";
                        break;

                    case PixelFormat.Mono12:
                        value = "Mono12";
                        break;

                    default:
                        throw new ApplicationException("Unsupported bit per pixel: " + pixelDepth);
                }

                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "PixelFormat", MIL.M_TYPE_STRING, value);
                }
            });
        }

        public override void SetTriggerMode(TriggerMode mode)
        {
            Invoke(() =>
            {
                long state = 0xFDFD;
#if DEBUG
                lock (DigitizerId_lock)
                {
                    MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_GRAB_TRIGGER_STATE, ref state);
                }
                if (state != MIL.M_DISABLE)
                    throw new ApplicationException("Trigger already set");
#endif

                string enable;
                string source;

                switch (mode)
                {
                    case TriggerMode.Off:
                        enable = "Off";
                        source = null;
                        state = MIL.M_DISABLE;
                        break;

                    case TriggerMode.Software:
                        enable = "On";
                        source = "Software";
                        state = MIL.M_ENABLE;
                        break;

                    case TriggerMode.Hardware:
                        enable = "On";
                        source = "External";
                        state = MIL.M_ENABLE;
                        break;

                    default:
                        throw new ApplicationException("unknown trigger mode: " + mode);
                }

                Logger.Debug("Trigger mode: " + (source ?? enable));
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "ExposureStart");
                    if (source != null)
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, source);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                }
            });
        }

        public override void SoftwareTrigger()
        {
            Invoke(() =>
            {
                Logger.Debug("SoftwareTrigger Waiting Unlock");
                GrabEvent.Reset();

                // La version "normale" fonctionne avec la caméra 29MPix mais pas avec la 155MPix
                //MIL.MdigControl(DigitizerId, MIL.M_GRAB_TRIGGER_SOFTWARE, 1);
                // Équivalent avec les features:
                lock (DigitizerId_lock)
                {
                    Logger.Debug("SoftwareTrigger");
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_EXECUTE, "TriggerSoftware", MIL.M_TYPE_COMMAND, MIL.M_NULL);
                }
            });
        }

        public void SetFOV(FieldOfView fov)
        {
            Invoke(() =>
            {
                // On resette l'offset pour être sûr que le width/height sera valide
                long zero = 0;
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetX", MIL.M_TYPE_MIL_INT32, ref zero);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetY", MIL.M_TYPE_MIL_INT32, ref zero);

                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Width", MIL.M_TYPE_MIL_INT32, ref fov.Width);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Height", MIL.M_TYPE_MIL_INT32, ref fov.Height);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetX", MIL.M_TYPE_MIL_INT32, ref fov.OffsetX);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetY", MIL.M_TYPE_MIL_INT32, ref fov.OffsetY);
                }

                Width = (int)fov.Width;
                Height = (int)fov.Height;
            });
        }
    }
}
