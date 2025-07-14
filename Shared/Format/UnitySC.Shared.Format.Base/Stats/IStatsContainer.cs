namespace UnitySC.Shared.Format.Base.Stats
{
    public interface IStatsContainer
    {
        object Mean { get; }
        object Min { get; }
        object Max { get; }
        object StdDev { get; }
        object Median { get; }
        object Delta { get; }
        object Sigma3 { get; }
    }
}
