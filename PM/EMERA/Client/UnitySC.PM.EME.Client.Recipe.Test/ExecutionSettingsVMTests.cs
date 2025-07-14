using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.EME.Client.Recipe.Test
{
    [TestClass]
    public class ExecutionSettingsVMTests
    {
        private EMERecipeVM _editedRecipe;

        [TestInitialize]
        public void SetupTest()
        {
            var sourceRecipe =
                new EMERecipe { Name = "SourceRecipe", ActorType = ActorType.EMERA, Key = Guid.NewGuid() };
            sourceRecipe.Execution = new ExecutionSettings
            {
                Strategy = AcquisitionStrategy.RasterScan,
                RunAutoFocus = true,
                RunAutoExposure = true,
                RunBwa = true,
                ReduceResolution = true,
                ConvertTo8Bits = true,
                CorrectDistortion = true,
                NormalizePixelValue = true,
                RunStitchFullImage = false
            };

            var mapper = new Mapper();
            _editedRecipe = mapper.AutoMap.Map<EMERecipeVM>(sourceRecipe);
        }

        [TestMethod]
        public void Constructor_SetsItemAndRegistersPropertyChangedEvent()
        {
            // Act
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Assert
            Assert.IsNotNull(viewModel.Item);
            Assert.AreEqual(_editedRecipe.Execution, viewModel.Item);
        }

        [TestMethod]
        public void CurrentAcquisitionStrategy_Setter_UpdatesStrategyAndRaisesPropertyChanged()
        {
            // Arrange
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);
            var strategy = AcquisitionStrategy.RasterScan;

            // Act
            viewModel.CurrentAcquisitionStrategy = strategy;

            // Assert
            Assert.AreEqual(strategy, viewModel.CurrentAcquisitionStrategy);
            Assert.AreEqual(_editedRecipe.Execution.Strategy, viewModel.CurrentAcquisitionStrategy);
        }

        [TestMethod]
        public void PropertyChanged_EventRaisedWhenItemChanges()
        {
            // Arrange
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);
            bool eventRaised = false;
            viewModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            // Act
            viewModel.Item = new ExecutionSettings();

            // Assert
            Assert.IsTrue(eventRaised);
            Assert.IsTrue(_editedRecipe.IsModified);
        }

        [TestMethod]
        public void AlignmentVMChanged_RunAutoFocusChanges()
        {
            // Arrange
            _editedRecipe.IsModified = false;
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Act
            viewModel.RunAutoFocus = false;

            // Assert        
            Assert.AreEqual(viewModel.RunAutoFocus, _editedRecipe.Execution.RunAutoFocus);
            Assert.IsTrue(_editedRecipe.IsModified);
        }

        [TestMethod]
        public void AlignmentVMChanged_RunAutoExposureChanges()
        {
            // Arrange
            _editedRecipe.IsModified = false;
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Act
            viewModel.RunAutoExposure = false;

            // Assert        
            Assert.AreEqual(viewModel.RunAutoExposure, _editedRecipe.Execution.RunAutoExposure);
            Assert.IsTrue(_editedRecipe.IsModified);
        }

        [TestMethod]
        public void AlignmentVMChanged_RunBwaChanges()
        {
            // Arrange
            _editedRecipe.IsModified = false;
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Act
            viewModel.RunBwa = false;

            // Assert
            Assert.AreEqual(viewModel.RunBwa, _editedRecipe.Execution.RunBwa);
            Assert.IsTrue(_editedRecipe.IsModified);
        }

        [TestMethod]
        public void AlignmentVMChanged_CompressImageChanges()
        {
            // Arrange
            _editedRecipe.IsModified = false;
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Act
            viewModel.ReduceResolution = false;

            // Assert
            Assert.IsFalse(_editedRecipe.Execution.ReduceResolution);
            Assert.IsTrue(_editedRecipe.IsModified);
        }

        [TestMethod]
        public void AlignmentVMChanged_ConvertTo8bits_ImageChanges()
        {
            // Arrange
            _editedRecipe.IsModified = false;
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Act
            viewModel.ConvertTo8Bits = false;

            // Assert
            Assert.IsFalse(_editedRecipe.Execution.ConvertTo8Bits);
            Assert.IsTrue(_editedRecipe.IsModified);
        }
        
        [TestMethod]
        public void ExecutionViewModelChanged_NormalizePixelValue_Changed()
        {
            // Arrange
            _editedRecipe.IsModified = false;
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Act
            viewModel.NormalizePixelValue = false;

            // Assert
            Assert.IsFalse(_editedRecipe.Execution.NormalizePixelValue);
            Assert.IsTrue(_editedRecipe.IsModified);
        }

        [TestMethod]
        public void ExecutionViewModelChanged_RunStitchFullImageValue_Changed()
        {
            // Arrange
            _editedRecipe.IsModified = false;
            var viewModel = new ExecutionSettingsViewModel(_editedRecipe);

            // Act
            viewModel.RunStitchFullImage = true;

            // Assert
            Assert.IsTrue(_editedRecipe.Execution.RunStitchFullImage);
            Assert.IsTrue(_editedRecipe.IsModified);
        }
    }
}
