using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosCppWrapper;

namespace SharedCpp.WrapperTests
{
    [TestClass]
    public class ProfileTrenchAnalyserTests
    {
        private double _tolerance = 1e-3;

        [TestMethod]
        public void TrenchNominalCase()
        {
            var profile = new Profile2d {
                new Point2d( 0.0, 50.0 ),
                new Point2d( 1.0, 50.0 ),
                new Point2d( 2.0, 50.0 ),
                new Point2d( 3.0, 10.0 ),
                new Point2d( 4.0, 10.0 ),
                new Point2d( 5.0, 10.0 ),
                new Point2d( 6.0, 10.0 ),
                new Point2d( 7.0, 50.0 ),
                new Point2d( 8.0, 50.0 ),
                new Point2d( 9.0, 50.0 ),
            };
            var parameters = new ProfileTrenchAnalyserParameters(40, 1, 3.0, _tolerance);
            parameters.AddTrenchDownExclusionZone(new ExclusionZone(0.5, 0.5));
            parameters.AddTrenchUpExclusionZone(new ExclusionZone(0.5, 0.5));

            var result = new ProfileTrenchAnalyser(parameters).Process(profile) as ProfileTrenchAnalyserResult;

            Assert.AreEqual(ProfileAnalyserResult.Status.Ok, result.GetStatus());
            Assert.AreEqual(40.0, result.GetDepth(), _tolerance);
            Assert.AreEqual(3.0, result.GetWidth(), _tolerance);
        }
    }
}
