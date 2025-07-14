using System;

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
    public class ProfileTrenchAnalyserResultToTrenchPointDataTest
    {
        [DataTestMethod]
        [DataRow(ProfileAnalyserResult.Status.Ok, 50.0, 1.0, MeasureState.Error, false)]
        [DataRow(ProfileAnalyserResult.Status.Ok, 149.5, 1.0, MeasureState.Success, false)]
        [DataRow(ProfileAnalyserResult.Status.Ok, 50.0, 1.5, MeasureState.Error, false)]
        [DataRow(ProfileAnalyserResult.Status.Ok, 149.5, 1.5, MeasureState.Success, false)]

        [DataRow(ProfileAnalyserResult.Status.Ok, 50.0, 1.0, MeasureState.Error, true)]
        [DataRow(ProfileAnalyserResult.Status.Ok, 149.5, 1.0, MeasureState.Error, true)]
        [DataRow(ProfileAnalyserResult.Status.Ok, 50.0, 1.5, MeasureState.Error, true)]
        [DataRow(ProfileAnalyserResult.Status.Ok, 149.5, 1.5, MeasureState.Success, true)]
        [DataRow(ProfileAnalyserResult.Status.EmptyProfile, 0.0, 0.0, MeasureState.NotMeasured, true)]
        [DataRow(ProfileAnalyserResult.Status.EmptyProfileNan, 0.0, 0.0, MeasureState.NotMeasured, true)]
        [DataRow(ProfileAnalyserResult.Status.ProfileTooSmallAfterStdDevFiltering, 0.0, 0.0, MeasureState.NotMeasured, true)]
        public void Convert(ProfileAnalyserResult.Status status, double depth, double width, MeasureState expectedState, bool widthNeeded)
        {
            var settings = new TrenchSettings
            {
                ScanSize = new Length(1.0, LengthUnit.Millimeter),
                DepthTolerance = new LengthTolerance(1.0, LengthToleranceUnit.Micrometer),
                DepthTarget = 150.0.Micrometers(),
                WidthTolerance = new LengthTolerance(0.1, LengthToleranceUnit.Millimeter),
                WidthTarget = 1.5.Millimeters(),
                IsWidthMeasured = widthNeeded,
            };
            var profile = new Profile2d();
            var result = new ProfileTrenchAnalyserResult(status, depth, width);

            var TrenchPointData = new ProfileTrenchAnalyserResultToTrenchPointData(settings).Convert(profile, result);

            Assert.AreEqual(depth, TrenchPointData.Depth.Micrometers);
            Assert.AreEqual(width, TrenchPointData.Width.Millimeters);
            Assert.AreEqual(expectedState, TrenchPointData.State);
        }

        [DataTestMethod]
        [DataRow(150.0, 1.5, 0.0, 1.0, 0.0, 1.0)]
        [DataRow(150.0, 1.0, 0.0, 1.0, 0.5, 1.0)]
        [DataRow(150.0, 0.75, 0.0, 1.0, 0.0, 2.0)]
        [DataRow(150.0, 0.5, 0.0, 1.0, 0.5, 2.0)]
        [DataRow(100.0, 1.5, 50.0, 1.0, 0.0, 1.0)]
        [DataRow(75.0, 1.5, 0.0, 2.0, 0.0, 1.0)]
        [DataRow(50.0, 1.5, 50.0, 2.0, 0.0, 1.0)]
        public void ConvertWithCorrection(double depth, double width,
            double correctionDepthOffset, double correctionDepthCoef,
            double correctionWidthOffset, double correctionWidthCoef)
        {
            var settings = new TrenchSettings
            {
                ScanSize = new Length(1.0, LengthUnit.Millimeter),

                DepthTolerance = new LengthTolerance(1.0, LengthToleranceUnit.Micrometer),
                DepthTarget = 150.0.Micrometers(),
                DepthCorrection = new ResultCorrectionSettings
                {
                    Coef = correctionDepthCoef,
                    Offset = new Length(correctionDepthOffset, LengthUnit.Micrometer)
                },

                WidthTolerance = new LengthTolerance(0.1, LengthToleranceUnit.Millimeter),
                WidthTarget = 1.5.Millimeters(),
                WidthCorrection = new ResultCorrectionSettings
                {
                    Coef = correctionWidthCoef,
                    Offset = new Length(correctionWidthOffset, LengthUnit.Millimeter)
                },
                IsWidthMeasured = true,
            };
            var profile = new Profile2d();
            var result = new ProfileTrenchAnalyserResult(ProfileAnalyserResult.Status.Ok, depth, width);

            var TrenchPointData = new ProfileTrenchAnalyserResultToTrenchPointData(settings).Convert(profile, result);

            Assert.AreEqual(depth * correctionDepthCoef + correctionDepthOffset, TrenchPointData.Depth.Micrometers);
            Assert.AreEqual(width * correctionWidthCoef + correctionWidthOffset, TrenchPointData.Width.Millimeters);
        }
    }
}
