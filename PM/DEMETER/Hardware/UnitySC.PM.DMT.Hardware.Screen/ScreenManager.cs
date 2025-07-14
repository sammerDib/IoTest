using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Hardware.Screen
{
    public class ScreenManager
    {
        public Dictionary<string, ScreenBase> Screens { get; set; } = new Dictionary<string, ScreenBase>();
        private readonly ILogger _logger;
        private readonly IGlobalStatusServer _globalStatusServer;

        public ScreenManager(ILogger logger, IGlobalStatusServer globalStatusServer)
        {
            _logger = logger;
            _globalStatusServer = globalStatusServer;
        }

        public void Init(List<DMTScreenConfig> configs, List<ScreenDensitronDM430GNControllerConfig> screensDensitronConfig,
            Dictionary<string, ControllerBase> controllers)
        {
            var confManager = ClassLocator.Default.GetInstance<IDMTServiceConfigurationManager>();
            ControllerBase controller;

            foreach (DMTScreenConfig conf in configs.Where(c => c.IsEnabled))
            {
                bool found = controllers.TryGetValue(conf.ControllerID, out controller);
                if (!found || (controller == null))
                    throw new Exception("Controller of the configuration was not found [deviceID = " + conf.DeviceID + ", ControllerId = " + conf.ControllerID + "]");

                _logger.Information("Initializing screen " + conf.Name);
                var screenDensitronConfig = screensDensitronConfig.First(a => a.DeviceID == conf.ControllerID);
                if (conf.IsSimulated && confManager.MilIsSimulated)
                {
                    DummyMatroxScreen screen = new DummyMatroxScreen();
                    screen.Init(conf, _globalStatusServer, screenDensitronConfig, (DummyScreenController)controller);
                    Screens.Add(conf.DeviceID, screen);
                }
                else
                {
                    MatroxScreen screen = new MatroxScreen();
                    screen.Init(conf, _globalStatusServer, screenDensitronConfig, (ScreenController)controller);
                    Screens.Add(conf.DeviceID, screen);
                }
            }

            // Set the other screens for each screen
            foreach (var screen in Screens)
            {
                screen.Value.OtherScreens = Screens.Where(s => s.Key != screen.Value.DeviceID).Select(s => s.Value).ToList();
            }
        }
    }
}
