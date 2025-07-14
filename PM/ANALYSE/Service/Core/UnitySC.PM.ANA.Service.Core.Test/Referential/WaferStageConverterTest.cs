using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Referential;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Referential
{
    [TestClass]
    public class WaferStageConverterTest
    {
        protected AnaHardwareManager SimulatedHardwareManager;
        protected CalibrationManager CalibManager;

        protected const string ObjectiveUpId = "objectiveUpId";
        protected const string ObjectiveBottomId = "objectiveBottomId";
        protected const double Precision = 1e-10;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            // Configuration
            var configurationManager = new FakeConfigurationManager();

            // Init logger
            SerilogInit.Init(configurationManager.LogConfigurationFilePath);
            ClassLocator.ExternalInit(container, true);

            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => new FakeConfigurationManager(), true);
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);
            ClassLocator.Default.Register(() => new FDCManager("test", "test"), true);
            ClassLocator.Default.Register<CalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath));
            
            ClassLocator.Default.Register<IHardwareLoggerFactory>(() => new Mock<IHardwareLoggerFactory>().Object);
            ClassLocator.Default.Register<AnaHardwareManager>(() =>
            {
                var mockManager = new Mock<AnaHardwareManager>(new SerilogLogger<AnaHardwareManager>(), ClassLocator.Default.GetInstance<IHardwareLoggerFactory>(), new FakeConfigurationManager());
                mockManager.Setup(_ => _.GetObjectiveInUseByPosition(ModulePositions.Up)).Returns(new Interface.ObjectiveConfig() { DeviceID = ObjectiveUpId });
                mockManager.Setup(_ => _.GetObjectiveInUseByPosition(ModulePositions.Down)).Returns(new Interface.ObjectiveConfig() { DeviceID = ObjectiveBottomId });
                return mockManager.Object;
            });
            SimulatedHardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

            CalibManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            CalibManager.UpdateCalibration(new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>()
                {
                    new ObjectiveCalibration()
                    {
                        DeviceId = ObjectiveUpId,
                        ZOffsetWithMainObjective = 0.Millimeters(),
                    },
                    new ObjectiveCalibration()
                    {
                        DeviceId = ObjectiveBottomId,
                        ZOffsetWithMainObjective = 0.Millimeters(),
                    }
                }
            });
        }

        [TestMethod]
        public void Expect_stage_referential_when_convert_from_wafer_to_stage()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);

            // Act
            var waferToStageConverter = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, null);
            var resultPosition = waferToStageConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(new StageReferential(), resultPosition.Referential);
        }

        [TestMethod]
        public void Expect_wafer_referential_when_convert_from_stage_to_wafer()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);

            // Act
            var stageToWaferConverter = new StageToWaferConverter(SimulatedHardwareManager, CalibManager, null);
            var resultPosition = stageToWaferConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(new WaferReferential(), resultPosition.Referential);
        }

        [TestMethod]
        public void Expect_same_position_when_convert_from_wafer_to_stage_without_wafer_referential_settings()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 5, 10, 15, 20);

            // Act
            var waferToStageConverter = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, null);
            var resultPosition = waferToStageConverter.Convert(initialPosition);

            // Assert
            var expectedPosition = new XYZTopZBottomPosition(new StageReferential(), 5, 10, 15, 20);
            Assert.AreEqual(expectedPosition.X, resultPosition.X);
            Assert.AreEqual(expectedPosition.Y, resultPosition.Y);
            Assert.AreEqual(expectedPosition.ZTop, resultPosition.ZTop);
            Assert.AreEqual(expectedPosition.ZBottom, resultPosition.ZBottom);
            Assert.AreEqual(expectedPosition.Referential, resultPosition.Referential);
        }

        [TestMethod]
        public void Expect_same_position_when_convert_from_stage_to_wafer_without_wafer_referential_settings()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 5, 10, 15, 20);

            // Act
            var stageToWaferConverter = new StageToWaferConverter(SimulatedHardwareManager, CalibManager, null);
            var resultPosition = stageToWaferConverter.Convert(initialPosition);

            // Assert
            var expectedPosition = new XYZTopZBottomPosition(new WaferReferential(), 5, 10, 15, 20);
            Assert.AreEqual(expectedPosition.X, resultPosition.X);
            Assert.AreEqual(expectedPosition.Y, resultPosition.Y);
            Assert.AreEqual(expectedPosition.ZTop, resultPosition.ZTop);
            Assert.AreEqual(expectedPosition.ZBottom, resultPosition.ZBottom);
            Assert.AreEqual(expectedPosition.Referential, resultPosition.Referential);
        }

        [TestMethod]
        public void Expect_correct_shift_when_converting_from_stage_to_wafer()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 10, 10, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 1.Millimeters(),
                ShiftY = 1.Millimeters()
            };
            var stageToWaferConverter = new StageToWaferConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = stageToWaferConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(9, resultPositionStage.X);
            Assert.AreEqual(9, resultPositionStage.Y);
        }

        [TestMethod]
        public void Expect_correct_shift_when_converting_from_wafer_to_stage()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 10, 10, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 1.Millimeters(),
                ShiftY = 1.Millimeters()
            };
            var waferToStageConverter = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = waferToStageConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(11, resultPositionStage.X);
            Assert.AreEqual(11, resultPositionStage.Y);
        }

        [TestMethod]
        public void Expect_correct_clockwise_rotation_when_converting_from_stage_to_wafer()
        {
            // When converting from stage to wafer, we have to "cancel"
            // the angle detected by the alignment process.
            // To cancel that angle, the rotation should be clockwise

            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 10, 10, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 0.Millimeters(),
                ShiftY = 0.Millimeters(),
                WaferAngle = 90.Degrees()
            };
            var stageToWaferConverter = new StageToWaferConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = stageToWaferConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(10.0, resultPositionStage.X, Precision, "When rotated of 90°, abcissae should not change");
            Assert.AreEqual(-10.0, resultPositionStage.Y, Precision, "When rotated of 90°, ordinate must change to -10");
        }

        [TestMethod]
        public void Expect_correct_anticlockwise_rotation_when_converting_from_wafer_to_stage()
        {
            // When converting from wafer to stage, we have to "add"
            // the angle detected by the alignment process.
            // To add that angle, the rotation should be anticlockwise

            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 10, 10, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 0.Millimeters(),
                ShiftY = 0.Millimeters(),
                WaferAngle = 90.Degrees()
            };
            var waferToStage = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = waferToStage.Convert(initialPosition);

            // Assert
            Assert.AreEqual(-10.0, resultPositionStage.X, Precision, "When rotated of 90°, abcissae should change to -10");
            Assert.AreEqual(10.0, resultPositionStage.Y, Precision, "When rotated of 90°, ordinate should not change");
        }

        [TestMethod]
        public void Expect_correct_ZTop_when_converting_from_stage_to_wafer()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 14, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ZTopFocus = 13.Millimeters()
            };
            var stageToWaferConverter = new StageToWaferConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = stageToWaferConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(1, resultPositionStage.ZTop);
        }

        [TestMethod]
        public void Expect_correct_ZTop_when_converting_from_wafer_to_stage()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 1, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ZTopFocus = 13.Millimeters()
            };
            var waferToStageConverter = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = waferToStageConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(14, resultPositionStage.ZTop);
        }

        [TestMethod]
        public void Expect_correct_translation_rotation_when_converting_wafer_center_from_wafer_to_stage()
        {
            // Arrange
            var waferCenter = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 7.Millimeters(),
                ShiftY = 8.Millimeters(),
                WaferAngle = 90.Degrees()
            };
            var waferToStage = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = waferToStage.Convert(waferCenter);

            // Assert
            Assert.AreEqual(7.0, resultPositionStage.X, Precision, "The wafer center conversion should only be affected by the translation");
            Assert.AreEqual(8.0, resultPositionStage.Y, Precision, "The wafer center conversion should only be affected by the translation");
        }

        [TestMethod]
        public void Expect_correct_translation_rotation_when_converting_position_from_wafer_to_stage()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 10, 10, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 7.Millimeters(),
                ShiftY = 8.Millimeters(),
                WaferAngle = 90.Degrees()
            };
            var waferToStage = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = waferToStage.Convert(initialPosition);

            // Assert
            Assert.AreEqual(-10.0 + 7.0, resultPositionStage.X, Precision);
            Assert.AreEqual(10.0 + 8.0, resultPositionStage.Y, Precision);
        }

        [TestMethod]
        public void Expect_correct_translation_rotation_when_converting_position_from_stage_to_wafer()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 10, 10, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 7.Millimeters(),
                ShiftY = 8.Millimeters(),
                WaferAngle = 90.Degrees()
            };
            var stageToWafer = new StageToWaferConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionStage = stageToWafer.Convert(initialPosition);

            // Assert
            Assert.AreEqual(10.0 - 8.0, resultPositionStage.X, Precision);
            Assert.AreEqual(-(10.0 - 7.0), resultPositionStage.Y, Precision);
        }

        [TestMethod]
        public void Expect_conversion_loop_success_with_shift_angle_and_focus_settings()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 0, 0);
            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 2.Millimeters(),
                ShiftY = 2.Millimeters(),
                WaferAngle = 10.Degrees(),
                ZTopFocus = 1.Millimeters()
            };
            var stageToWaferConverter = new StageToWaferConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);
            var waferToStageConverter = new WaferToStageConverter(SimulatedHardwareManager, CalibManager, waferRefSettings);

            // Act
            var resultPositionWafer = stageToWaferConverter.Convert(initialPosition);
            var resultPositionStage = waferToStageConverter.Convert(resultPositionWafer);

            // Assert
            Assert.AreEqual(initialPosition.X, resultPositionStage.X, Precision);
            Assert.AreEqual(initialPosition.Y, resultPositionStage.Y, Precision);
            Assert.AreEqual(initialPosition.ZTop, resultPositionStage.ZTop);
            Assert.AreEqual(initialPosition.ZBottom, resultPositionStage.ZBottom);
            Assert.AreEqual(initialPosition.Referential, resultPositionStage.Referential);
        }
    }
}
