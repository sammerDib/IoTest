using System;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.TestUtils.ObjectiveSelector.Configuration
{
    public class ObjectiveConfigFactory
    {
        public static ObjectiveConfig Build(Action<ObjectiveConfig> action = null)
        {
            var config = new ObjectiveConfig
            {
                Name = null,
                DeviceID = null,
                Coord = 0.Millimeters(),
                IsEnabled = false,
                IsSimulated = false,
                LogLevel = DeviceLogLevel.None,
                Magnification = 0,
                ObjType = ObjectiveConfig.ObjectiveType.NIR,
                SmallStepSizeXY = 0.Millimeters(),
                NormalStepSizeXY = 0.Millimeters(),
                BigStepSizeXY = 0.Millimeters(),
                SmallStepSizeZ = 0.Millimeters(),
                NormalStepSizeZ = 0.Millimeters(),
                BigStepSizeZ = 0.Millimeters(),
                DepthOfField = 0.Millimeters(),
                PiezoAxisID = null,
                IsMainObjective = false
            };
            action?.Invoke(config);
            return config;
        }
    }
}
