using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Recipe.Test
{
    [TestClass]
    public class AcquisitionsEditorVMTests
    {
        private AcquisitionsEditorViewModel _viewModel;
        private EMERecipeVM _editedRecipe;
        private WeakReferenceMessenger _messenger;
        private FilterWheelBench _filterWheelBench;
        private CameraBench _camera;
        private FakeLightsSupervisor _fakeLightsSupervisor;
        private LightBench _lightBench;

        [TestInitialize]
        public void SetupTest()
        {
            _messenger = new WeakReferenceMessenger();
            _camera = new CameraBench(new FakeCameraSupervisor(_messenger), new FakeCalibrationSupervisor(), _messenger);
            _filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), new FakeCalibrationSupervisor(), null);           
            _lightBench = new LightBench(new FakeLightsSupervisor(_messenger), new FakeKeyboardMouseHook(), null, _messenger);
            var sourceRecipe = new EMERecipe
            {
                Name = "SourceRecipe",
                ActorType = ActorType.EMERA,
                Key = Guid.NewGuid(),
                Acquisitions = new List<Acquisition>
                {
                    new Acquisition
                    {
                        Name = "Acquisition1", ExposureTime = 1.0, Filter = EMEFilter.NoFilter, LightDeviceId = "3"
                    },
                    new Acquisition
                    {
                        Name = "Acquisition2", ExposureTime = 2.0, Filter = EMEFilter.BandPass450nm50, LightDeviceId = "4"
                    }
                }
            };

            var mapper = new Mapper();
            _editedRecipe = mapper.AutoMap.Map<EMERecipeVM>(sourceRecipe);
            _viewModel =
                new AcquisitionsEditorViewModel(_editedRecipe, _camera, _filterWheelBench, _lightBench, null, null, _messenger);
        }
        [TestMethod]
        public void LoadRecipeData_LoadsAcquisitionsFromEditedRecipe()
        {
            // Arrange
            _viewModel.AcquisitionsViewModel.Clear();

            // Act
            _viewModel.LoadRecipeData();
            var acquisitions = new LinkedList<Acquisition>(_viewModel.AcquisitionsViewModel.Select(d => d.Item));
            Assert.AreEqual(2, acquisitions.Count);

            // Assert
            CollectionAssert.AreEqual(_editedRecipe.Acquisitions, acquisitions);
        }
        [TestMethod]
        public void SaveRecipeData_SavesAcquisitionsToEditedRecipe()
        {
            // Arrange
            _viewModel.LoadRecipeData();

            // Act
            _viewModel.SaveRecipeData();
            var acquisitions = new LinkedList<Acquisition>(_viewModel.AcquisitionsViewModel.Select(d => d.Item));

            // Assert
            CollectionAssert.AreEqual(acquisitions, _editedRecipe.Acquisitions);
        }
        [TestMethod]
        public void InsertAcquisition_AddsNewAcquisitionsToAcquisitionsEditor()
        {
            // Arrange
            int initialCount = _viewModel.AcquisitionsViewModel.Count;

            // Act
            _viewModel.AddAcquisition.Execute(null);

            // Assert
            Assert.AreEqual(initialCount + 1, _viewModel.AcquisitionsViewModel.Count);
            Assert.AreEqual(initialCount + 1, _editedRecipe.Acquisitions.Count);
            Assert.AreEqual(true, _editedRecipe.IsModified);
        }
        [TestMethod]
        public void DeleteAllCommand_WhenConfirmed_ClearsAcquisitions()
        {
            // Arrange
            var dialogOwnerServiceMock = new Mock<IDialogOwnerService>();
            dialogOwnerServiceMock.Setup(d => d.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)).Returns(MessageBoxResult.Yes);
            var viewModel = new AcquisitionsEditorViewModel(_editedRecipe, _camera, _filterWheelBench, _lightBench, dialogOwnerServiceMock.Object, null, _messenger);

            // Act
            viewModel.DeleteAll.Execute(null);

            // Assert
            Assert.AreEqual(0, viewModel.AcquisitionsViewModel.Count);
            Assert.AreEqual(0, _editedRecipe.Acquisitions.Count);
            Assert.AreEqual(true, _editedRecipe.IsModified);
            dialogOwnerServiceMock.Verify(d => d.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No), Times.Once);
        }
        [TestMethod]
        public void DeleteAcquisitionCommand_WhenConfirmed_RemovesAcquisition()
        {
            // Arrange
            var dialogOwnerServiceMock = new Mock<IDialogOwnerService>();
            dialogOwnerServiceMock.Setup(d => d.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)).Returns(MessageBoxResult.Yes);
            var viewModel = new AcquisitionsEditorViewModel(_editedRecipe, _camera, _filterWheelBench, _lightBench, dialogOwnerServiceMock.Object, null, _messenger);
            var acquisitionVM = viewModel.AcquisitionsViewModel.FirstOrDefault(x => x.Name == "Acquisition2");

            // Act
            viewModel.DeleteAcquisition.Execute(acquisitionVM);

            // Assert
            Assert.IsFalse(viewModel.AcquisitionsViewModel.Contains(acquisitionVM));
            Assert.IsFalse(_editedRecipe.Acquisitions.Contains(acquisitionVM.Item));
            Assert.AreEqual(true, _editedRecipe.IsModified);
            dialogOwnerServiceMock.Verify(d => d.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No), Times.Once);
        }
    }
}
