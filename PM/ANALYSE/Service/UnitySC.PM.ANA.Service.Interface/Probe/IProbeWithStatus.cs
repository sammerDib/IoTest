namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbeWithStatus : IProbe
    {
        ProbeStatus Status { get; set; }
    }
}
