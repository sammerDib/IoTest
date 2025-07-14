using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Referential
{
    [TestClass]
    public class StageMotorConverterTest
    {
        protected AnaHardwareManager SimulatedHardwareManager;
        protected CalibrationManager CalibManager;

        protected const string ObjectiveUpId = "objectiveUpId";

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            ClassLocator.ExternalInit(container, true);

            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => new FakeConfigurationManager(), true);
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);
            ClassLocator.Default.Register(() => new FDCManager("test", "test"), true);
            ClassLocator.Default.Register<CalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath));

            ClassLocator.Default.Register<IHardwareLoggerFactory>(() => new Mock<IHardwareLoggerFactory>().Object);

            ClassLocator.Default.Register<AnaHardwareManager>(()=>
            {
                var mockManager = new Mock<AnaHardwareManager>(new SerilogLogger<AnaHardwareManager>(), ClassLocator.Default.GetInstance<IHardwareLoggerFactory>(), new FakeConfigurationManager());
                mockManager.Setup(_ => _.GetObjectiveInUseByPosition(ModulePositions.Up))
                               .Returns(new Interface.ObjectiveConfig { DeviceID = ObjectiveUpId })
                               .Verifiable();
                return mockManager.Object;
            });
            SimulatedHardwareManager =ClassLocator.Default.GetInstance<AnaHardwareManager>();
            CalibManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        }

        [TestMethod]
        public void Expect_correct_shift_when_converting_from_stage_to_motor()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 10, 10, 0, 0);

            CalibManager.UpdateCalibration(new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>()
                {
                    new ObjectiveCalibration()
                    {
                        DeviceId = ObjectiveUpId,
                        Image = new ImageParameters()
                        {
                            XOffset = 10.Millimeters(),
                            YOffset = 5.Millimeters(),
                        }
                    }
                }
            });

            CalibManager.UpdateCalibration(new XYCalibrationData()
            {
                User = "Default",
                Corrections = new List<Correction>()
                {
                    new Correction()
                    {
                        XTheoricalPosition = 10.Millimeters(),
                        YTheoricalPosition = 10.Millimeters(),
                        ShiftX = 2.Millimeters(),
                        ShiftY = 1.Millimeters(),
                    },
                    // This is the correction that should be applied to the objective offseted point
                    new Correction()
                    {
                        XTheoricalPosition = 0.Millimeters(), // Objective offsetted X
                        YTheoricalPosition = 5.Millimeters(), // Objective offsetted Y
                        ShiftX = 7.Millimeters(),
                        ShiftY = 8.Millimeters(),
                    }
                },
                WaferCalibrationDiameter = 300.Millimeters(),
                ShiftX = 0.Millimeters(),
                ShiftY = 0.Millimeters(),
                ShiftAngle = 0.Degrees()
            });

            var data = CalibManager.GetXYCalibrationData();
            data.WaferCalibrationDiameter = 300.Millimeters();
            data.ShiftX = 0.Millimeters();
            data.ShiftY = 0.Millimeters();
            data.ShiftAngle = 0.Degrees();
            Assert.IsTrue(XYCalibrationCalcul.PreCompute(data, UnitySCSharedAlgosCppWrapper.InterpolateAlgoType.QuadNN));

            var stageToMotorConverter = new StageToMotorConverter(SimulatedHardwareManager, CalibManager);

            // Act
            var resultPositionMotor = stageToMotorConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(10 + 10 + 2, resultPositionMotor.X);
            Assert.AreEqual(10 + 5 + 1, resultPositionMotor.Y);
        }

        [TestMethod]
        public void Expect_correct_shift_when_converting_from_motor_to_stage()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new MotorReferential(), 10, 10, 0, 0);

            CalibManager.UpdateCalibration(new XYCalibrationData()
            {
                User = "Default",
                Corrections = new List<Correction>()
                {
                    // This is the correction that should be applied before the objective offset
                    new Correction()
                    {
                        XTheoricalPosition = 10.Millimeters(),
                        YTheoricalPosition = 10.Millimeters(),
                        ShiftX = 7.Millimeters(),
                        ShiftY = 8.Millimeters(),
                    },
                    new Correction()
                    {
                        XTheoricalPosition = 0.Millimeters(),
                        YTheoricalPosition = 5.Millimeters(),
                        ShiftX = 2.Millimeters(),
                        ShiftY = 1.Millimeters(),
                    },
                    new Correction()
                    {
                        XTheoricalPosition = 20.Millimeters(),
                        YTheoricalPosition = 15.Millimeters(),
                        ShiftX = 5.Millimeters(),
                        ShiftY = 3.Millimeters(),
                    }
                },
                WaferCalibrationDiameter = 300.Millimeters(),
                ShiftX = 0.Millimeters(),
                ShiftY = 0.Millimeters(),
                ShiftAngle = 0.Degrees()
            });

            CalibManager.UpdateCalibration(new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>()
                {
                    new ObjectiveCalibration()
                    {
                        DeviceId = ObjectiveUpId,
                        Image = new ImageParameters()
                        {
                            XOffset = 10.Millimeters(),
                            YOffset = 5.Millimeters(),
                        }
                    }
                }
            });

            var motorToStageConverter = new MotorToStageConverter(SimulatedHardwareManager, CalibManager);

            // Act
            var resultPositionStage = motorToStageConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(10 - 7 - 10, resultPositionStage.X);
            Assert.AreEqual(10 - 8 - 5, resultPositionStage.Y);
        }
    }
}
