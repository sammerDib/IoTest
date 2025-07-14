using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [DataContract]
    [KnownType(typeof(TSVMeasureTools))]
    [KnownType(typeof(BowMeasureTools))]
    [KnownType(typeof(NanoTopoMeasureTools))]
    [KnownType(typeof(XYCalibrationMeasureTools))]
    [KnownType(typeof(ThicknessMeasureTools))]
    [KnownType(typeof(TopographyMeasureTools))]
    [KnownType(typeof(ThicknessMeasureToolsForLayer))]
    [KnownType(typeof(WarpMeasureTools))]
    [KnownType(typeof(EdgeTrimMeasureTools))]
    [KnownType(typeof(TrenchMeasureTools))]
    public abstract class MeasureToolsBase
    {
    }
}
