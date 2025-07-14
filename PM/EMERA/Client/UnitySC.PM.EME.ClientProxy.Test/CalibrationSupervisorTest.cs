using System;
using System.Linq;
using System.Xml.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Proxy.Calibration;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Proxy.Test
{
    [TestClass]
    public class CalibrationSupervisorTest
    {
        private CalibrationSupervisor _calibrationSupervisor;

        [TestInitialize]
        public void Initialize()
        {
            _calibrationSupervisor = new CalibrationSupervisor(new SerilogLogger<ICalibrationService>(), null);
        }

        [TestMethod]
        public void ExtractedTestApplicationCalibrationData_ShouldGiveANonNullPixelSize()
        {
            // When 
            var testApplicationCalibration = _calibrationSupervisor.GetCameraCalibrationData().Result;

            // Then 
            Assert.AreEqual(2.02.Micrometers(), testApplicationCalibration.PixelSize);
        }

        [TestMethod]
        public void TestCalibrationsRetrieval()
        {
            // When 
            var calibrations = _calibrationSupervisor.GetCalibrations().Result;

            var xmlIncludes =
                (XmlIncludeAttribute[])Attribute.GetCustomAttributes(typeof(ICalibrationData),
                    typeof(XmlIncludeAttribute));
            var types = xmlIncludes.Select(x => x.Type).ToList();

            // Then 
            //Each type of calibration that inherits from ICalibrationData must have an associated file.
            Assert.AreEqual(6, calibrations.Count());
        }

        [TestMethod]
        public void ExtractedPixelSizeFromTestApplicationCalibrationData()
        {
            // When 
            var testApplicationCalibration = _calibrationSupervisor.GetCalibrations().Result
                .OfType<CameraCalibrationData>().FirstOrDefault();

            // Then             
            Assert.AreEqual(2.02.Micrometers(), testApplicationCalibration.PixelSize);
        }

        [TestMethod]
        public void GetWaferReferential_ShouldProduceWaferReferentialSettings()
        {
            // When             
            var waferReferentialSettings = _calibrationSupervisor.GetWaferReferentialSettings(200.Millimeters()).Result;

            // Then             
            Assert.IsNotNull(waferReferentialSettings);
        }

        [TestMethod]
        public void Gel_ShouldProduceWaferReferentialSettings()
        {
            var testApplicationCalibration = _calibrationSupervisor.GetCalibrations().Result
                .OfType<CameraCalibrationData>().FirstOrDefault();
            testApplicationCalibration.PixelSize = 3.2.Micrometers();
            // When             
            _calibrationSupervisor.SaveCalibration(testApplicationCalibration);
            testApplicationCalibration = _calibrationSupervisor.GetCalibrations().Result.OfType<CameraCalibrationData>()
                .FirstOrDefault();
            // Then             
            Assert.AreEqual(3.2.Micrometers(), testApplicationCalibration.PixelSize);

            // When   
            testApplicationCalibration.PixelSize = 2.02.Micrometers();
            _calibrationSupervisor.SaveCalibration(testApplicationCalibration);
            testApplicationCalibration = _calibrationSupervisor.GetCalibrations().Result.OfType<CameraCalibrationData>()
                .FirstOrDefault();
            // Then             
            Assert.AreEqual(2.02.Micrometers(), testApplicationCalibration.PixelSize);
        }
    }
}
