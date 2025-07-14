using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class ProfileStepAnalyserResultToStepPointDataTests
    {
        [DataTestMethod]
        [DataRow(ProfileAnalyserResult.Status.Ok, 149.5, MeasureState.Success)]
        [DataRow(ProfileAnalyserResult.Status.Ok, 50.0, MeasureState.Error)]
        [DataRow(ProfileAnalyserResult.Status.EmptyProfile, 0.0, MeasureState.NotMeasured)]
        [DataRow(ProfileAnalyserResult.Status.EmptyProfileNan, 0.0, MeasureState.NotMeasured)]
        [DataRow(ProfileAnalyserResult.Status.ProfileTooSmallAfterStdDevFiltering, 0.0, MeasureState.NotMeasured)]
        public void Convert(ProfileAnalyserResult.Status status, double height, MeasureState expectedState)
        {
            var settings = new StepSettings
            {
                ScanSize = new Length(1.0, LengthUnit.Millimeter),
                ToleranceHeight = new LengthTolerance(1.0, LengthToleranceUnit.Micrometer),
                TargetHeight = 150.0.Micrometers(),
            };
            var profile = new Profile2d();
            var result = new ProfileStepAnalyserResult(status, 0.0, height);

            var stepPointData = new ProfileStepAnalyserResultToStepPointData(settings).Convert(profile, result);

            Assert.AreEqual(height, stepPointData.StepHeight.Micrometers);
            Assert.AreEqual(expectedState, stepPointData.State);
        }
    }
}
