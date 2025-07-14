using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.DMT.Service.Interface.Measure
{
    /// <summary>
    ///     Data for back light  measure
    /// </summary>
    [DataContract]
    public class BackLightMeasure : MeasureBase
    {
        public override string MeasureName => "BackLight";

        public override MeasureType MeasureType => MeasureType.BacklightMeasure;

        public override List<ResultType> GetOutputTypes()
        {
            var outputResultTypes = new List<ResultType>();

            SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.TopoBackLight_Front : DMTResult.TopoBackLight_Back));

            return outputResultTypes;
        }
    }
}
