using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.TSV
{
    [DataContract]
    public class TSVPointData : MeasurePointDataResultBase
    {
        [DataMember]
        public Length Depth { get; set; }

        [XmlIgnore]
        [DataMember]
        public MeasureState DepthState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public Length Length { get; set; }

        [XmlIgnore]
        [DataMember]
        public MeasureState LengthState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public Length Width { get; set; }

        [XmlIgnore]
        [DataMember]
        public MeasureState WidthState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public string ResultImageFileName { get; set; }

        [DataMember]
        public byte[] DepthRawSignal { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} Depth: {Depth} Length:{Length} Width:{Width}";
        }

        public override void NewIterInPath(int newIter)
        {
            ResultImageFileName =  MeasurePointDataResultBaseHelper.FormatNewIterPathCopy(ResultImageFileName, newIter);
        }
    }
}
