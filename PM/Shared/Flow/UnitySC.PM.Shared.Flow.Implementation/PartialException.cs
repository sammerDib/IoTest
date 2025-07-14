using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Flow.Implementation
{
    [Serializable]
    public class PartialException : Exception
    {
        public PartialException()
        {
        }

        public PartialException(string message) : base(message)
        {
        }

        public PartialException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PartialException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
