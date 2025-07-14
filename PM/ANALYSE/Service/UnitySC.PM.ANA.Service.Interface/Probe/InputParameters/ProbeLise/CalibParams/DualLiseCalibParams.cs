using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class DualLiseCalibParams : IDualLiseCalibParams
    {
        public DualLiseCalibParams()
        {
        }

        #region Properties

        [DataMember]
        public OpticalReferenceDefinition ProbeCalibrationReference { get; set; }

        [DataMember]
        public int NbRepeatCalib { get; set; }

        [DataMember]
        public double TopLiseAirgapThreshold { get; set; }

        [DataMember]
        public double BottomLiseAirgapThreshold { get; set; }

        [DataMember]
        public double ZTopUsedForCalib { get; set; }

        [DataMember]
        public double ZBottomUsedForCalib { get; set; }

        [DataMember]
        public int CalibrationMode { get; set; }

        #endregion Properties
    }
}
