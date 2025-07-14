using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device
{
    [DataContract]
    public class MatroxCameraInfo : CameraInfo
    {
        [DataMember]
        public int MinWidth { get; set; }

        [DataMember]
        public int MaxWidth { get; set; }

        [DataMember]
        public int MinHeight { get; set; }

        [DataMember]
        public int MaxHeight { get; set; }

        [DataMember]
        public int WidthIncrement { get; set; }

        [DataMember]
        public int HeightIncrement { get; set; }
    }
}
