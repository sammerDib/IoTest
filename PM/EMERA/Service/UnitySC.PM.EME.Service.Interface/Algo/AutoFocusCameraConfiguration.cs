using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class AutoFocusCameraConfiguration : AxesMovementConfiguration
    {
        public double SmallRangeCoeff { get; set; } = 0.1;

        public double MediumRangeCoeff { get; set; } = 0.5;

        public double LargeRangeCoeff { get; set; } = 1;

        /// <summary>
        ///  Factor apply to objective depth of field to determinate the Z Axis step size.
        /// </summary>
        public double FactorBetweenDepthOfFieldAndStepSize { get; set; } = 0.02;

        /// <summary>
        /// Rate used to limitate the default camera framerate value.
        /// The more you decrease this value, the more accurate will the result be.
        /// (But, require more time for processing).
        /// </summary>
        public double CameraFramerateLimiter { get; set; } = 1;

        /// <summary>
        /// Number of lines used to set camera AOI (Area of Interest).
        /// A higher number of lines used will result in a longer processing.
        /// </summary>
        public int CameraNbLinesAOI { get; set; } = 128;

        /// <summary>
        /// Approximative Piezo step size between two image acquisition.
        /// According camera framerate, this value while determinate the piezo speed.
        /// </summary>
        public Length PiezoStep { get; set; } = 90.Nanometers();

        /// <summary>
        /// The minimum step size for Z Axis.
        /// According camera framerate, this value while determinate the Z axis speed.
        /// </summary>
        public Length MinZStep { get; set; } = 0.6.Micrometers();

        /// <summary>
        /// The maximum step size for Z Axis.
        /// According camera framerate, this value while determinate the Z axis speed.
        /// </summary>
        public Length MaxZStep { get; set; } = 80.Micrometers();

        /// <summary>
        /// Period at which we will trigg a get position request to Z axis or piezo.
        /// </summary>
        public int PositionTrackingPeriod_ms { get; set; } = 10;

        public int MeasureNbToQualityScore { get; set; } = 50;

        public Length AutoFocusScanRange { get; set; } = 2.Millimeters();
    }
}
