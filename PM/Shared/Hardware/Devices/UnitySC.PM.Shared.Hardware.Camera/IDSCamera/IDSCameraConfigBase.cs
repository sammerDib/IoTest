using System.Collections.Generic;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Camera.IDSCamera
{
    public class UI524xCpNirIDSCameraConfig : IDSCameraConfigBase
    {
        public override string Model => "UI524xCpNir";
    }

    public class UI324xCpNirIDSCameraConfig : IDSCameraConfigBase
    {
        public override string Model => "UI324xCpNir";

        /// <summary>
        /// GPIO line ID used for trigger OUT.
        /// </summary>
        public int GpioLineId;
    }

    public abstract class IDSCameraConfigBase : CameraConfigBase
    {
        /// <summary>
        /// Manufacturer name of camera's model.
        /// </summary>
        [XmlIgnore]
        public abstract string Model { get; }

        /// <summary>
        /// Relative file path (from the ConfigurationFolderPath) of the camera parameter set stored
        /// as an .ini file on disk and loaded during camera initialisation.
        /// </summary>
        public string ParameterFileRelativePath { get; set; }

        /// <summary>
        /// Manufacturer serial number of the camera.
        /// </summary>
        public string SerialNumber;

        /// <summary>
        /// Gain factor to set when initializing camera (multiplicator) Will be convert in
        /// percentage in material.
        /// </summary>
        public int Gain;

        /// <summary>
        /// Color mode to be set at camera initialization Exemple (Mono8, RBG12 ...).
        /// </summary>
        public string ColorMode;

        /// <summary>
        /// Mirroring effect to apply on image at camera initialization Possible value :
        /// - UpDown: Mirrors the image along the horizontal axis
        /// - LeftRight: Mirrors the image along the vertical axis
        /// </summary>
        public string RopEffect;

        /// <summary>
        /// Number of allocated memory spaces used for ring buffering.
        /// </summary>
        public int ImageMemoryBufferCount;

        /// <summary>
        /// Duration interval (in milliseconds) for the real camera framerate value update. The
        /// default value is 500 milliseconds.
        /// </summary>
        public int RefreshFramerateInterval_ms = 500;

        /// <summary>
        /// Camera mode specifications.
        /// </summary>
        public List<IDSCameraMode> CameraModes { get; set; }
    }
}
