using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface;

using static UnitySC.PM.ANA.Service.Interface.ObjectiveConfig;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class ObjectiveToCalibrate
    {
        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public string ObjectiveName { get; set; }

        [DataMember]
        public ModulePositions Position { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public bool IsMain { get; set; }

        [DataMember]
        public double Magnification { get; set; }

        [DataMember]
        public ObjectiveType ObjType { get; set; }
    }
}
