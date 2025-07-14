using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    public enum ResultCorrectionType
    {
        None,
        Offset,
        Linear
    }


    [DataContract]
    [KnownType(typeof(MeasureBowConfiguration))]
    [KnownType(typeof(MeasureNanoTopoConfiguration))]
    [KnownType(typeof(MeasureTopoConfiguration))]
    [KnownType(typeof(MeasureTSVConfiguration))]
    [KnownType(typeof(MeasureXYCalibrationConfiguration))]
    [KnownType(typeof(MeasureStepConfiguration))]
    [KnownType(typeof(MeasureEdgeTrimConfiguration))]
    [KnownType(typeof(MeasureTrenchConfiguration))]
    [KnownType(typeof(MeasureThicknessConfiguration))]
    public abstract class MeasureConfigurationBase
    {
    }
}
