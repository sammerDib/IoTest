using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosCppWrapper;

namespace SharedCpp.WrapperTests
{
    [TestClass]
    public class ProfileStepAnalyserTests
    {
        private double _toleranceHeight = 1e-4;

        [TestMethod]
        public void StepUp()
        {
            var profile = new Profile2d {
                new Point2d( -0.375000, 1028.591412 ),
                new Point2d( -0.360777, 1028.682221 ),
                new Point2d( -0.346115, 1028.704865 ),
                new Point2d( -0.331496, 1028.896400 ),
                new Point2d( -0.316634, 1028.752320 ),
                new Point2d( -0.301886, 1028.917701 ),
                new Point2d( -0.287192, 1028.725088 ),
                new Point2d( -0.273146, 1028.698821 ),
                new Point2d( -0.258299, 1028.820735 ),
                new Point2d( -0.243543, 1028.930594 ),
                new Point2d( -0.228582, 1028.779670 ),
                new Point2d( -0.213821, 1028.865279 ),
                new Point2d( -0.198895, 1028.755972 ),
                new Point2d( -0.184498, 1028.712650 ),
                new Point2d( -0.169478, 1028.744730 ),
                new Point2d( -0.154762, 1028.786705 ),
                new Point2d( -0.139749, 1028.849435 ),
                new Point2d( -0.125104, 1028.819757 ),
                new Point2d( -0.110577, 1028.937791 ),
                new Point2d( -0.095803, 1028.854099 ),
                new Point2d( -0.080842, 1028.802518 ),
                new Point2d( -0.067206, 1028.862277 ),
                new Point2d( -0.051787, 1028.907643 ),
                new Point2d( -0.036692, 1028.959385 ),
                new Point2d( -0.021849, 1028.845103 ),
                new Point2d( -0.007233, 1028.853922 ),
                new Point2d( 0.007200, 1028.934013 ),
                new Point2d( 0.022153, 0.000000 ),
                new Point2d( 0.037047, 1078.215925 ),
                new Point2d( 0.051503, 1078.008572 ),
                new Point2d( 0.066183, 1078.090261 ),
                new Point2d( 0.081070, 1077.989950 ),
                new Point2d( 0.095246, 1077.951405 ),
                new Point2d( 0.110418, 1077.889354 ),
                new Point2d( 0.125285, 1078.012422 ),
                new Point2d( 0.140151, 1078.027844 ),
                new Point2d( 0.154778, 1077.858307 ),
                new Point2d( 0.169315, 1077.752209 ),
                new Point2d( 0.184119, 1077.816359 ),
                new Point2d( 0.199130, 1077.906690 ),
                new Point2d( 0.213726, 1077.950320 ),
                new Point2d( 0.228120, 1077.833952 ),
                new Point2d( 0.243147, 1077.964278 ),
                new Point2d( 0.258386, 1077.984430 ),
                new Point2d( 0.272557, 1077.967430 ),
                new Point2d( 0.287243, 1077.935310 ),
                new Point2d( 0.302026, 1077.988674 ),
                new Point2d( 0.316892, 1078.029924 ),
                new Point2d( 0.331457, 1078.025272 ),
                new Point2d( 0.345953, 1077.904030 ) 
            };
            var parameters = new ProfileStepAnalyserParameters(ProfileStepAnalyserParameters.KindStep.Up, 50, 1);

            var result = new ProfileStepAnalyser(parameters).Process(profile) as ProfileStepAnalyserResult;

            var toleranceStepX = 0.15 / 2.0; // Half the space between two points

            Assert.AreEqual(ProfileAnalyserResult.Status.Ok, result.GetStatus());
            Assert.AreEqual(49.1447469592094, result.GetStepHeight(), _toleranceHeight);
            Assert.AreEqual(0.02212, result.GetStepX(), toleranceStepX);
        }

        [TestMethod]
        public void EmptyProfile()
        {
            var profile = new Profile2d();
            var parameters = new ProfileStepAnalyserParameters(ProfileStepAnalyserParameters.KindStep.Up, 50, 1);
            var result = new ProfileStepAnalyser(parameters).Process(profile) as ProfileStepAnalyserResult;
            Assert.AreEqual(ProfileAnalyserResult.Status.EmptyProfile, result.GetStatus());
        }


        [TestMethod]
        public void EmptyNaN()
        {
            var profile = new Profile2d {
                new Point2d( -0.5, double.NaN ),
                new Point2d( 0.0, double.NaN ),
                new Point2d( 0.5, double.NaN ),
            };
            var parameters = new ProfileStepAnalyserParameters(ProfileStepAnalyserParameters.KindStep.Up, 50, 1);
            var result = new ProfileStepAnalyser(parameters).Process(profile) as ProfileStepAnalyserResult;
            Assert.AreEqual(ProfileAnalyserResult.Status.EmptyProfileNan, result.GetStatus());
        }

        [TestMethod]
        public void TooSmallStdDev()
        {
            var profile = new Profile2d {
                new Point2d( -0.5, 100.0 ),
                new Point2d( 0.0, 101.0 ),
                new Point2d( 0.5, 90.0 ),
            };
            var parameters = new ProfileStepAnalyserParameters(ProfileStepAnalyserParameters.KindStep.Up, 50, 1);
            parameters.SetNbStdDevFiltering(0.1);
            var result = new ProfileStepAnalyser(parameters).Process(profile) as ProfileStepAnalyserResult;
            Assert.AreEqual(ProfileAnalyserResult.Status.ProfileTooSmallAfterStdDevFiltering, result.GetStatus());
        }
    }
}
