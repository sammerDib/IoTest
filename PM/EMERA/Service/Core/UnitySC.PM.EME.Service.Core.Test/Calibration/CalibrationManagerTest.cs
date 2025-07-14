using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Calibration
{
    [TestClass]
    public class CalibrationManagerTest
    {
        private CalibrationManager _calibrationManager;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            // Make sure to allow registration overriding for SpecializeRegister
            container.Options.AllowOverridingRegistrations = true;
            Bootstrapper.Register(container, true);

            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            // Make sure calibration manager is not empty
            Assert.IsNotNull(_calibrationManager.Calibrations);
            Assert.IsTrue(_calibrationManager.Calibrations.Any());             
        }
        [TestMethod]
        public void GetCalibrationTypes_ReturnsCorrectTypes()
        {
            // Act
            var calibrationTypes = _calibrationManager.GetCalibrationTypes();
            // Assert : Check that there is at least one type of calibration
            Assert.IsNotNull(calibrationTypes);
            Assert.IsTrue(calibrationTypes.Any());
        }
        [TestMethod]
        public void GetWaferConfigurationForDiameter_ReturnsCorrectConfiguration()
        {
            // Arrange
            var waferDiameter = new Length(200, LengthUnit.Millimeter); // Remplacez par le diamètre souhaité
            // Act
            var result = _calibrationManager.GetWaferReferentialSettings(waferDiameter);
            // Assert            
            Assert.IsNotNull(result);            
        }

        [TestMethod]
        public void GetFilters_ReturnsCorrectFilters()
        {
            var result = _calibrationManager.GetFilters();       
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void GetDistortion()
        {
            var result = _calibrationManager.GetDistortion();
            Assert.IsTrue(result.CameraMat.Any());
        }
    }    
}
