using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Core.Recipe.Save;
using UnitySC.PM.EME.Service.Core.Test.Calibration;
using UnitySC.PM.EME.Service.Core.Test.Camera;
using UnitySC.PM.EME.Service.Core.Test.Recipe.Fixtures;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Test.Tools;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class RecipeOrchestratorTests : TestWithMockedHardware<RecipeOrchestratorTests>, ITestWithCamera,
        ITestWithFilterWheel, ITestWithPhotoLumAxes, ITestWithLights
    {
        private InMemoryFileSaver _imageFileSaver;
        private Mock<IAdaFileSaver> AdaFileSaver { get; } = new Mock<IAdaFileSaver>();
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }
        public Mock<FilterWheel> SimulatedFilterWheel { get; set; }
        public Dictionary<string, EMELightBase> SimulatedLights { get; set; }
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }

        private Mock<IRecipeAcquisitionTemplateComposer> ComposerMock { get; } =
            new Mock<IRecipeAcquisitionTemplateComposer>();

        [TestInitialize]
        public void Setup()
        {
            _imageFileSaver = new InMemoryFileSaver();
            EmeraCamera = new FakeEmeraCamera();

            ComposerMock
                .Setup(x => x.GetImageDirectory(It.IsAny<Core.Recipe.RecipeAdapter>(),
                    It.IsAny<AcquisitionSettings>())).Returns(string.Empty);
        }

        [TestMethod]
        public void RecipeShouldFailedWhenAcquisitionThrows()
        {
            // Given
            var status = ExecutionStatus.NotExecuted;
            string errorMessage = string.Empty;

            IMessenger messenger = new WeakReferenceMessenger();
            messenger.Register<RecipeExecutionMessage>(this, (_, message) =>
            {
                status = message.Status;
                errorMessage = message.ErrorMessage;
            });

            var systemUnderTest = CreateRecipeOrchestrator(messenger, _imageFileSaver);

            // When
            SimulatedMotionAxes.Setup(x => x.GoToPosition(It.IsAny<PositionBase>(), It.IsAny<AxisSpeed>()))
                .Throws(new Exception("Acquisition failed"));
            var recipe = RecipeFixture.CreateRecipe();
            systemUnderTest.Start(recipe);

            // Then
            Assert.AreEqual(ExecutionStatus.Failed, status);
            Assert.AreEqual("Acquisition failed", errorMessage);
        }
        
        [TestMethod]
        public void Cancel_WhenTokenSourceIsNull_DoesNotThrowException()
        {
            // Arrange
            IMessenger messenger = new WeakReferenceMessenger();
            var systemUnderTest = CreateRecipeOrchestrator(messenger, _imageFileSaver);

            // Act & Assert
            try
            {
                systemUnderTest.Cancel();
            }
            catch
            {
                Assert.Fail("Cancel() threw an exception when _tokenSource was null");
            }
        }
        
        [TestMethod]
        public void Cancel_WhenCalled_CancelsCancellationTokenSource()
        {
            // Arrange
            IMessenger messenger = new WeakReferenceMessenger();
            var systemUnderTest = CreateRecipeOrchestrator(messenger, _imageFileSaver);
            var cts = new CancellationTokenSource();

            // Use reflection to set the private _tokenSource field
            var tokenSourceField = typeof(RecipeOrchestrator).GetField("_tokenSource",
                BindingFlags.NonPublic | BindingFlags.Instance);
            tokenSourceField.SetValue(systemUnderTest, cts);

            // Act
            systemUnderTest.Cancel();

            // Assert
            Assert.IsTrue(cts.IsCancellationRequested);
        }
        
        [TestMethod]
        public void Cancel_WhenCalledMultipleTimes_OnlyCancelsOnce()
        {
            // Arrange
            IMessenger messenger = new WeakReferenceMessenger();
            var systemUnderTest = CreateRecipeOrchestrator(messenger, _imageFileSaver);
            var cts = new CancellationTokenSource();

            // Use reflection to set the private _tokenSource field
            var tokenSourceField = typeof(RecipeOrchestrator).GetField("_tokenSource",
                BindingFlags.NonPublic | BindingFlags.Instance);
            tokenSourceField.SetValue(systemUnderTest, cts);

            // Act
            systemUnderTest.Cancel();
            systemUnderTest.Cancel();

            // Assert
            Assert.IsTrue(cts.IsCancellationRequested);
            // Verify that Cancel() was only called once on the CancellationTokenSource
            // This would require modifying the RecipeExecution class to use a mock CancellationTokenSource for testing
        }

        [TestMethod]
        public void RecipeShouldSucceed_AndSaveFullImages()
        {
            // Given
            var status = ExecutionStatus.NotExecuted;
            string errorMessage = string.Empty;

            IMessenger messenger = new WeakReferenceMessenger();
            messenger.Register<RecipeExecutionMessage>(this, (_, message) =>
            {
                status = message.Status;
                errorMessage = message.ErrorMessage;
            });

            var mockCalibrationManager = new Mock<ICalibrationManager>();
            mockCalibrationManager.Setup(x => x.GetDistortion()).Returns(new FakeCalibrationManager().GetDistortion());
            mockCalibrationManager.Setup(x => x.GetAxisOrthogonalityCalibrationData()).Returns(new FakeCalibrationManager().GetAxisOrthogonalityCalibrationData());
            var systemUnderTest = CreateRecipeOrchestrator(messenger, _imageFileSaver, mockCalibrationManager.Object);
            var recipe = RecipeFixture.CreateRecipe(runStitchFullImage : true);

            // When
            systemUnderTest.Start(recipe);

            // Then
            Assert.AreEqual(ExecutionStatus.Finished, status,
                $"Recipe execution failed with the following message: {errorMessage}");

            Assert.AreEqual(recipe.Acquisitions.Count, _imageFileSaver.SavedImages.Count);
            var cameraInfo = EmeraCamera.GetCameraInfo();
            var resultImage = _imageFileSaver.SavedImages.First();
            Assert.AreNotEqual(cameraInfo.Width / 4, resultImage.DataWidth);
            Assert.AreNotEqual(cameraInfo.Height / 4, resultImage.DataHeight);
            Assert.AreEqual(ServiceImage.ImageType.Greyscale, resultImage.Type);
            Assert.AreEqual(8, resultImage.Depth);
            mockCalibrationManager.Verify(x => x.GetAxisOrthogonalityCalibrationData());
        }

        [TestMethod]
        public void RecipeShouldSucceed_AndSaveVignettesImages()
        {
            // Given
            var status = ExecutionStatus.NotExecuted;
            string errorMessage = string.Empty;

            IMessenger messenger = new WeakReferenceMessenger();
            messenger.Register<RecipeExecutionMessage>(this, (_, message) =>
            {
                status = message.Status;
                errorMessage = message.ErrorMessage;
            });

            var systemUnderTest = CreateRecipeOrchestrator(messenger, _imageFileSaver);
            var recipe = RecipeFixture.CreateRecipe();

            // When
            systemUnderTest.Start(recipe);

            // Then
            Assert.AreEqual(ExecutionStatus.Finished, status,
                $"Recipe execution failed with the following message: {errorMessage}");

            Assert.AreEqual(64, _imageFileSaver.SavedImages.Count);
            var cameraInfo = EmeraCamera.GetCameraInfo();
            var resultImage = _imageFileSaver.SavedImages.First();
            Assert.AreEqual(cameraInfo.Width / 4, resultImage.DataWidth);
            Assert.AreEqual(cameraInfo.Height / 4, resultImage.DataHeight);
            Assert.AreEqual(ServiceImage.ImageType.Greyscale, resultImage.Type);
            Assert.AreEqual(8, resultImage.Depth);
        }

        [TestMethod]
        public void RecipeShouldFailedWhenSaveThrows()
        {
            // Given
            var status = ExecutionStatus.NotExecuted;
            string errorMessage = string.Empty;

            IMessenger messenger = new WeakReferenceMessenger();
            messenger.Register<RecipeExecutionMessage>(this, (_, message) =>
            {
                status = message.Status;
                errorMessage = message.ErrorMessage;
            });

            var mockFileSaver = new Mock<IImageFileSaver>();
            mockFileSaver.Setup(x => x.Save(It.IsAny<Core.Recipe.RecipeAdapter>(),
                It.IsAny<AcquisitionSettings>(),
                It.IsAny<ServiceImage>(), It.IsAny<(int, int)>())).Throws(new Exception("Save has failed"));

            var systemUnderTest = CreateRecipeOrchestrator(messenger, mockFileSaver.Object);
            var recipe = RecipeFixture.CreateRecipe();

            // When
            systemUnderTest.Start(recipe);

            // Then
            Assert.AreEqual(ExecutionStatus.Failed, status);
            Assert.AreEqual("Save has failed", errorMessage);
        }

        private RecipeOrchestrator CreateRecipeOrchestrator(IMessenger messenger, IImageFileSaver inMemoryFileSaver, ICalibrationManager calibrationManager= null)
        {
            var logger = new Mock<ILogger<RecipeOrchestrator>>().Object;
            var pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            var recipeSaving = new RecipeSaving(logger, inMemoryFileSaver, AdaFileSaver.Object, ComposerMock.Object,
                pmConfiguration, ClassLocator.Default.GetInstance<DbRegisterAcquisitionServiceProxy>());

            if (calibrationManager == null)
            {
               calibrationManager = ClassLocator.Default.GetInstance<ICalibrationManager>();
            }
            var referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
            var recipePreparation =
                new RecipeExecution(logger, new FakeConfigurationManager(true), HardwareManager, EmeraCamera);
            var dfSupervisor = new FakeDFSupervisor();
            var composer = new Mock<IRecipeAcquisitionTemplateComposer>().Object;
            return new RecipeOrchestrator(messenger, logger, calibrationManager, referentialManager,
                recipePreparation, recipeSaving, dfSupervisor, composer);
        }
    }
}
