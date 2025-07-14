using System.Runtime.Serialization;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.XYCalibration
{
    [DataContract]
    public class XYCalibrationPointData : MeasurePointDataResultBase
    {
        [DataMember]
        public Length ShiftX { get; set; }

        [DataMember]
        public Length ShiftY { get; set; }

        [DataMember]
        public string ResultImageFileName { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} XShift: {ShiftX} YShift:{ShiftY}";
        }

    }
}
