using System;
using System.Windows;
using System.Xml.Serialization;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class VX29MGMatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "VX29MG";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_GIGE_VISION;
    }

    public class VC155MXMatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "VC155MX";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_RADIENTCXP;
    }

    public class VC151MXMatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "VC151MX";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_RAPIXOCXP;
    }

    public class VieworksVH16MG2M4MatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "VH-16MG2-M4";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_GIGE_VISION;
    }

    public class VieworksVC65MXM31CameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "VC-65MX-M31";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_RADIENTCXP;
    }

    public class AVTGE4900CameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "GE4900";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_GIGE_VISION;
    }

    public class AVTGE4400CameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "GE4400";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_GIGE_VISION;
    }

    public class BaslerAce2ProCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "Ace2Pro";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_GIGE_VISION;
    }

    public class VieworksVtMatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "VT...";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_RADIENTCXP;

        /// <summary>
        /// Must be set before initialization.
        /// </summary>
        public new CameraMode CameraMode = CameraMode.tdi;

        /// <summary>
        /// Buffers will be allocated for up to GlobalBuffersSize_bytes.
        /// Since the buffer size will depend on CameraMode, the number of buffers will depend on camera mode too (and also on TdiModeLineBufferingCount in TDI mode).
        /// </summary>
        public Int32 GlobalBuffersSize_bytes = 460 * 1024 * 1024;

        /// <summary>
        /// In TDI mode, the camera acquires lines...
        /// But the frame grabber can stitch thoses lines up before calling the image ready callback function. This number is not linked to the number of TDI lines (actual Y size of the camera).
        /// This can be usefull to avoid stressing the CPU with a callback call per line.
        /// Each MilImage size, in TDI mode, will be CamPixelPidth * TdiModeLineBufferingCount. (in rectangle mode, they will be CamPixelPidth * TdiHeight)
        /// </summary>
        public Int32 TdiModeLineBufferingCount = 100;

        /// <summary>
        /// The camera should be kept below the specified temperature.
        /// </summary>
        public double TemperatureLimit_degCel = 50d;
    }

    public class M1280MatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "M1280";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_GIGE_VISION;
    }

    public class ELIIXA16KMatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "ELIXAC4CCP1605";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_RADIENTCXP;
    }

    public class Spyder3MatroxCameraConfig : MatroxCameraConfigBase
    {
        public override string Model => "Spyder3";
        public override string MilSystemDescriptor { get; set; } = MIL.M_SYSTEM_SOLIOS;

        public string DCFName => "Spyder3.dcf";

        // FIXME(JPR): for now they are not used but later when we have several camera on one card, it may be useful. This configuration may imply to refactor how the camera digitizer are created
        public int SoliosCardId;

        public int DigitizerId;

        public string CameraId;

        public class COM
        {
            public string Port { get; set; }
            public int BaudRate { get; set; }
        }

        public COM Com;
    }

    public abstract class MatroxCameraConfigBase : CameraConfigBase
    {
        /// <summary>
        /// Represents camera settings to use
        /// for example: at camera hardware initialisation
        /// </summary>

        public enum FanOperationMode
        {
            Off,
            On,
            Temperature
        };
        public enum HotPixelCorrection
        {
            Off,
            On
        };
        public enum FlatFieldCorrection
        {
            Off,
            On
        };

        /// <summary>
        /// Represents a Region Of Interest (ROI) on a camera
        /// </summary>
        public class FieldOfView
        {
            [XmlAttribute]
            public string Name;

            public long OffsetX, OffsetY, Width, Height;
        }

        /// <summary>
        /// Descripteur MIL du sytème (ex: M_SYSTEM_GIGE_VISION)
        /// </summary>
        public abstract string MilSystemDescriptor { get; set; }

        /// <summary>
        /// Modèle de camera
        /// </summary>
        [XmlIgnore]
        public abstract string Model { get; }

        /// <summary>
        /// Where to find the .dcf config files for the camera.
        /// </summary>
#warning TODO FDE à déplacer
        public string DcfFolderPath = @"C:\Program Files\Matrox Imaging\Drivers\RadientCXP\dcf";

        public string SerialNumber;
        public double Gain;
        public double GrabTimeout = 80;

        //EC//Additional OPTIONAL settings used by "DMTHardwareManager" to initialise "VC151MXCamera" camera parameters at startup
        public Rect AOI= new Rect(0, 0, 14192, 10640);
        public bool DefectivePixelCorrection = false;
        public bool DynamicDefectivePixelCorrection = false;
        public double BlackLevel = 0;
        [XmlElement("FanOperationMode")]
        public FanOperationMode FanOperation= FanOperationMode.On;
        [XmlElement("HotPixelCorrection")]
        public HotPixelCorrection HotPixel = HotPixelCorrection.Off;
        [XmlElement("FlatFieldCorrection")]
        public FlatFieldCorrection FlatField= FlatFieldCorrection.Off;

    }
}
