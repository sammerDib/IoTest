using System;
using System.Runtime.Serialization;

namespace UnitySC.Equipment.Abstractions.Drivers.Common.Exceptions
{
    public class RobotCommandDeniedException : InvalidOperationException
    {
        #region Constructor

        public RobotCommandDeniedException() : base()
        {
        }

        public RobotCommandDeniedException(string message) : base(message)
        {
        }

        public RobotCommandDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public RobotCommandDeniedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        #endregion
    }
}
