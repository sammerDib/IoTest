using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface.TestUtils.ObjectiveSelector.Configuration
{
    public class ObjectivesSelectorConfigBaseFactory
    {
        public static ObjectivesSelectorConfigBase Build(Action<ObjectivesSelectorConfigBase> action)
        {
            var config = new SingleObjectiveSelectorConfig
            {
                IsSimulated = false,
                IsEnabled = false,
                LogLevel = DeviceLogLevel.None,
                Name = "Objective selector #1",
                DeviceID = "objective-selector#1",
                Objectives = new List<ObjectiveConfig> { ObjectiveConfigFactory.Build() }
            };
            action?.Invoke(config);
            return config;
        }
    }
}
