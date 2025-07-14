using System;
using System.Threading;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class VieworksVC65MXM31Camera : VC155MXCamera
    {
        public override string SerialNumber { get => MdigInquireStringFeature("DeviceSerialNumber"); }

        public VieworksVC65MXM31Camera(VieworksVC65MXM31CameraConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        public override void Init(MilSystem milSystem, int devNumber)
        {
            Invoke(() =>
            {
                // Init de base
                //.............
                base.Init(milSystem, devNumber);

                // Lecture des infos de la caméra
                //...............................
                Model = MdigInquireStringFeature("DeviceModelName");
                // SerialNumber = MdigInquireStringFeature("DeviceID"); on le lit différement
                Version = MdigInquireStringFeature("DeviceVersion");

                // Petite bidouille
                base.SetExposureTimeMs(20);
                Thread.Sleep(1000);
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
                    //MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "ExposureStart");
                    if (source != null)
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, source);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                }
            });
        }

        public override void SetGain(double gain)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "DigitalAll");
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
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "DigitalAll");
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref gain);
                }
                return gain;
            });
        }
    }
}
