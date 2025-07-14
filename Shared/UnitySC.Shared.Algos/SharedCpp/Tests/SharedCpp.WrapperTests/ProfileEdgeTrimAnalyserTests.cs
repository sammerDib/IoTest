using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosCppWrapper;

namespace SharedCpp.WrapperTests
{
    [TestClass]
    public class ProfileEdgeTrimAnalyserTests
    {
        private double _tolerance = 1e-3;

        [TestMethod]
        public void ResultConstruction()
        {
            var status = ProfileAnalyserResult.Status.Ok;
            double stepX = 2.0;
            double height = 15.0;
            double width = 30.0;
            var result = new ProfileEdgeTrimAnalyserResult(status, stepX, height, width);

            Assert.AreEqual(status, result.GetStatus());
            Assert.AreEqual(stepX, result.GetStepX());
            Assert.AreEqual(height, result.GetStepHeight());
            Assert.AreEqual(width, result.GetWidth());
        }

        [TestMethod]
        public void EdgeTrimDownBasic()
        {
            var profile = new Profile2d {
                new Point2d( 0.0, 50.0 ),
                new Point2d( 1.0, 50.0 ),
                new Point2d( 2.0, 50.0 ),
                new Point2d( 3.0, 10.0 ),
                new Point2d( 4.0, 10.0 ),
                new Point2d( 5.0, 10.0 ),
                new Point2d( 6.0, 10.0 ),
            };
            var parameters = new ProfileEdgeTrimAnalyserParameters(ProfileEdgeTrimAnalyserParameters.KindStep.Down, 40, 1, 3.0, _tolerance);
            parameters.AddStepExclusionZone(new ExclusionZone(0.5, 0.5));

            var result = new ProfileEdgeTrimAnalyser(parameters).Process(profile) as ProfileEdgeTrimAnalyserResult;

            Assert.AreEqual(ProfileAnalyserResult.Status.Ok, result.GetStatus());
            Assert.AreEqual(40.0, result.GetStepHeight(), _tolerance);
            Assert.AreEqual(3.0, result.GetWidth(), _tolerance);
        }
    }
}
