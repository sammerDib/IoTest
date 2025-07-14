using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver
{
    /// <summary>
    /// Provide services to transform a cancel code into a string description for RE201.
    /// </summary>
    public static class RorzeLoadPortCancelCodeInterpreter
    {
        static RorzeLoadPortCancelCodeInterpreter()
        {
            var cancelCodeToString = new SortedDictionary<int, string>
            {
                { 0x0100, "Too few elements for constituting the command." },
                { 0x0110, "Too many elements for constituting the command." },
                { 0x0200, "Motion target is not corresponded." },
                { 0x0300, "Too few elements for constituting the command." },
                { 0x0310, "Too many elements for constituting the command." },
                { 0x0400, "Command is not corresponded." },
                { 0x0500, "Too few parameters." },
                { 0x0510, "Too many parameters." },
                { 0x0520, "Improper number of parameters." },
                { 0x0700, "Abnormal Mode: Preparation is not completed." },
                { 0x0702, "Abnormal Mode: The mode is not the maintenance mode." },
                { 0x0A00, "Origin search is not completed." },
                { 0x0A01, "Origin reset is not completed." },
                { 0x0B00, "Processing." },
                { 0x0B01, "Moving." },
                { 0x0D00, "Abnormal flash memory." },
                { 0x0F00, "Error-occurred state." },
                { 0x1001, "Carrier cannot be detected." },
                { 0x1004, "Too small designated pulses." },
                { 0x1005, "Too large designated pulses." },
                { 0x1010, "Not clamped." },
                { 0x1100, "Emergency stop signal is turned on." },
                { 0x1200, "Temporarily stop signal is turned on." },
                { 0x1300, "Interlock signal is turned on." },
                { 0x1400, "Th drive power is turned off." },
                { 0x2000, "No response from ID reader/writer." },
                { 0x2100, "Command for ID reader/writer cancelled." }
            };

            // Add all cancel codes combinations with 1 digit variable
            for (var i = 0; i < 0xF; ++i)
            {
                cancelCodeToString.Add(0x0600 + i, $"Too small value of the {i + 1}th parameter.");
                cancelCodeToString.Add(0x0610 + i, $"Too large value of the {i + 1}th parameter.");
                cancelCodeToString.Add(0x0620 + i, $"The {i + 1}th parameter is not a numeral.");
                cancelCodeToString.Add(0x0630 + i, $"The number of digits of the {i + 1}th parameter is not proper.");
                cancelCodeToString.Add(0x0640 + i, $"The {i + 1}th parameter is not hexadecimal numeral.");
                cancelCodeToString.Add(0x0650 + i, $"The {i + 1}th parameter is not proper.");
                cancelCodeToString.Add(0x0660 + i, $"The {i + 1} parameter is not pulse.");
            }

            // Add all cancel codes combinations with 2 digits variable
            for (var i = 0; i < 0xFF; ++i)
                cancelCodeToString.Add(0x0800 + i, $"The {i + 1}th set data is not proper.");

            CancelCodeToString = new ReadOnlyDictionary<int, string>(cancelCodeToString);
        }

        public static IReadOnlyDictionary<int, string> CancelCodeToString { get; }
    }
}
