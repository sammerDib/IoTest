using UnitySC.Shared.Logger;

namespace UnitySC.DataFlow.ProcessModules.Drivers.WCF
{
    /// <summary>
    /// Empty to have a default null logger which do nothing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class NullLogger<T> : NullLogger, ILogger<T>
    {
    }
}
