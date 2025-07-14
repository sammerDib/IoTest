using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Laser
{
    [Serializable]
    [DataContract]
    public class SMD12LaserConfig : LaserConfig
    {
        public SMD12LaserConfig() : base()
        {
        }
    }
}
