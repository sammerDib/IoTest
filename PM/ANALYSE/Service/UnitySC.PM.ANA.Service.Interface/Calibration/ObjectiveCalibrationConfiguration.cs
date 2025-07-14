using System.Collections.Generic;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Interface.ObjectiveConfig;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    public class ObjectiveCalibrationConfiguration : DefaultConfiguration
    {
        public static List<DefaultObjectiveCalibrationValue> DefaultObjectiveCalibrationValues { get; set; } = new List<DefaultObjectiveCalibrationValue>()
        {
            // Up
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 5,
                ObjType = ObjectiveType.NIR,
                PixelSizeX = 2.125.Micrometers(),
                PixelSizeY = 2.125.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up
            },
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 5,
                ObjType = ObjectiveType.INT,
                PixelSizeX = 2.1246.Micrometers(),
                PixelSizeY = 2.1246.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up
            },
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 2,
                ObjType = ObjectiveType.VIS,
                PixelSizeX = 5.3.Micrometers(),
                PixelSizeY = 5.3.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up
            },
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 10,
                ObjType = ObjectiveType.NIR,
                PixelSizeX = 1.0582.Micrometers(),
                PixelSizeY = 1.0582.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up
            },
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 10,
                ObjType = ObjectiveType.INT,
                PixelSizeX = 1.058.Micrometers(),
                PixelSizeY = 1.058.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up
            },
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 20,
                ObjType = ObjectiveType.NIR,
                PixelSizeX = 0.5291.Micrometers(),
                PixelSizeY = 0.5291.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up                
            },
              new DefaultObjectiveCalibrationValue()
            {
                Magnification = 20,
                ObjType = ObjectiveType.INT,
                PixelSizeX = 0.53.Micrometers(),
                PixelSizeY = 0.53.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up                
            },
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 50,
                ObjType = ObjectiveType.NIR,
                PixelSizeX = 0.2134.Micrometers(),
                PixelSizeY = 0.2134.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up
                
            },
             new DefaultObjectiveCalibrationValue()
            {
                Magnification = 50,
                ObjType = ObjectiveType.INT,
                PixelSizeX = 0.2111.Micrometers(),
                PixelSizeY = 0.2111.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Up              
            },

             // Down
            new DefaultObjectiveCalibrationValue()
            {
                Magnification = 5,
                ObjType = ObjectiveType.NIR,
                PixelSizeX = 1.055.Micrometers(),
                PixelSizeY = 1.055.Micrometers(),
                AutoFocusScanSize = 2.Millimeters(),
                Position = ModulePositions.Down             
            }
        };
    }

    public class DefaultObjectiveCalibrationValue
    {
        public double Magnification { get; set; }
        public ObjectiveType ObjType { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public Length AutoFocusScanSize { get; set; }        
        public ModulePositions Position { get; set; }
    }
}
