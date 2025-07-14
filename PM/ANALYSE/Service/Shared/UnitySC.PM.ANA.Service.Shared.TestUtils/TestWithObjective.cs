using Moq;
using System.Collections.Generic;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public interface ITestWithObjective
    {
        string ObjectiveUpId { get; set; }
        string ObjectiveBottomId { get; set; }

        Length PixelSizeX { get; set; }
        Length PixelSizeY { get; set; }

        AnaHardwareManager HardwareManager { get; set; }
        CalibrationManager CalibManager { get; set; }
    }

    public static class TestWithObjectiveHelper
    {
        public static readonly string objectiveUp20XId = "objectiveUp20XId";

        public static void Setup(ITestWithObjective test)
        {
            test.ObjectiveUpId = "objectiveUpId";
            test.ObjectiveBottomId = "objectiveBottomId";

            if (test.PixelSizeX is null)
            {
                test.PixelSizeX = 2.Micrometers();
            }
            if (test.PixelSizeY is null)
            {
                test.PixelSizeY = 2.Micrometers();
            }

            var simulatedObjectiveUpSelector = new Mock<IObjectiveSelector>();
            var objectiveUpConfig = new ObjectiveConfig()
            {
                DepthOfField = 0.01.Millimeters(),
                DeviceID = test.ObjectiveUpId
            };
            var objectiveUp20XConfig = new ObjectiveConfig()
            {
                DepthOfField = 0.001.Millimeters(),
                DeviceID = objectiveUp20XId
            };
            var objectiveUpSelectorConfig = new SingleObjectiveSelectorConfig()
            {
                Objectives = new List<ObjectiveConfig>() { objectiveUpConfig, objectiveUp20XConfig }
            };
            simulatedObjectiveUpSelector.Setup(_ => _.GetObjectiveInUse()).Returns(objectiveUpConfig);
            simulatedObjectiveUpSelector.SetupGet(_ => _.Config).Returns(objectiveUpSelectorConfig);
            simulatedObjectiveUpSelector.SetupGet(_ => _.Position).Returns(ModulePositions.Up);
            simulatedObjectiveUpSelector.Setup(selector => selector.DeviceID).Returns(test.ObjectiveUpId);

            test.HardwareManager.ObjectivesSelectors[test.ObjectiveUpId] = simulatedObjectiveUpSelector.Object;

            var simulatedObjectiveBottomSelector = new Mock<IObjectiveSelector>();
            var objectiveBottomConfig = new ObjectiveConfig()
            {
                DepthOfField = 0.01.Millimeters(),
                DeviceID = test.ObjectiveBottomId
            };
            var objectiveDownSelectorConfig = new SingleObjectiveSelectorConfig()
            {
                Objectives = new List<ObjectiveConfig>() { objectiveBottomConfig },
            };
            simulatedObjectiveBottomSelector.Setup(_ => _.GetObjectiveInUse()).Returns(objectiveBottomConfig);
            simulatedObjectiveBottomSelector.SetupGet(_ => _.Config).Returns(objectiveDownSelectorConfig);
            simulatedObjectiveBottomSelector.SetupGet(_ => _.Position).Returns(ModulePositions.Down);
            simulatedObjectiveBottomSelector.Setup(selector => selector.DeviceID).Returns(test.ObjectiveBottomId);

            test.HardwareManager.ObjectivesSelectors[test.ObjectiveBottomId] = simulatedObjectiveBottomSelector.Object;

            SetupDefaultObjectiveCalibration(test);
        }

        private static void SetupDefaultObjectiveCalibration(ITestWithObjective test)
        {
            var imageParams = new ImageParameters()
            {
                PixelSizeX = test.PixelSizeX,
                PixelSizeY = test.PixelSizeY,
            };

            var autofocusUpParams = new AutofocusParameters()
            {
                ZFocusPosition = 12.Millimeters(),
                Lise = new LiseAutofocusParameters()
                {
                    AirGap = 12.Millimeters(),
                    ZStartPosition = 12.Millimeters(),
                    MinGain = 1.4,
                    MaxGain = 1.5
                }
            };

            var autofocusBottomParams = new AutofocusParameters()
            {
                ZFocusPosition = -2.Millimeters(),               
                Lise = new LiseAutofocusParameters()
                {
                    AirGap = -2.Millimeters(),
                    ZStartPosition = -2.Millimeters(),
                    MinGain = 1.4,
                    MaxGain = 1.5
                }
            };

            ObjectiveCalibration objectiveUpCalibration = new ObjectiveCalibration()
            {
                DeviceId = test.ObjectiveUpId,
                AutoFocus = autofocusUpParams,
                Image = imageParams,
                ZOffsetWithMainObjective = 0.Millimeters(),
                OpticalReferenceElevationFromStandardWafer = 0.Micrometers(),
            };
            ObjectiveCalibration objectiveBottomCalibration = new ObjectiveCalibration()
            {
                DeviceId = test.ObjectiveBottomId,
                AutoFocus = autofocusBottomParams,
                Image = imageParams,
                ZOffsetWithMainObjective = 0.Millimeters(),
            };
            var objectiveCalibrationData = new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>() { objectiveUpCalibration, objectiveBottomCalibration },

            };

            test.CalibManager.UpdateCalibration(objectiveCalibrationData);
        }
    }
}
