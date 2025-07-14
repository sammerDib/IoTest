using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Controller
{
    [Serializable]
    [DataContract]
    public class SMC100ControllerConfig : ControllerConfig
    {
        [DataMember]
        public string COMPort;
        [DataMember]
        public int BaudRate;
        [DataMember]
        public string Parity;
        [DataMember]
        public int DataBits;
        [DataMember]
        public string StopBits;
        [DataMember]
        public int TimeOut;
        [DataMember]
        public int UpdateInterval;
        [DataMember]
        public double ReferenceHeightPosition;
        [DataMember]
        public List<NewPortSubDriveConfig> SubDrivesList;
    }

    [Serializable]
    [DataContract]
    public class NewPortSubDriveConfig
    {
        public EnumDriveAxisId DriveAxisId;
        public int DriveAddress;
        public double WaferEdgePosition; //Only used for ada generation TODO check if still needed
    }

    public enum EnumDriveAxisId { TopX, TopZ, BottomX, BevelTopZ, ApexZ };


}
