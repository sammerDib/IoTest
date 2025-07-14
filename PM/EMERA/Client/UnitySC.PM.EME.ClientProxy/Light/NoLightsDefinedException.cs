using System;

namespace UnitySC.PM.EME.Client.Proxy.Light
{
    public class NoLightsDefinedException : Exception
    {
        public NoLightsDefinedException(string message) : base(message) { }
    }
}
