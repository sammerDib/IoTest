using System;
using System.Globalization;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status
{
    #region Enums

    /// <summary>Common RA420 GPIO input statuses.</summary>
    [Flags]
    public enum AlignerGeneralInputs
    {
        ExhaustFanRotating = 1 << 4,

        SubstrateDetectionSensor1 = 1 << 8,
        SubstrateDetectionSensor2 = 1 << 9
    }

    /// <summary>Common RA420 GPIO output statuses.</summary>
    [Flags]
    public enum AlignerGeneralOutputs
    {
        AlignerReadyToOperate = 1 << 0,
        TemporarilyStop = 1 << 1,
        SignificantError = 1 << 2,
        LightError = 1 << 3,

        SubstrateDetection =
            1
            << 6, // 6 in documentation, but 8 in real tool (checked with I/O STATE software) check why sometimes it's 8
        AlignmentComplete = 1 << 7,

        SpindleSolenoidValveChuckingOFF =
            1 << 16, // 8 in documentation, but 16 in real tool (checked with I/O STATE software)

        SpindleSolenoidValveChuckingON =
            1 << 17 // 9 in documentation, but 17 in real tool (checked with I/O STATE software)
    }

    #endregion Enums

    public class AlignerGpioStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignerGpioStatus" /> class.
        /// <param name="other">Create a deep copy of <see cref="AlignerGpioStatus" /> instance</param>
        /// </summary>
        public AlignerGpioStatus(AlignerGpioStatus other)
        {
            Set(other);
        }

        public AlignerGpioStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            var d1 = (AlignerGeneralInputs)int.Parse(statuses[0], NumberStyles.AllowHexSpecifier);

            I_ExhaustFanRotating = (d1 & AlignerGeneralInputs.ExhaustFanRotating) != 0;
            I_SubstrateDetectionSensor1 =
                (d1 & AlignerGeneralInputs.SubstrateDetectionSensor1) != 0;
            I_SubstrateDetectionSensor2 =
                (d1 & AlignerGeneralInputs.SubstrateDetectionSensor2) != 0;

            var d2 = (AlignerGeneralOutputs)int.Parse(statuses[1], NumberStyles.AllowHexSpecifier);

            O_AlignerReadyToOperate = (d2 & AlignerGeneralOutputs.AlignerReadyToOperate) != 0;
            O_TemporarilyStop = (d2 & AlignerGeneralOutputs.TemporarilyStop) != 0;
            O_SignificantError = (d2 & AlignerGeneralOutputs.SignificantError) != 0;
            O_LightError = (d2 & AlignerGeneralOutputs.LightError) != 0;
            O_SubstrateDetection = (d2 & AlignerGeneralOutputs.SubstrateDetection) != 0;
            O_AlignmentComplete = (d2 & AlignerGeneralOutputs.AlignmentComplete) != 0;
            O_SpindleSolenoidValveChuckingOFF =
                (d2 & AlignerGeneralOutputs.SpindleSolenoidValveChuckingOFF) != 0;
            O_SpindleSolenoidValveChuckingON =
                (d2 & AlignerGeneralOutputs.SpindleSolenoidValveChuckingON) != 0;
        }

        #endregion Constructors

        #region Properties

        public bool I_ExhaustFanRotating { get; internal set; }
        public bool I_SubstrateDetectionSensor1 { get; internal set; }
        public bool I_SubstrateDetectionSensor2 { get; internal set; }

        public bool O_AlignerReadyToOperate { get; internal set; }
        public bool O_TemporarilyStop { get; internal set; }
        public bool O_SignificantError { get; internal set; }
        public bool O_LightError { get; internal set; }
        public bool O_SubstrateDetection { get; internal set; }
        public bool O_AlignmentComplete { get; internal set; }
        public bool O_SpindleSolenoidValveChuckingOFF { get; internal set; }
        public bool O_SpindleSolenoidValveChuckingON { get; internal set; }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(AlignerGpioStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    I_ExhaustFanRotating = false;
                    I_SubstrateDetectionSensor1 = false;
                    I_SubstrateDetectionSensor2 = false;

                    O_AlignerReadyToOperate = false;
                    O_TemporarilyStop = false;
                    O_SignificantError = false;
                    O_LightError = false;
                    O_SubstrateDetection = false;
                    O_AlignmentComplete = false;
                    O_SpindleSolenoidValveChuckingOFF = false;
                    O_SpindleSolenoidValveChuckingON = false;
                }
                else
                {
                    I_ExhaustFanRotating = other.I_ExhaustFanRotating;
                    I_SubstrateDetectionSensor1 = other.I_SubstrateDetectionSensor1;
                    I_SubstrateDetectionSensor2 = other.I_SubstrateDetectionSensor2;

                    O_AlignerReadyToOperate = other.O_AlignerReadyToOperate;
                    O_TemporarilyStop = other.O_TemporarilyStop;
                    O_SignificantError = other.O_SignificantError;
                    O_LightError = other.O_LightError;
                    O_SubstrateDetection = other.O_SubstrateDetection;
                    O_AlignmentComplete = other.O_AlignmentComplete;
                    O_SpindleSolenoidValveChuckingOFF = other.O_SpindleSolenoidValveChuckingOFF;
                    O_SpindleSolenoidValveChuckingON = other.O_SpindleSolenoidValveChuckingON;
                }
            }
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new AlignerGpioStatus(this);
        }

        #endregion Status Override
    }
}
