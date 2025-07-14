namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IDualProbeConfig : IProbeConfig
    {
        ProbeSingleConfigBase ProbeUp { get; set; }

        ProbeSingleConfigBase ProbeDown { get; set; }
    }
}
