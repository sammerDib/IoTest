using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Recipe
{
    // WARNING: this test is not usable as it is. Other initialization setups
    public class ANARecipeExecutionTest : FunctionalTest
    {
        public override void Run()
        {
            var recipe = new ANARecipe();

            recipe.Execution.Alignment.RunAutoFocus = true;
            recipe.Alignment.AutoFocusLise =
                new AutoFocusLiseParameters
                {
                    ZIsDefinedByUser = false,
                    ZTopFocus = 13.3.Millimeters(), // expected
                    LiseParametersAreDefinedByUser = false,
                    LiseGain = 1.8,
                    ZMin = 12.Millimeters(),
                    ZMax = 15.Millimeters(),
                };

            recipe.Execution.Alignment.RunAutoLight = true;
            recipe.Alignment.AutoLight = new AutoLightParameters
            {
                LightIntensityIsDefinedByUser = false,
                LightIntensity = 99, // expected between 4 and 7
                Exposure = 30,
                MinLightPower = 1,
                MaxLightPower = 100,
                LightPowerStep = 0.1
            };

            recipe.Execution.Alignment.RunBwa = true;
            recipe.Alignment.BareWaferAlignment = new BareWaferAlignmentParameters { CustomImagePositions = null, };

            recipe.Points = new List<MeasurePoint>
            {
                new MeasurePoint
                {
                    PatternRec = null,
                    Id = 0,
                    Position = new PointPosition { X = 2.776, Y = -138.323, ZTop = 0.780, ZBottom = 0 }
                },
                new MeasurePoint
                {
                    PatternRec = null,
                    Id = 1,
                    Position = new PointPosition { X = 2.822, Y = -138.322, ZTop = 0.780, ZBottom = 0 }
                }
            };

            recipe.Measures = new List<MeasureSettingsBase>
            {
                new TSVSettings
                {
                    Name = "mon tsv",
                    IsActive = true,
                    MeasurePoints = new List<int> { 0, 1 },
                    NbOfRepeat = 1,
                    Strategy = Service.Interface.Algo.TSVAcquisitionStrategy.Standard,
                    Precision = Service.Interface.Algo.TSVMeasurePrecision.Fast,
                    DepthTarget = 50.Micrometers(),
                    DepthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                    LengthTarget = 5.Micrometers(),
                    LengthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer),
                    WidthTarget = 6.Micrometers(),
                    WidthTolerance = new LengthTolerance(2, LengthToleranceUnit.Micrometer),
                    CameraId = "1",
                    Probe=new SingleLiseSettings(){ProbeId = "ProbeLiseUp"},
                    EllipseDetectionTolerance = 1.Micrometers(),
                    Shape = UnitySC.Shared.Format.Metro.TSV.TSVShape.Elipse,
                    ROI = null //new CenteredRegionOfInterest { Width = null, Height = null, OffsetX = null, OffsetY = null }
                }
            };

            var waferCharacteristic = new WaferDimensionalCharacteristic
            {
                WaferShape = WaferShape.Notch,
                Diameter = 300.Millimeters(),
                Category = "1.15",
                DiameterTolerance = null,
                Flats = null,
                Notch = new NotchDimentionalCharacteristic
                {
                    Depth = 1.Millimeters(),
                    Angle = 0.Degrees(),
                    DepthPositiveTolerance = 0.25.Millimeters(),
                    AngleNegativeTolerance = 1.Degrees(),
                    AnglePositiveTolerance = 5.Degrees()
                },
                SampleWidth = null,
                SampleHeight = null
            };

            // Add waferCharacteristic in recipe.Step.Product.WaferCategory
            recipe.Step = new Step();
            recipe.Step.Product = new Product();
            recipe.Step.Product.WaferCategory = new WaferCategory();
            recipe.Step.Product.WaferCategory.DimentionalCharacteristic = waferCharacteristic;

            var referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
            var pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            var executor = new ANARecipeExecutionManager(Logger, HardwareManager, referentialManager, pmConfiguration);
            ClassLocator.Default.GetInstance<CameraServiceEx>().Init();
            //HardwareManager.Init();
            // var objectiveSelector1 = HardwareManager.ObjectivesSelectors["ObjectiveSelector01"];
            // var x50Config = objectiveSelector1.Config.Objectives[1];
            // objectiveSelector1.SetObjective(x50Config);

            executor.Execute(recipe);
        }
    }
}
