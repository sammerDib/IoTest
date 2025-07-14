using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class CanOverEthercatConfig
    {
        [DataMember]
        public CoeSerialComSetting CoeSerialComSetting { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CoeSerialComSetting
    {
        [DataMember]
        public string IndexComPort1 { get; set; }

        [DataMember]
        public string IndexComPort2 { get; set; }

        [DataMember]
        public EnableRtsCts EnableRtsCts { get; set; }

        [DataMember]
        public EnableXonXoffTx EnableXonXoffTx { get; set; }

        [DataMember]
        public EnableXonXoffRx EnableXonXoffRx { get; set; }

        [DataMember]
        public BaudRate BaudRate { get; set; }

        [DataMember]
        public DataFrame DataFrame { get; set; }
    }

    [Serializable]
    [DataContract]
    public class EnableRtsCts : SubIndexModuleBase
    {
    }

    [Serializable]
    [DataContract]
    public class EnableXonXoffTx : SubIndexModuleBase
    {
    }

    [Serializable]
    [DataContract]
    public class EnableXonXoffRx : SubIndexModuleBase
    {
    }

    [Serializable]
    [DataContract]
    public class BaudRate : SubIndexModuleBase
    {
    }

    [Serializable]
    [DataContract]
    public class DataFrame : SubIndexModuleBase
    {
    }

    [Serializable]
    [DataContract]
    public class SubIndexModuleBase
    {
        [DataMember]
        public string SubIndex { get; set; }

        [DataMember]
        public int SizeByte { get; set; }
    }
}
