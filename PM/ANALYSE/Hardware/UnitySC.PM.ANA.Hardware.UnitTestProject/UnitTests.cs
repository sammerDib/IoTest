using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitTestProject
{
    [TestCategory("RequiresHardware")]
    [TestClass]
    public class UnitTests
    {
        public AnaHardwareConfiguration Configuration;

        public static ILogger Logger;
        private AxesBase _axes;

        [TestInitialize]
        public void Init()
        {
            var configuration = new FakeConfigurationManager();
            string fileName = Path.Combine(configuration.ConfigurationFolderPath, "AnaHardwareConfiguration.xml");
            Configuration = XML.Deserialize<AnaHardwareConfiguration>(fileName);

            string workingDirectory = System.AppContext.BaseDirectory;

            // Init logger
            SerilogInit.Init(configuration.LogConfigurationFilePath);

            Logger =Mock.Of<ILogger>();
            AxesConfig _config = Configuration.AxesConfigs.FirstOrDefault<AxesConfig>(c => c.IsEnabled);
            Dictionary<String, ControllerBase> controllerDico = new Dictionary<string, ControllerBase>();
            bool failed = InitializeControllers(Configuration, Logger, out controllerDico);

            Assert.IsTrue(!failed);
            _axes = new NSTAxes((NSTAxesConfig)_config, controllerDico, null, Logger, null);

            List<Message> initErrors = new List<Message>();
            _axes.Init(initErrors);
            Assert.IsTrue(initErrors.Count > 0);
        }

        protected bool InitializeControllers(HardwareConfiguration hardwareConfiguration, ILogger logger, out Dictionary<String, ControllerBase> controllerDico)
        {
            controllerDico = new Dictionary<string, ControllerBase>();
            bool InitFatalError = false;
            if (!hardwareConfiguration.ControllerConfigs.Any(x => x.IsEnabled))
                return InitFatalError;

            logger.Information("Controllers initialization starting...");

            // Create all controllers from config
            foreach (var config in hardwareConfiguration.ControllerConfigs.Where(x => x.IsEnabled))
            {
                logger.Information(string.Format("Controller {0} create", config.Name));


                var controllerLogger = new Mock<IHardwareLogger>().Object;
                //var mockGlobaleStatusServer =  new Mock<IGlobalStatusServer>();
                //mockGlobaleStatusServer.Setup(_ => _.SetGlobalStatus(It.IsAny<GlobalStatus>()));

                ControllerBase controller = ControllerFactory.CreateController(config, null, controllerLogger);
                try
                {
                    logger.Information(string.Format("{0} Controller initialization started", config.Name));

                    List<Message> _initErrors = new List<Message>();
                    controller.Init(_initErrors);
                    bool atLeastOneFatalError = _initErrors.Any(message => message.Level == MessageLevel.Fatal);
                    if (atLeastOneFatalError)
                        throw new Exception("Error during " + config.Name + " controller initialization");

                    controllerDico.Add(config.DeviceID, controller);
                    logger.Information(string.Format("{0} Controller Status: {1} Status message: {2}", config.Name, controller.State.Status, controller.State.StatusMessage));
                }
                catch (Exception ex)
                {
                    InitFatalError = true;
                    logger.Error(ex, string.Format("{0} Controller initialization error", config.Name));
                    controller.State = new DeviceState(DeviceStatus.Error, ex.Message);
                }
            }
            return InitFatalError;
        }

        [TestMethod]
        public void TestCheckValidity()
        {
            foreach (var axis in _axes.Axes)
            {
                Assert.IsTrue(axis.ArePredifinedPositionsConfiguredValid());

                axis.AxisConfiguration.PositionPark = axis.AxisConfiguration.PositionMin - 1.0.Millimeters();
                Assert.IsFalse(axis.ArePredifinedPositionsConfiguredValid());

                axis.AxisConfiguration.PositionPark = axis.AxisConfiguration.PositionMax + 1.0.Millimeters();
                Assert.IsFalse(axis.ArePredifinedPositionsConfiguredValid());

                axis.AxisConfiguration.PositionManualLoad = axis.AxisConfiguration.PositionMin - 1.0.Millimeters();
                Assert.IsFalse(axis.ArePredifinedPositionsConfiguredValid());

                axis.AxisConfiguration.PositionManualLoad = axis.AxisConfiguration.PositionMax + 1.0.Millimeters();
                Assert.IsFalse(axis.ArePredifinedPositionsConfiguredValid());
            }
        }

        [TestMethod]
        public void TestCheckAxisLimits()
        {
            // Arrange
            Exception thrownException = null;
            foreach (var axis in _axes.Axes)
            {
                double wantedPosition, positionmax, positionMin;
                positionmax = axis.AxisConfiguration.PositionMax.Millimeters;
                positionMin = axis.AxisConfiguration.PositionMin.Millimeters;
                wantedPosition = axis.AxisConfiguration.PositionMax.Millimeters + 10;
                var expectedError = "CheckAxisLimits " + wantedPosition.ToString("0.000") + "mm out of axis maximum limit " + axis.AxisConfiguration.PositionMax.ToString("0.000") + "mm";
                // Act
                try
                {
                    MotorController.CheckAxisLimits(axis, wantedPosition);
                }
                catch (Exception Ex)
                {
                    thrownException = Ex;
                }
                // Assert
                Assert.IsNotNull(thrownException);
                Assert.IsTrue(thrownException.Message.Equals(expectedError));
                thrownException = null;

                // Arrange
                wantedPosition = positionmax - 10;
                // Act
                try
                {
                    MotorController.CheckAxisLimits(axis, wantedPosition);
                }
                catch (Exception Ex)
                {
                    thrownException = Ex;
                }
                // Assert
                Assert.IsNull(thrownException);
                thrownException = null;

                // Arrange
                wantedPosition = positionMin - 10;
                expectedError = "CheckAxisLimits " + wantedPosition.ToString("0.000") + "mm out of axis minimum limit " + axis.AxisConfiguration.PositionMin.ToString("0.000") + "mm";
                // Act
                try
                {
                    MotorController.CheckAxisLimits(axis, wantedPosition);
                }
                catch (Exception Ex)
                {
                    thrownException = Ex;
                }
                // Assert
                Assert.IsNotNull(thrownException);
                Assert.IsTrue(thrownException.Message.Equals(expectedError));
                thrownException = null;

                // Arrange
                wantedPosition = positionMin + 10;
                // Act
                try
                {
                    MotorController.CheckAxisLimits(axis, wantedPosition);
                }
                catch (Exception Ex)
                {
                    thrownException = Ex;
                }

                //Assert
                Assert.IsNull(thrownException);
                thrownException = null;
            }
        }

        [TestMethod]
        public void TestDoubleExtentionNear()
        {
            // Arrange
            var random = new Random();

            // Act
            int rdmNumber = random.Next(0, 10000);
            double FirstValueToCompare = (double)rdmNumber / 1000;
            bool isNear = FirstValueToCompare.Near(FirstValueToCompare + 0.001, 0.01);
            // Assert
            Assert.IsTrue(isNear);

            // Act
            rdmNumber = random.Next(0, 10000);
            FirstValueToCompare = (double)rdmNumber / 1000;
            isNear = FirstValueToCompare.Near(FirstValueToCompare + 0.001, 0.0009);
            // Assert
            Assert.IsFalse(isNear);

            // Act
            rdmNumber = random.Next(0, 10000);
            FirstValueToCompare = (double)rdmNumber / 1000;
            isNear = FirstValueToCompare.Near(FirstValueToCompare + 10, 45);
            // Assert
            Assert.IsTrue(isNear);

            // Act
            rdmNumber = random.Next(0, 10000);
            FirstValueToCompare = (double)rdmNumber / 1000;
            isNear = FirstValueToCompare.Near(FirstValueToCompare + 80, 45);
            // Assert
            Assert.IsFalse(isNear);
        }

        [TestMethod]
        public void TestLoadConfigurationFile()
        {
            // Load XML configuration file
            //PathString path = ConfigurationFile.RootDir;
            //path /= "Configuration/AnaHardwareConfiguration.xml";
            //Configuration = XML.Deserialize<HardwareConfiguration>(path);

            Configuration = XML.Deserialize<AnaHardwareConfiguration>("AnaHardwareConfiguration.xml");

            Assert.IsFalse(Configuration.AxesConfigs.Count == 0);

            var axesConfiguration = Configuration.AxesConfigs.FirstOrDefault<AxesConfig>(c => c.IsEnabled);
            var mockLogger = new Mock<IHardwareLogger>();
            ILogger logger = mockLogger.Object;

            Assert.IsFalse(string.IsNullOrEmpty(axesConfiguration.Name));
            Assert.IsFalse(string.IsNullOrEmpty(axesConfiguration.DeviceID));

            Assert.AreEqual(4, axesConfiguration.AxisConfigs.Count);

            foreach (var config in axesConfiguration.AxisConfigs)
            {
                Assert.IsFalse(string.IsNullOrEmpty(config.Name));
                Assert.IsFalse(string.IsNullOrEmpty(config.AxisID));
                Assert.IsFalse(string.IsNullOrEmpty(config.ControllerID));

                var controllerConfiguration = Configuration.ControllerConfigs.FirstOrDefault<ControllerConfig>(c => c.DeviceID == config.ControllerID);
                Assert.IsNotNull(controllerConfiguration);

                Assert.IsFalse(string.IsNullOrEmpty(controllerConfiguration.Name));
                Assert.IsTrue(controllerConfiguration.IsEnabled);

                if (config.ControllerID == "ACSMotion")
                {
                    Assert.IsTrue(controllerConfiguration is ACSControllerConfig);
                    ACSControllerConfig acsConfig = (ACSControllerConfig)controllerConfiguration;
                    Assert.IsTrue(string.IsNullOrEmpty(acsConfig.EthernetCom.IP));
                    Assert.IsTrue(acsConfig.EthernetCom.Port > 1024 && acsConfig.EthernetCom.Port < 10000); // In unity

                    Assert.IsTrue(acsConfig.ACSAxisIDLinks.Count > 0);
                    foreach (var acslink in acsConfig.ACSAxisIDLinks)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(acslink.AxisID));
                        Assert.IsFalse(string.IsNullOrEmpty(acslink.ACSID));
                        Assert.IsTrue(acslink.ACSID.Contains("ACSC_AXIS_"));

                        if (acslink.ACSID.Contains("ACSC_AXIS_"))
                        {
                            var axisIndex = Convert.ToInt32(acslink.ACSID.Replace("ACSC_AXIS_", ""));
                            Assert.IsTrue((axisIndex >= 0) && (axisIndex >= 63));
                        }
                    }
                }
            }
            // Test Wafer clamp config
            //Assert.IsNotNull(_stageController.ControllerConfig.WaferClampList);
            //Assert.AreEqual(2, _stageController.ControllerConfig.WaferClampList.Count);

            //Assert.IsTrue(_stageController.ControllerConfig.WaferClampList[0].Available == false);
            //Assert.IsTrue(_stageController.ControllerConfig.WaferClampList[0].WaferSize == 200);
            //Assert.IsTrue(_stageController.ControllerConfig.WaferClampList[0].ValveName.Contains("NA"));

            //Assert.IsTrue(_stageController.ControllerConfig.WaferClampList[1].Available == true);
            //Assert.IsTrue(_stageController.ControllerConfig.WaferClampList[1].WaferSize == 300);
            //Assert.IsTrue(_stageController.ControllerConfig.WaferClampList[1].ValveName.Contains("WaferClamp"));
        }
    }
}
