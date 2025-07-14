using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver
{
    /// <summary>
    /// Provide services to transform a cancel code into a string description for RR757.
    /// </summary>
    public static class RorzeRobotCancelCodeInterpreter
    {
        static RorzeRobotCancelCodeInterpreter()
        {
            var cancelCodeToString = new SortedDictionary<int, string>
            {
                { 0x0200, "Motion target is not supported." },
                { 0x0300, "Too few command elements." },
                { 0x0310, "Too many command elements." },
                { 0x0400, "Command is not supported." },
                { 0x0500, "Too few parameters." },
                { 0x0510, "Too many parameters." },
                { 0x0520, "Improper parameter number." },
                { 0x0700, "Abnormal mode: Preparation not completed." },
                { 0x0702, "Abnormal mode: Not in maintenance mode." },
                { 0x0704, "Command, which cannot be executed by the teaching pendant, is received." },
                { 0x090F, "The setting for two - step lifting / lowering is less than 10. (Only in the case of wafer retaining function = 24H)." },
                { 0x091F, "The setting for two - step lifting / lowering exceeds 90. (Only in the case of wafer retaining function = 24H)." },
                { 0x0920, "Improper slot designation." },
                { 0x0921, "The number of slots not set." },
                { 0x0A00, "Origin search not completed." },
                { 0x0A01, "Origin reset not completed." },
                { 0x0B00, "Processing." },
                { 0x0B01, "Moving." },
                { 0x0D00, "Abnormal flash memory." },
                { 0x0F00, "Error - occurred state." },
                { 0x1002, "Improper setting." },
                { 0x1003, "Improper current position." },
                { 0x1004, "Motion cannot be performed due to too small designated position." },
                { 0x1005, "Motion cannot be performed due to too large designated position." },
                { 0x1010, "No wafer on the upper finger." },
                { 0x1011, "No wafer on the lower finger." },
                { 0x1020, "Wafer exists on the upper finger." },
                { 0x1021, "Wafer exists on the lower finger." },
                { 0x1100, "The emergency stop signal is turned on." },
                { 0x1200, "The temporary stop signal is turned on." },
                { 0x1300, "The interlock signal is turned on." },
                { 0x1400, "The drive power is turned off." }
            };

            // Add all cancel codes combinations with 1 digit variable
            for (var i = 0; i < 0xF; ++i)
            {
                cancelCodeToString.Add(0x0600 + i, $"The parameter No. {i + 1} is too small.");
                cancelCodeToString.Add(0x0610 + i, $"The parameter No. {i + 1} is too large.");
                cancelCodeToString.Add(0x0620 + i, $"The parameter No. {i + 1} is not numeral.");
                cancelCodeToString.Add(0x0630 + i, $"The number of digits of the parameter No. {i + 1} is not correct.");
                cancelCodeToString.Add(0x0640 + i, $"The parameter No. {i + 1} is not a hexadecimal numeral.");
                cancelCodeToString.Add(0x0650 + i, $"The parameter No. {i + 1} is not correct.");
                cancelCodeToString.Add(0x0660 + i, $"The parameter No. {i + 1} is not pulse.");
                cancelCodeToString.Add(0x0900 + i, $"The teaching position of the axis No. {i + 1} is too small.");
                cancelCodeToString.Add(0x0910 + i, $"The teaching position of the axis No. {i + 1} is too large.");
            }

            // Add all cancel codes combinations with 2 digits variable
            for (var i = 0; i < 0xFF; ++i)
                cancelCodeToString.Add(0x0800 + i, $"The setting data of the No. {i + 1} is not proper.");

            CancelCodeToString = new ReadOnlyDictionary<int, string>(cancelCodeToString);
        }

        public static IReadOnlyDictionary<int, string> CancelCodeToString { get; }
    }
}
