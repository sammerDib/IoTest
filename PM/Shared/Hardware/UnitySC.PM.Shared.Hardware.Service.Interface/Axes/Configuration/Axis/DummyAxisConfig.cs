using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [Serializable]
    [DataContract]
    public class DummyAxisConfig : AxisConfig
    {
        public DummyAxisConfig()
            : base()
        {
        }
    }
}
