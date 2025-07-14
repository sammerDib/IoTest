using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.EME.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.EME.Client.TestUtils.Dispatcher;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.Test
{
    [TestClass]
    public class FilterCalibrationVMTests
    { 
        [TestInitialize]
        public void Setup()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register<INavigationManager, NavigationManagerForCalibration>(true);
            Application app = Application.Current ?? new Application();
        }

        [TestMethod]
        public void StartFilterCalibration_ShouldReturnFilters()
        {                    
            // Given 
            var fakeAlgoSupervisor = new FakeAlgoSupervisor();
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var fakeCalibrationSupervisor = new FakeCalibrationSupervisor();
            var mockDialogOwnerService = new Mock<IDialogOwnerService>();
            mockDialogOwnerService.Setup(d => d.ShowMessageBox(It.IsAny<string>(),
                                                    It.IsAny<string>(),
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Question,
                                                    MessageBoxResult.No))
                                                    .Returns(MessageBoxResult.Yes);

            var filterBench = new FilterWheelBench(fakeFilterWheelSupervisor, fakeCalibrationSupervisor, null);

            var dummyLength = new Length(66, LengthUnit.Millimeter);
            var dummyFilter =
                new Filter("Filter", EMEFilter.BandPass450nm50, 42, dummyLength, dummyLength, dummyLength);
            var dummyCalibrationResult = new List<Filter> { dummyFilter };
            fakeAlgoSupervisor.ExpectedFilterCalibrationResult = dummyCalibrationResult;

            var filterViewModel =
                new FilterCalibrationVM(filterBench, fakeCalibrationSupervisor, fakeAlgoSupervisor, mockDialogOwnerService.Object, new TestDispatcher());
            filterViewModel.Init();
            // When 
            filterViewModel.StartCalibration.Execute(null);

            // Then            
            filterViewModel.CalibrationResult.Should().BeEquivalentTo(dummyCalibrationResult, options => options
                    .Excluding(x => x.CalibrationStatus));
        }

        [TestMethod]
        public void ShouldSaveCalibration()
        {
            // Given
            var calibrationServiceMock = new Mock<ICalibrationService>();
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var mockDialogOwnerService = new Mock<IDialogOwnerService>();
            mockDialogOwnerService.Setup(d => d.ShowMessageBox(It.IsAny<string>(),
                                                    It.IsAny<string>(),
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Question,
                                                    MessageBoxResult.No))
                                                    .Returns(MessageBoxResult.Yes);

            var filterBench = new FilterWheelBench(fakeFilterWheelSupervisor, calibrationServiceMock.Object, null);

            var fakeAlgoSupervisor = new FakeAlgoSupervisor();

            var filterViewModel =
                new FilterCalibrationVM(filterBench, calibrationServiceMock.Object, fakeAlgoSupervisor, mockDialogOwnerService.Object, new TestDispatcher());
            filterViewModel.Init();
            filterViewModel.CalibrationResult = new ObservableCollection<FilterVM> { new FilterVM(new Filter()) };

            // When
            filterViewModel.Save();

            // Then
            calibrationServiceMock.Verify(x =>
                x.SaveCalibration(It.Is<FilterData>(y => !y.Filters.IsEmpty())));
        }
        [TestMethod]
        public void PerformAutofocus_ShouldSetBusyStateAndCallMove_WhenCalled()
        {         
            // Given 
            var fakeAlgoSupervisor = new FakeAlgoSupervisor();
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var fakeCalibrationSupervisor = new FakeCalibrationSupervisor();
            var mockDialogOwnerService = new Mock<IDialogOwnerService>();
            mockDialogOwnerService.Setup(d => d.ShowMessageBox(It.IsAny<string>(),
                                                    It.IsAny<string>(),
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Question,
                                                    MessageBoxResult.No))
                                                    .Returns(MessageBoxResult.Yes);

            var filterBench = new FilterWheelBench(fakeFilterWheelSupervisor, fakeCalibrationSupervisor, null);

            var filterViewModel =
                new FilterCalibrationVM(filterBench, fakeCalibrationSupervisor, fakeAlgoSupervisor, mockDialogOwnerService.Object, new TestDispatcher());
            filterViewModel.Init();
            var paramFilterVM = new FilterVM(0, new Filter { Name = "NoFilter", Position = 0 });
            filterViewModel.CurrentCalibrationFilter = paramFilterVM;

            // When 
            filterViewModel.RunAutoFocus.Execute(paramFilterVM);

            // Then            
            Assert.AreEqual(filterViewModel.CurrentCalibrationFilter.DistanceOnFocus, 1.2);
        }
        [TestMethod]
        public void PerformDeleteCalibrationItem_ShouldRemoveFilter_WhenConfirmed()
        {
            var mockDialogOwnerService = new Mock<IDialogOwnerService>();
            var fakeAlgoSupervisor = new FakeAlgoSupervisor();
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var fakeCalibrationSupervisor = new FakeCalibrationSupervisor();
            var filterBench = new FilterWheelBench(fakeFilterWheelSupervisor, fakeCalibrationSupervisor, null);

            var filterViewModel =
                new FilterCalibrationVM(filterBench, fakeCalibrationSupervisor, fakeAlgoSupervisor, mockDialogOwnerService.Object, new TestDispatcher());
            filterViewModel.Init();
            // Arrange
            var filterVM = new FilterVM(0, new Filter { Name = "Filter1" });
            filterViewModel.CalibrationResult.Add(filterVM);
            mockDialogOwnerService.Setup(d => d.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>(), It.IsAny<MessageBoxResult>()))
                                   .Returns(MessageBoxResult.Yes);

            // Act
            filterViewModel.DeleteCalibrationItem.Execute(filterVM);

            // Assert
            Assert.IsFalse(filterViewModel.CalibrationResult.Contains(filterVM));
        }
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenHasNotChanged()
        {
            // Arrange
            var mockDialogOwnerService = new Mock<IDialogOwnerService>();
            var fakeAlgoSupervisor = new FakeAlgoSupervisor();
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var fakeCalibrationSupervisor = new FakeCalibrationSupervisor();
            var filterBench = new FilterWheelBench(fakeFilterWheelSupervisor, fakeCalibrationSupervisor, null);

            var filterViewModel =
                new FilterCalibrationVM(filterBench, fakeCalibrationSupervisor, fakeAlgoSupervisor, mockDialogOwnerService.Object, new TestDispatcher());
            filterViewModel.Init();
            filterViewModel.HasChanged = false;

            // Act
            bool result = filterViewModel.CanLeave(null);

            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when HasChanged is false.");
        }

        [TestMethod]
        public void CanLeave_ShouldReturnFalse_WhenHasChangedAndUserClicksNo()
        {
            // Arrange
            var mockDialogOwnerService = new Mock<IDialogOwnerService>();
            var fakeAlgoSupervisor = new FakeAlgoSupervisor();
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var fakeCalibrationSupervisor = new FakeCalibrationSupervisor();
            var filterBench = new FilterWheelBench(fakeFilterWheelSupervisor, fakeCalibrationSupervisor, null);

            var filterViewModel =
                new FilterCalibrationVM(filterBench, fakeCalibrationSupervisor, fakeAlgoSupervisor, mockDialogOwnerService.Object, new TestDispatcher());
            filterViewModel.Init();
            filterViewModel.HasChanged = true;

            const string expectedMessage = "You have unsaved changes. Do you really want to leave?";
            const string expectedTitle = "Confirmation";

            mockDialogOwnerService
                .Setup(d => d.ShowMessageBox(
                    expectedMessage,
                    expectedTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No))
                .Returns(MessageBoxResult.No);

            // Act
            bool result = filterViewModel.CanLeave(null);

            // Assert
            Assert.IsFalse(result, "Expected CanLeave to return false when HasChanged is true, and user declined to leave by clicking No.");
        }
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenForceCloseIsTrue()
        {
            // Arrange
            var mockDialogOwnerService = new Mock<IDialogOwnerService>();
            var fakeAlgoSupervisor = new FakeAlgoSupervisor();
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var fakeCalibrationSupervisor = new FakeCalibrationSupervisor();
            var filterBench = new FilterWheelBench(fakeFilterWheelSupervisor, fakeCalibrationSupervisor, null);

            var filterViewModel =
                new FilterCalibrationVM(filterBench, fakeCalibrationSupervisor, fakeAlgoSupervisor, mockDialogOwnerService.Object, new TestDispatcher());
            filterViewModel.Init();
            // Act
            bool result = filterViewModel.CanLeave(null, forceClose: true);
            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when forceClose is true, regardless of HasChanged.");
        }
    }
}
