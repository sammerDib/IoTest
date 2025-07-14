using System;

namespace UnitySC.PM.Shared.Hardware.Camera.CyberOptics
{
    public class PSF15CameraConfig : CameraConfigBase
    {
        /// <summary>
        /// Where to reach the server running on the acquisition PC.
        /// Use null for local mode.
        /// </summary>
        public string TcpServerAddress;

        /// <summary>
        /// Where to reach the server running on the acquisition PC.
        /// </summary>
        public Int32 TcpServerPort = 5000;

        /// <summary>
        /// Accuracy is better with 500µm wrap length, but 1mm can help for focusing.
        /// </summary>
        public enum WrapLength : Int32 { wrap500um = 500, wrap1000um = 1000 };

        public WrapLength WrapLength_um = WrapLength.wrap500um;

        /// <summary>
        /// Light intensity, in %. Use 0 or less to deactivate this channel group.
        /// 2 side channels, when watching, from each side, the light coming from the center (30° triangulation angle).
        /// </summary>
        public Single SideIntensity_percent;

        /// <summary>
        /// Light intensity, in %. Use 0 or less to deactivate this channel group.
        /// 2 top channels, when watching, from the top, the light coming from each side (30° triangulation angle).
        /// </summary>
        public Single TopIntensity_percent;

        /// <summary>
        /// Light intensity, in %. Use 0 or less to deactivate this channel group.
        /// 2 specular channels, when watching, from each side, the light coming from the other side (60° triangulation angle).
        /// </summary>
        public Single SpecularIntensity_percent;

        /// <summary>
        /// If set to true, SoftwareTrigger() will be automatically called, as fast as possible. Do not call it yourself in ContinuouslyTriggering mode!
        /// </summary>
        public bool ContinuouslyTriggering;

        /// <summary>
        /// If set to true, raw images (36 per site) will be fetched alongside height map (and quality).
        /// They are usefull for focusing, and to check wether saturation occurs.
        /// </summary>
        public bool FetchRawImages;

        /// <summary>
        /// One 2D image can be acquired per site. Set it's illumination up here. 3* 0s means no 2D image requested.
        /// </summary>
        public Single TwoDIntensityProjector_percent;

        public Int32 TwoDIntensityLEDRingLow_percent;
        public Int32 TwoDIntensityLEDRingHigh_percent;

        /// <summary>
        /// 2D image active or not?
        /// </summary>
        public bool Illuminate2DImage =>
            (TwoDIntensityProjector_percent > 0f) || (TwoDIntensityLEDRingLow_percent > 0) || (TwoDIntensityLEDRingHigh_percent > 0);
    }
}