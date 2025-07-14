namespace UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise
{
    public interface IProbeDualLise : IProbeLise
    {
        IProbeLise ProbeLiseUp { get; }
        IProbeLise ProbeLiseDown { get; }
    }
}
