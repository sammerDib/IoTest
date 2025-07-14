using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.DMT.Service.Interface.Measure
{
    /// <summary>
    ///     Data for reflectivity measure
    /// </summary>
    [DataContract]
    public class BrightFieldMeasure : MeasureBase
    {
        public override string MeasureName => "Bright-field";

        public override MeasureType MeasureType => MeasureType.BrightFieldMeasure;

        [DataMember] public bool ApplyUniformityCorrection { get; set; } = false;

        [DataMember] public Color Color { get; set; } = Colors.White;

        public override List<ResultType> GetOutputTypes()
        {
            var outputResultTypes = new List<ResultType>();
            SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.BrightField_Front : DMTResult.BrightField_Back));
            return outputResultTypes;
        }
    }
}
