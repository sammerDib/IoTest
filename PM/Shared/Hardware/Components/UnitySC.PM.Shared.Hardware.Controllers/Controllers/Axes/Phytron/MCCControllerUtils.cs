using System;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Phytron
{
    public enum MotorStatus
    {
        InPosition = 0,
        Moving = 1,
    }

    public static class Convert
    {
        public static MotorStatus MotorStatusFromString(string motorStatus)
        {
            switch (motorStatus)
            {
                case "E": return MotorStatus.InPosition;
                case "N": return MotorStatus.Moving;
                default: throw new Exception($"Motor status {motorStatus} not supported");
            }
        }
    }

    /// <summary>
    /// Non-printing characters.
    /// </summary>
    public static class NPC
    {
        public const string STX = "\u0002"; // Start of Text
        public const string ETX = "\u0003"; // End of Text
        public const string ACK = "\u0006"; // Acknowledge
        public const string NAK = "\u0015"; // Negative Acknowledge
    }
}
