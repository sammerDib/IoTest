using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Implementation.Test
{
    [TestClass]
    public class CalibrationServiceTest
    {
        [TestMethod]
        public void DummyTestApplicationCalibration_ShouldGiveCorrectPixelSize()
        {
            // Given
            var configurationManager = new FakeConfigurationManager(AppDomain.CurrentDomain.BaseDirectory, false);
            var calibrationService = new CalibrationService(null, configurationManager, null);

            // When
            var calibration = calibrationService.GetCameraCalibrationData().Result;

            // Then
            Assert.AreEqual(2.02.Micrometers(), calibration.PixelSize);
        }

        [TestMethod]
        public void ThrowException_IfFileDoesNotExist()
        {
            // Given
            var configurationManager = new FakeConfigurationManager("doesNotExist/", null, true);
            var calibrationService = new CalibrationService(null, configurationManager, null);

            // When
            var response = calibrationService.GetCameraCalibrationData();

            // Then
            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Exception);
        }
    }
}
