using System.Runtime.Serialization;

namespace UnitySC.PM.LIGHTSPEED.Data.Measure
{
    [DataContract(Namespace = "")]
    public abstract class MeasureBase
    {
        /// <summary>
        /// True if the execution of this measure is required
        /// </summary>
        [DataMember]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Exposure time during acquisition.
        /// This is an abstract value, that must will be corrected using the calibration to get the actual value in second.
        /// </summary>
        [DataMember]
        public double ExposureTime { get; set; }

        /// <summary>
        /// Output measure results
        /// </summary>
        [DataMember]
        public Enum.OutputResult OutputResults { get; set; }

        /// <summary>
        /// Define when the exposure time trigger is set for a lot
        /// </summary>
        [DataMember]
        public Enum.AutoExposureTimeTrigger AutoExposureTimeTrigger { get; set; }
    }
}
