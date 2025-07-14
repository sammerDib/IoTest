using System;

namespace UnitySC.PM.ANA.Service.Core.BareWaferAlignment
{
    public class UnsupportedWaferException : Exception
    {
        public UnsupportedWaferException(String message) : base(message)
        {
        }
    }
}