using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver
{
    /// <summary>
    /// Provide services to transform a cancel code into a string description for RV101.
    /// </summary>
    public static class RorzeLoadPortCancelCodeInterpreter
    {
        static RorzeLoadPortCancelCodeInterpreter()
        {
            var cancelCodeToString = new SortedDictionary<int, string>
            {
                { 0x0200, "The operating objective is not supported." },
                { 0x0300, "The composition elements of command are too few." },
                { 0x0310, "The composition elements of command are too many." },
                { 0x0400, "Command is not supported." },
                { 0x0500, "Too few parameters." },
                { 0x0510, "Too many parameters." },
                { 0x0520, "Improper number of parameters." },
                { 0x0700, "Abnormal Mode: Preparation not completed." },
                { 0x0702, "Abnormal Mode: Not in the maintenance mode." },
                { 0x0A00, "Origin search not completed." },
                { 0x0A01, "Origin reset not completed." },
                { 0x0B00, "Processing." },
                { 0x0B01, "Moving." },
                { 0x0D00, "Abnormal flash memory." },
                { 0x0F00, "Error-occurred state." },
                { 0x1000, "Carrier is Placed." },
                { 0x1001, "Carrier is not placed." },
                { 0x1002, "Improper setting." },
                { 0x1004, "Designated position is too small." },
                { 0x1005, "Designated position is too large." },
                { 0x1006, "Cover is opened." },
                { 0x1007, "Abnormal present state." },
                { 0x1008, "Work is protruded." },
                { 0x1100, "Emergency stop signal is turned ON." },
                { 0x1200, "Temporarily stop signal is turned ON." },
                { 0x1300, "Interlock signal is ON." },
                { 0x1400, "Drive power is OFF." }
            };

            // Add all cancel codes combinations with 1 digit variable
            for (var i = 0; i < 0xF; ++i)
            {
                cancelCodeToString.Add(0x0600 + i, $"The value of the No. {i + 1} parameter is too small.");
                cancelCodeToString.Add(0x0610 + i, $"The value of the No. {i + 1} parameter is too large.");
                cancelCodeToString.Add(0x0620 + i, $"The No. {i + 1} parameter is not numerical number.");
                cancelCodeToString.Add(0x0630 + i, $"The digit number of the No. {i + 1} parameter is not proper.");
                cancelCodeToString.Add(0x0640 + i, $"The No. {i + 1} parameter is not a hexadecimal numeral.");
                cancelCodeToString.Add(0x0650 + i, $"The No. {i + 1} parameter is not proper.");
                cancelCodeToString.Add(0x0660 + i, $"The No. {i + 1} parameter is not pulse.");
            }

            // Add all cancel codes combinations with 2 digits variable
            for (var i = 0; i < 0xFF; ++i)
                cancelCodeToString.Add(0x0800 + i, $"The setting data of the No. {i + 1} is not proper.");

            CancelCodeToString = new ReadOnlyDictionary<int, string>(cancelCodeToString);
        }

        public static IReadOnlyDictionary<int, string> CancelCodeToString { get; }
    }
}
