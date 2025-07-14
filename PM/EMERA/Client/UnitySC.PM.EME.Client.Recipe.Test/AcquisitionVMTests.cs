using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Recipe.Test
{
    [TestClass]
    public class AcquisitionVMTests
    {
        private FilterWheelBench _filterWheelBench;
        private LightBench _lightBench;
        private CameraBench _camera;
        private Mock<ICalibrationService> _calibrationServiceMock;
        private Mock<ILogger> _loggerMock;
        private Mock<IMessenger> _messengerMock;
        private Acquisition _acquisition;

        [TestInitialize]
        public void Setup()
        {
            _calibrationServiceMock = new Mock<ICalibrationService>();

            _loggerMock = new Mock<ILogger>();
            _messengerMock = new Mock<IMessenger>();
            _acquisition = new Acquisition
            {
                Name = "Test Acquisition",
                Filter = EMEFilter.BandPass450nm50,
                LightDeviceId = "ddf_0deg",
                ExposureTime = 1.0
            };

            _filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), new FakeCalibrationSupervisor(), null);

            _camera = new CameraBench(new FakeCameraSupervisor(null), new FakeCalibrationSupervisor(), null);
            _lightBench = new LightBench(new FakeLightsSupervisor(_messengerMock.Object), new FakeKeyboardMouseHook(), null, _messengerMock.Object);
        }
        [TestMethod]
        public void Constructor_InitializesCorrectly()
        {
            // Arrange & Act
            var viewModel = AcquisitionViewModel.Create(
                _camera,
                _filterWheelBench,
                _lightBench,
                _loggerMock.Object,
                _messengerMock.Object,
                "Acquisition1");

            var filters = _filterWheelBench.GetFilters();
            var lights = _lightBench.Lights;
            // Assert
            Assert.IsNotNull(viewModel.Camera);
            Assert.AreEqual(100, viewModel.ExposureTime);
            Assert.AreEqual(filters.FirstOrDefault(), viewModel.CurrentFilter);
            Assert.AreEqual(lights.FirstOrDefault().DeviceID, viewModel.Item.LightDeviceId);
        }
        
        [TestMethod]
        public void Create_ValidParameters_ShouldReturnViewModel()
        {
            // Act
            var viewModel = AcquisitionViewModel.LoadAcquisitionAndCreate(
                _camera,
                _filterWheelBench,
                _lightBench,
                _acquisition,
                _loggerMock.Object,
                _messengerMock.Object);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(_acquisition.Name, viewModel.Name);
            Assert.AreEqual(_acquisition.Filter, viewModel.CurrentFilter.Type);
        }
        
        [TestMethod]
        public void Filter_PropertyChanged_ShouldUpdateItemFilter()
        {
            // Arrange
            var newFilter = new Filter("Filter", EMEFilter.BandPass450nm50, 0);
            var viewModel = AcquisitionViewModel.LoadAcquisitionAndCreate(
                _camera,
                _filterWheelBench,
                _lightBench,
                _acquisition,
                _loggerMock.Object,
                _messengerMock.Object);

            // Act
            viewModel.CurrentFilter = newFilter;

            // Assert
            Assert.AreEqual(newFilter.Type, viewModel.Item.Filter);
        }
        
        [TestMethod]
        public void Create_NoFilters_ShouldLogError()
        {
            // Arrange
            _calibrationServiceMock.Setup(s => s.GetFilters())
                .Returns(() => new Response<List<Filter>> { Result = null });
            _filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), _calibrationServiceMock.Object, null);

            // Act
            var viewModel = AcquisitionViewModel.LoadAcquisitionAndCreate(
                _camera,
                _filterWheelBench,
                _lightBench,
                _acquisition,
                _loggerMock.Object,
                _messengerMock.Object);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(0, viewModel.Filters.Count);
            _loggerMock.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void NonExistingFilter_ShouldBeConsideredAsUnknown()
        {
            // Arrange
            _acquisition = new Acquisition
            {
                Name = "Test Acquisition",
                Filter = EMEFilter.LinearPolarizingFilter, // non-existing filter
                LightDeviceId = "ddf_0deg",
                ExposureTime = 1.0
            };

            // Act
            var viewModel = AcquisitionViewModel.LoadAcquisitionAndCreate(
                _camera,
                _filterWheelBench,
                _lightBench,
                _acquisition,
                _loggerMock.Object,
                _messengerMock.Object);

            // Assert
            Assert.AreEqual(EMEFilter.Unknown, viewModel.CurrentFilter.Type);
        }

        [TestMethod]
        public void Name_PropertyChanged_ShouldUpdateItemName()
        {
            // Arrange            
            var viewModel = AcquisitionViewModel.LoadAcquisitionAndCreate(
                _camera,
                _filterWheelBench,
                _lightBench,
                _acquisition,
                _loggerMock.Object,
                _messengerMock.Object);

            // Act
            viewModel.Name = "New Name";

            // Assert
            Assert.AreEqual("New Name", viewModel.Item.Name);
        }
        
        [TestMethod]
        public void SetCurrentLight_UpdatesItemLight()
        {
            // Arrange
            var viewModel = AcquisitionViewModel.LoadAcquisitionAndCreate(
               _camera,
               _filterWheelBench,
               _lightBench,
               _acquisition,
               _loggerMock.Object,
               _messengerMock.Object);

            // Act
            viewModel.CurrentLight = _lightBench.Lights.Last();

            // Assert
            Assert.AreEqual(_lightBench.Lights.Last(), viewModel.CurrentLight);
            Assert.AreEqual(_lightBench.Lights.Last().DeviceID, viewModel.Item.LightDeviceId);
        }
        
        [TestMethod]
        public void LoadAcquisitionAndCreate_InitializesCorrectly()
        {
            // Arrange
            var acquisition = new Acquisition { LightDeviceId = "4" };

            // Act
            var result = AcquisitionViewModel.LoadAcquisitionAndCreate(
               _camera,
               _filterWheelBench,
               _lightBench,
               acquisition,
               _loggerMock.Object,
               _messengerMock.Object);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_lightBench.Lights.FirstOrDefault(x => x.DeviceID == "4"), result.CurrentLight);
        }
    }
}
