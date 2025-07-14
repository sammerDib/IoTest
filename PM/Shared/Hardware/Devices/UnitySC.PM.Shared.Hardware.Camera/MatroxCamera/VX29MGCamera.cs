using System;
using System.Threading;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    /// <summary>
    /// Sur le papier, la caméra devrait marcher comme la VC155. Dans la pratique, il faut ajouter plein de bidouilles
    /// </summary>
    public class VX29MGCamera : VC155MXCamera
    {
        public override string SerialNumber { get => MdigInquireStringFeature("DeviceID"); }

        public VX29MGCamera(MatroxCameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
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

        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            Invoke(() =>
            {
                base.SetExposureTimeMs(exposureTime_ms);

                // Parfois l'effet n'est pas immédiat, donc on capture une image bidon
                SoftwareTrigger();
                WaitForSoftwareTriggerGrabbed();
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
                    //MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "ExposureStart");
                    if (source != null)
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, source);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                }

                // Faut attendre un peu pour que la caméra réagisse bien.
                double exp = GetExposureTimeMs();
                System.Threading.Thread.Sleep((int)exp);
            });
        }

        //public override void SetTriggerMode(TriggerMode mode)
        //{
        //    Invoke(() =>
        //    {
        //        base.SetTriggerMode(mode);

        //        // Faut attendre un peu pour que la caméra réagisse bien.
        //        double exp = GetExposureTime();
        //        System.Threading.Thread.Sleep((int)(exp * 1000));
        //    });
        //}

        public override void SoftwareTrigger()
        {
            Invoke(() =>
            {
#if DEBUG
                string mode = MdigInquireStringFeature("TriggerMode");
                if (mode != "On")
                    throw new ApplicationException();
#endif

                base.SoftwareTrigger();
            });
        }

        public override void StartContinuousGrab()
        {
            Invoke(() =>
            {
                // Init de l'acquisition
                //......................
                string enable = "On";
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                    base.StartContinuousGrab();
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                }

                // Purge car on reçoit parfois des images non désirées
                //....................................................
                try
                {
                    bool empty = true;
                    int count = 0;
                    double exp = GetExposureTimeMs();
                    for (int i = 0; i < 5; i++)
                    {
                        string mode = MdigInquireStringFeature("TriggerMode");
                        if (mode != "On")
                            throw new ApplicationException();
                        System.Threading.Thread.Sleep((int)exp + 1000);
                        empty = (ImageCount == count);
                        if (empty)
                            break;
                        count = ImageCount;
                    }
                    if (!empty)
                        throw new ApplicationException("Too much unwanted images");
                    Logger.Debug("Acquisition ready (Continuous)");
                }
                catch
                {
                    // Gestion des erreurs
                    //....................
                    Logger.Information("Acquisition stops (Continuous)");
                    IsAcquiring = false;
                    throw;
                }
            });
        }
    }
}
