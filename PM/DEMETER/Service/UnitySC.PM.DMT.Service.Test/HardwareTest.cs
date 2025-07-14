using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Test
{
    [TestCategory("RequiresHardware")]
    [TestClass]
    public class HardwareTest : BaseTest
    {
        public HardwareConfiguration Configuration;

        private AxesConfig _config;
        public static ILogger Logger;
        private MotionAxes _axes;

        [TestMethod]
        public void BasicTest()
        {
            
            string fileName = ClassLocator.Default.GetInstance<DMTFakeConfigurationManager>().DMTHardwareConfigurationFilePath;
            Configuration = XML.Deserialize<DMTHardwareConfiguration>(fileName);

            string workingDirectory = System.AppContext.BaseDirectory;

            Logger = new HardwareLogger("Info", "HardwareTest", "Debug");
            AxesConfig _config = Configuration.AxesConfigs.FirstOrDefault<AxesConfig>(c => c.IsEnabled && (c is PSDAxesConfig));
            Dictionary<String, MotionControllerBase> controllerDico = new Dictionary<string, MotionControllerBase>();
            bool failed = InitializeMotionControllers(Configuration, Logger, out controllerDico);
            EmptyReferentialManager referential = new EmptyReferentialManager();

            Assert.IsTrue(!failed);
            _axes = new PSDAxes((PSDAxesConfig)_config, controllerDico, null, Logger, referential);

            // Init Axes
            List<Message> initErrors = new List<Message>();
            _axes.Init(initErrors);
            Assert.IsTrue(initErrors.Count == 0);

            // Waiting Loading position
            WaitingForDestination(new LinearPosition(new MotorReferential(), 1));

            // Move to process position
            var pos = new Length(0, LengthUnit.Millimeter);
            _axes.Move(new PMAxisMove("Linear", pos));
            _axes.WaitMotionEnd(5000);

            // Waiting Process position
            WaitingForDestination(new LinearPosition(new MotorReferential(), 0));

            // Move to Loading position
            pos = new Length(1, LengthUnit.Millimeter);
            _axes.Move(new PMAxisMove("Linear", pos));
            _axes.WaitMotionEnd(5000);

            // Waiting Loading position
            WaitingForDestination(new LinearPosition(new MotorReferential(), 1));            
        }

        private void WaitingForDestination(LinearPosition dest)
        {
            PositionBase currPos = _axes.GetPosition();
            Assert.IsTrue(currPos is LinearPosition);

            if ((LinearPosition)currPos == dest)
                return;

            DateTime starTime = DateTime.Now;
            while (DateTime.Now.Subtract(starTime).TotalSeconds < 10)
            {
                //Get position
                PositionBase getPos = _axes.GetPosition();
                Assert.IsTrue(getPos is LinearPosition);
                if ((LinearPosition)currPos != (LinearPosition)getPos)
                {
                    LinearPosition pos = (LinearPosition)getPos;
                    if (pos.Position == dest.Position) break;
                }
            }
        }

        protected bool InitializeMotionControllers(HardwareConfiguration hardwareConfiguration, ILogger logger, out Dictionary<String, MotionControllerBase> controllerDico)
        {
            controllerDico = new Dictionary<string, MotionControllerBase>();
            bool InitFatalError = false;
            if (!hardwareConfiguration.ControllerConfigs.Any(x => x.IsEnabled))
                return InitFatalError;

            logger.Information("Motion controllers initialization starting...");

            // Create all motion controllers from config
            foreach (var config in hardwareConfiguration.MotionControllerConfigs.Where(x => x.IsEnabled))
            {
                logger.Information(string.Format("Motion controller {0} create", config.Name));

                var motionControllerLogger = new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Controller.ToString(), config.Name);
                var motionController = ControllerFactory.CreateMotionController(config, null, motionControllerLogger);
                if (!(motionController is null))
                {
                    try
                    {
                        logger.Information(string.Format("{0} Motion controller initialization started", config.Name));

                        List<Message> _initErrors = new List<Message>();
                        motionController.Init(_initErrors);

                        bool atLeastOneFatalError = _initErrors.Any(message => message.Level == MessageLevel.Fatal);
                        if (atLeastOneFatalError)
                            throw new Exception("Error during " + config.Name + " motion controller initialization");

                        controllerDico.Add(config.DeviceID, motionController);
                        logger.Information(string.Format("{0} Motion controller Status: {1} Status message: {2}", config.Name, 
                            motionController.State.Status, motionController.State.StatusMessage));
                    }
                    catch (Exception ex)
                    {
                        InitFatalError = true;
                        logger.Error(ex, string.Format("{0} Motion controller initialization error", config.Name));
                        motionController.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
            }
            return InitFatalError;
        }
    }
}
