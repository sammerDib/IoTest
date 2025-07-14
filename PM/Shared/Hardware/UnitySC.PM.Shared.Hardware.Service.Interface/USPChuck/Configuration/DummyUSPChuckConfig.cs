using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [Serializable]
    [DataContract]
    public class DummyUSPChuckConfig : USPChuckConfig<SubstrateSlotConfig>
    {        
    }
}

