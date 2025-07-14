using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Client.Proxy.Exceptions
{
    public sealed class NullObjectiveCalibrationException : Exception
    {
        public NullObjectiveCalibrationException() : base() { }

        public NullObjectiveCalibrationException(string message) : base(message) { }

        public NullObjectiveCalibrationException(string message, Exception innerException) : base(message, innerException) { }        
    }
}
