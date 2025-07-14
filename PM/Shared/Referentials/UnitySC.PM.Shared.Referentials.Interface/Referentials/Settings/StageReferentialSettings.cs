using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class StageReferentialSettings : ReferentialSettingsBase
    {
        public StageReferentialSettings() : base(ReferentialTag.Stage)
        {
            EnableProbeSpotOffset = false;
            EnableDistanceSensorOffset = false;
        }

        /// <summary>
        /// Whether or not to enable the offset correction for the probe spot position
        /// </summary>
        [DataMember]
        public bool EnableProbeSpotOffset { get; set; }
        /// <summary>
        /// Whether or not to enable the offset correction for the distance sensor position
        /// </summary>
        [DataMember]
        public bool EnableDistanceSensorOffset { get; set; }

    }
}
