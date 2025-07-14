using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver
{
    /// <summary>
    /// Provide services to transform a cancel code into a string description for RA420.
    /// </summary>
    public static class RorzeAlignerCancelCodeInterpreter
    {
        static RorzeAlignerCancelCodeInterpreter()
        {
            var cancelCodeToString = new SortedDictionary<int, string>
            {
                { 0x0001, "Command not designated." },
                { 0x0002, "The designated target motion not equipped." },
                { 0x0003, "Too many/too few parameters (number of elements)." },
                { 0x0004, "Command not equipped." },
                { 0x0005, "Too many/too few parameters." },
                { 0x0006, "Abnormal range of the parameter." },
                { 0x0007, "Abnormal mode." },
                { 0x0008, "Abnormal data." },
                { 0x0009, "System in preparation." },
                { 0x000A, "Origin search not completed." },
                { 0x000B, "Moving/Processing." },
                { 0x000C, "No motion." },
                { 0x000D, "Abnormal flash memory." },
                { 0x000E, "Insufficient memory." },
                { 0x000F, "Error-occurred state." },
                { 0x0010, "Origin search is completed but the motion cannot be started due to interlock." },
                { 0x0011, "The emergency stop signal is turned on." },
                { 0x0012, "The temporarily stop signal is turned on." },
                { 0x0013, "Abnormal interlock signal." },
                { 0x0014, "Drive power is turned off." },
                { 0x0015, "Not excited." },
                { 0x0016, "Abnormal current position." },
                { 0x0017, "Abnormal target position." },
                { 0x0018, "Command processing." },
                { 0x0019, "Invalid substrate state." },
                { 0x0020, "Substrate yet to be aligned." }
            };

            CancelCodeToString = new ReadOnlyDictionary<int, string>(cancelCodeToString);
        }

        public static IReadOnlyDictionary<int, string> CancelCodeToString { get; }
    }
}
