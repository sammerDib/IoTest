using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.DMT.Service.Test;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Test
{
    [TestCategory("RequiresHardware")]
    [TestClass]
    public class AxesTests : BaseTest
    {
        public HardwareConfiguration Configuration;

        public static ILogger Logger;
        private MotionAxes _axes;

        [TestMethod]
        public void InitializeControllers_Test()
        {
            // Configuration
            var currentConfiguration = new DMTFakeConfigurationManager();
            string fileName = currentConfiguration.DMTHardwareConfigurationFilePath;
            Configuration = XML.Deserialize<DMTHardwareConfiguration>(fileName);

            string workingDirectory = System.AppContext.BaseDirectory;

            Logger = new HardwareLogger("Info", "UnitTests", "Debug");
            AxesConfig _config = Configuration.AxesConfigs.FirstOrDefault<AxesConfig>(c => c.IsEnabled);
            Dictionary<String, MotionControllerBase> controllerDico = new Dictionary<string, MotionControllerBase>();
            bool failed = InitializeMotionControllers(Configuration, Logger, out controllerDico);

            Assert.IsTrue(!failed);
            _axes = new PSDAxes((PSDAxesConfig)_config, controllerDico, null, Logger, null);

            List<Message> initErrors = new List<Message>();
            _axes.Init(initErrors);
            Assert.IsTrue(initErrors.Count == 0);
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

        [TestMethod]
        public void TestCheckValidity()
        {
            InitializeControllers_Test();
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
            InitializeControllers_Test();
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
                expectedError = "CheckAxisLimits " + wantedPosition.ToString("0.000") + "mm out of axis maximum limit " + axis.AxisConfiguration.PositionMax.ToString("0.000") + "mm";

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
                Assert.IsNotNull(thrownException);
                Assert.IsTrue(thrownException.Message.Equals(expectedError));
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
            // Configuration
            var currentConfiguration = new DMTFakeConfigurationManager();
            string fileName = currentConfiguration.DMTHardwareConfigurationFilePath;
            Configuration = XML.Deserialize<DMTHardwareConfiguration>(fileName);

            Assert.IsFalse(Configuration.AxesConfigs.Count == 0);

            var axesConfiguration = Configuration.AxesConfigs.FirstOrDefault<AxesConfig>(c => c.IsEnabled);

            IHardwareLogger logger = Mock.Of<IHardwareLogger>();

            Assert.IsFalse(string.IsNullOrEmpty(axesConfiguration.Name));
            Assert.IsFalse(string.IsNullOrEmpty(axesConfiguration.DeviceID));

            Assert.IsTrue(axesConfiguration.AxisConfigs.Count == 1);

            foreach (var config in axesConfiguration.AxisConfigs)
            {
                Assert.IsFalse(string.IsNullOrEmpty(config.Name));
                Assert.IsFalse(string.IsNullOrEmpty(config.AxisID));
                Assert.IsFalse(string.IsNullOrEmpty(config.ControllerID));

                var controllerConfiguration = Configuration.MotionControllerConfigs.FirstOrDefault<ControllerConfig>(c => c.DeviceID == config.ControllerID);
                Assert.IsNotNull(controllerConfiguration);

                Assert.IsFalse(string.IsNullOrEmpty(controllerConfiguration.Name));
                Assert.IsTrue(controllerConfiguration.IsEnabled);                
            }
        }
    }
}
