using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.DMT.Service.Interface.Measure
{
    /// <summary>
    ///     Data for high angle darkfield measure
    /// </summary>
    [DataContract]
    public class HighAngleDarkFieldMeasure : MeasureBase
    {
        public override string MeasureName => "High angle dark-field";

        public override MeasureType MeasureType => MeasureType.HighAngleDarkFieldMeasure;

        public override List<ResultType> GetOutputTypes()
        {
            var outputResultTypes = new List<ResultType>();
            SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.HighAngleDarkField_Front : DMTResult.HighAngleDarkField_Back));
            return outputResultTypes;
        }
    }
}
