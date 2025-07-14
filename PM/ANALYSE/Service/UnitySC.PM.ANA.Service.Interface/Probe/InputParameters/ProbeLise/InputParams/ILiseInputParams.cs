namespace UnitySC.PM.ANA.Service.Interface
{
    public interface ILiseInputParams : IProbeInputParams
    {
        IProbeSample ProbeSample { get; set; }
    }
}
