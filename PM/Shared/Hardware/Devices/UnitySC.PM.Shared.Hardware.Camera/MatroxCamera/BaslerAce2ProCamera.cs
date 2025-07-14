using System;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class BaslerAce2ProCamera : MatroxCameraBase
    {
        public override string SerialNumber { get => MdigInquireStringFeature("DeviceSerialNumber"); }

        public BaslerAce2ProCamera(BaslerAce2ProCameraConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
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
                    //Version = MdigInquireStringFeature("DeviceVersion");

                    MaxExposureTimeMs = MdigInquireInt64Feature("ExposureTime", MIL.M_FEATURE_MAX) / 1000.0;
                    MinExposureTimeMs = MdigInquireInt64Feature("ExposureTime", MIL.M_FEATURE_MIN) / 1000.0;
                    Width = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_X);
                    Height = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_Y);
                }

                // Allocation des buffers
                //.......................
                AllocateGrabBuffers(20);

                State = new Service.Interface.DeviceState(Service.Interface.DeviceStatus.Ready);
            });
        }

        public override void SetTriggerMode(TriggerMode mode)
        {
            Invoke(() =>
            {
#if DEBUG
                //lock (DigitizerId_lock)
                //{
                //    MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_GRAB_TRIGGER_STATE, ref state);
                //}
                //if (state != MIL.M_DISABLE)
                //    throw new ApplicationException("Trigger already set");
#endif

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

                Logger.Debug("Trigger mode: " + (source ?? enable));
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "FrameStart");
                    if (source != null)
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, source);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                }
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
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "All");
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref gain);
                }
            });
        }

        public override double GetGain()
        {
            return Invoke(() =>
            {
                double gain = double.NaN;
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "All");
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref gain);
                }
                return gain;
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
    }
}
