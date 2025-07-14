using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitySC.EFEM.Rorze.Drivers.RC530
{
    /// <summary>
    /// Provide services to transform a cancel code into a string description for RC530.
    /// </summary>
    public static class RorzeRC530CancelCodeInterpreter
    {
        static RorzeRC530CancelCodeInterpreter()
        {
            var cancelCodeToString = new SortedDictionary<int, string>
            {
                { 0x0005, "Too many/too few parameters." },
                { 0x0006, "Abnormal range of the parameter." },
                { 0x0007, "Abnormal mode." },
                { 0x0009, "System in preparation." },
                { 0x000D, "Abnormal flash memory." },
                { 0x000E, "Insufficient memory." },
                { 0x000F, "Error-occurred state." }
            };

            CancelCodeToString = new ReadOnlyDictionary<int, string>(cancelCodeToString);
        }

        public static IReadOnlyDictionary<int, string> CancelCodeToString { get; }
    }
}
