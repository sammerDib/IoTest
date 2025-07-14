using System;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class SafetyException : Exception
    {
        public SafetyException(string message) : base(message)
        {
        }
    }

    public class AxisSafetyException : Exception
    {
        public AxisSafetyException(string message) : base(message)
        {
        }
    }
}
