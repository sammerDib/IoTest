using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Hardware.Screen
{
    [Serializable]
    public class DMTScreenConfig : ScreenConfig
    {
        public double ScreenWhiteDisplayTimeSec { get; set; }

        public string Resolution;

        /// <summary>
        /// Temps nécessaire pour que l'image soit effectivement affichée sur l'écran. En secondes.
        /// </summary>
        public double ScreenStabilizationTimeSec;

        /// <summary>
        /// Display position relative to the computer's main screen, as defined in the display properties
        /// </summary>
        public DisplayPosition DisplayPosition;

        public bool PowerOn { get; set; }

        public bool FanAutoOn { get; set; }

        public double PixelPitchHorizontal;

        public double PixelPitchVertical;

        public Polarisation Polarisation;

        public double ScreenTemperatureLimit { get; set; }

        public int BlackoutDurationMs { get; set; }

        [XmlArray("WavelengthPeaks")]
        [XmlArrayItem("Wavelength")]
        public List<Length> WavelengthPeaks;
    }
}
