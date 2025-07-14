using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Test.Calibration
{
    [TestClass]
    public class CalibrationManagerTest
    {
        private CalibrationManager _calibrationManager;

        [TestInitialize]
        public void Init()
        {
            // Register
            var container = new Container();

            Bootstrapper.Register(container);
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            // Make sure calibration manager is empty
            Assert.AreEqual(0, _calibrationManager.Calibrations.Count());
        }

        [TestMethod]
        public void No_user_in_calibration_throws()
        {
            // Given invalid calibration with no user
            var calibration = new ObjectivesCalibrationData()
            {
                User = null
            };

            // When updating the calibration
            // Then it throws an InvalidOperationException
            Assert.ThrowsException<InvalidOperationException>(() => _calibrationManager.UpdateCalibration(calibration));
        }

        [TestMethod]
        public void One_valid_calibration_is_added()
        {
            // Given valid calibration
            string user = "TestUser" + DateTime.Now;
            var calibration = new ObjectivesCalibrationData()
            {
                User = user
            };

            // When updating the calibration
            _calibrationManager.UpdateCalibration(calibration);

            // Then the calibration is properly added
            Assert.AreEqual(1, _calibrationManager.Calibrations.Count());
            Assert.AreEqual(user, _calibrationManager.Calibrations.First().User);
        }

        [TestMethod]
        public void Valid_calibration_of_same_type_replaces_previous_calibration()
        {
            // Given a calibration manager with an already existing calibration of type ObjectivesCalibrationData
            var calibration = new ObjectivesCalibrationData()
            {
                User = "TestUser" + DateTime.Now
            };
            _calibrationManager.UpdateCalibration(calibration);

            // Given a new calibration of type ObjectivesCalibrationData
            string newUser = "NewUser" + DateTime.Now;
            var newCalibration = new ObjectivesCalibrationData()
            {
                User = newUser
            };

            // When updating the calibration
            _calibrationManager.UpdateCalibration(newCalibration);

            // Then the calibration of type ObjectivesCalibrationData has been replaced by the new one
            Assert.AreEqual(1, _calibrationManager.Calibrations.Count());
            Assert.AreEqual(newUser, _calibrationManager.Calibrations.First().User);
        }

        [TestMethod]
        public void Valid_calibration_of_other_type_is_added_alongside_the_existing_calibration()
        {
            // Given a calibration manager with an already existing calibration of type ObjectivesCalibrationData
            string objectivesUser = "ObjectivesUser" + DateTime.Now;
            var objectivesCalibration = new ObjectivesCalibrationData()
            {
                User = objectivesUser
            };
            _calibrationManager.UpdateCalibration(objectivesCalibration);

            // Given a new calibration of type XYCalibrationData
            string xyUser = "XYUser" + DateTime.Now;
            var xyCalibration = new XYCalibrationData()
            {
                User = xyUser
            };

            // When updating the calibration
            _calibrationManager.UpdateCalibration(xyCalibration);

            // Then the calibration of type XYCalibrationData is added alongside the one of type ObjectivesCalibrationData
            Assert.AreEqual(2, _calibrationManager.Calibrations.Count());
            Assert.AreEqual(objectivesUser, _calibrationManager.Calibrations.OfType<ObjectivesCalibrationData>().First().User);
            Assert.AreEqual(xyUser, _calibrationManager.Calibrations.OfType<XYCalibrationData>().First().User);
        }
    }
}
