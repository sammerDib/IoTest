using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Step
{
    [DataContract]
    public class StepPointData :  MeasureScanLine
    {
        [DataMember]
        public Length StepHeight { get; set; }

        public override string ToString()
        {
             return $"{base.ToString()} StepHeight: {StepHeight}";
        }
    }
}
