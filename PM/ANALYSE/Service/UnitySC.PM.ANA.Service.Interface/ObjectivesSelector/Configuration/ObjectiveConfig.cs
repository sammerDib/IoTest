using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    public class ObjectiveConfig : IDeviceConfiguration
    {
        public enum ObjectiveType
        { NIR, INT, VIS }

        public string Name { get; set; }
        public string DeviceID { get; set; }

        /// <summary>Objective coordinate in its objective selector (mm)</summary>
        /// To find the coordinates in FPMS, search for "physicalPosition" in ObjectiveConfiguration.xml Then find the "SlotConfiguration" in GlobalMotionFile.xml
        public Length Coord { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsSimulated { get; set; }
        public DeviceLogLevel LogLevel { get; set; }
        public double Magnification { get; set; }
        public ObjectiveType ObjType { get; set; }
        public Length SmallStepSizeXY { get; set; }
        public Length NormalStepSizeXY { get; set; }
        public Length BigStepSizeXY { get; set; }
        public Length SmallStepSizeZ { get; set; }
        public Length NormalStepSizeZ { get; set; }
        public Length BigStepSizeZ { get; set; }
        public Length DepthOfField { get; set; }
        public string PiezoAxisID { get; set; }
        public bool IsMainObjective { get; set; }
    }
}
