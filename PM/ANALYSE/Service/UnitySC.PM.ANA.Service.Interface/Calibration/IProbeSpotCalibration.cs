using System;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    
    [XmlInclude(typeof(LiseHFObjectiveSpotCalibration))]
    public interface IProbeSpotCalibration
    {
        /// <summary>
        /// Last Modification saved date
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// Objective Device Id link to this calibration
        /// </summary>
        string ObjectiveDeviceId { get; set; }

        /// <summary>
        /// X axis Offset from Centered reference (red-cross) in image to Probe Centered Position
        /// </summary>
        Length XOffset { get; set; }

        /// <summary>
        /// Y axis Offset from Centered reference (red-cross) in image to Probe Centered Position
        /// </summary>
        Length YOffset { get; set; }

        /// <summary>
        /// PixelSize X used to compute X Axis length
        /// </summary>
        Length PixelSizeX { get; set; }

        /// <summary>
        /// PixelSize Y used to compute X Axis length
        /// </summary>
        Length PixelSizeY { get; set; } 
    }
}
