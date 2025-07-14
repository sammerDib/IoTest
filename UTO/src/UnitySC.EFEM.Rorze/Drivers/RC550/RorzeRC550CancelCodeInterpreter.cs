using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitySC.EFEM.Rorze.Drivers.RC550
{
    /// <summary>
    /// Provide services to transform a cancel code into a string description for RC550.
    /// </summary>
    public static class RorzeRC550CancelCodeInterpreter
    {
        static RorzeRC550CancelCodeInterpreter()
        {
            var cancelCodeToString = new SortedDictionary<int, string>
            {
                {
                    0x0002, "Designated motion target is not equipped. "
                            + "=> Check command format."
                },
                {
                    0x0003, "Too many/too few elements. "
                            + "=> Check command format."
                },
                {
                    0x0004, "Designated command is not supported. "
                            + "=> Check command format."
                },
                {
                    0x0005, "Too many/too few parameters. "
                            + "=> Check command parameter."
                },
                {
                    0x0006, "Abnormal parameter range. "
                            + "=> Check command parameter."
                },
                {
                    0x0008, "Abnormal data. "
                            + "=> Setting value of the setting data is abnormal."
                },
                {
                    0x0009, "System preparing. "
                            + "=> Command is received during initialization."
                },
                {
                    0x000D, "Abnormal flash memory. "
                            + "=> Malfunction of the controller. Consult our inquiry office if this occurred."
                },
                {
                    0x000E, "Initialization not completed. "
                            + "=> Execute initialization command(INIT)."
                },
                {
                    0x000F, "Error - occurred state. "
                            + "=> System is in the error-occurred state. "
                            + "Resend command after resetting error. "
                            + "Some fatal errors can be reset only by \"INIT\"command."
                },
                {
                    0x0017, "Command processing. "
                            + "=> Wait until processing command is completed."
                },
                {
                    0x0019, "Designated motion target is invalid. "
                            + "=> Invalid motion target is designate"
                }
            };

            CancelCodeToString = new ReadOnlyDictionary<int, string>(cancelCodeToString);
        }

        public static IReadOnlyDictionary<int, string> CancelCodeToString { get; }
    }
}
