using System;
using System.Windows;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Camera
{
    [XmlInclude(typeof(VieworksVtMatroxCameraConfig))]
    [XmlInclude(typeof(M1280MatroxCameraConfig))]
    [XmlInclude(typeof(VX29MGMatroxCameraConfig))]
    [XmlInclude(typeof(VieworksVH16MG2M4MatroxCameraConfig))]
    [XmlInclude(typeof(VieworksVC65MXM31CameraConfig))]
    [XmlInclude(typeof(AVTGE4900CameraConfig))]
    [XmlInclude(typeof(AVTGE4400CameraConfig))]
    [XmlInclude(typeof(BaslerAce2ProCameraConfig))]
    [XmlInclude(typeof(VC155MXMatroxCameraConfig))]
    [XmlInclude(typeof(VC151MXMatroxCameraConfig))]
    [XmlInclude(typeof(ELIIXA16KMatroxCameraConfig))]
    [XmlInclude(typeof(MatroxCameraConfigBase))]
    [XmlInclude(typeof(UI524xCpNirIDSCameraConfig))]
    [XmlInclude(typeof(UI324xCpNirIDSCameraConfig))]
    [XmlInclude(typeof(IDSCameraConfigBase))]
    [XmlInclude(typeof(Spyder3MatroxCameraConfig))]
    [XmlInclude(typeof(DummyCameraConfig))]
    public class CameraConfigBase : IDeviceConfiguration, IModuleInformation
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }

        [XmlIgnore]
        public string ObjectivesSelectorID { get; set; }

        [XmlIgnore]
        public string ModuleID { get; set; }

        [XmlIgnore]
        public string ModuleName { get; set; }

        [XmlIgnore]
        public ModulePositions ModulePosition { get; set; } // UP or DOWN (TOP or BOTTOM)

        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }
        public int Depth { get; set; } = 8;
        public CameraMode CameraMode { get; set; } // FIXME: CRITICAL! > do not depend on specific Matrox cameras type here

        public string DeadPixelsFile { get; set; }

        public bool IsMainCamera { get; set; }

        public Size DefaultImageResolution { get; set; }
               
    }
}
