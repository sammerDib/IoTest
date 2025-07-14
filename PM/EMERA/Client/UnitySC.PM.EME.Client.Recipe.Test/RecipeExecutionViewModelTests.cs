using System;

using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Recipe.Test
{
    [TestClass]
    public class RecipeExecutionViewModelTests
    {
        [TestInitialize]
        public void Setup()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register<Mapper>();
            var messenger = new WeakReferenceMessenger();
            var cameraBench = new CameraBench(new FakeCameraSupervisor(messenger), new FakeCalibrationSupervisor(), messenger);
            ClassLocator.Default.Register(() => cameraBench);            
        }

        [TestMethod]
        public void ShouldExecuteRecipeWithSuccess()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var supervisor = new Mock<IEMERecipeService>();
            supervisor.Setup(s => s.StartRecipe(It.IsAny<EMERecipe>(), String.Empty))
                .Returns(new Response<VoidResult>())
                .Callback(() => messenger.Send(new RecipeExecutionMessage() { Status = ExecutionStatus.Finished }));

            var recipeExecution = new RecipeExecutionViewModel(messenger, supervisor.Object, new EMERecipeVM());

            // When
            recipeExecution.ExecuteRecipeCommand.Execute(null);

            // Then
            supervisor.Verify(x => x.StartRecipe(It.IsAny<EMERecipe>(), String.Empty));
            Assert.AreEqual(ExecutionStatus.Finished, recipeExecution.ExecutionStatus);
            Assert.IsFalse(recipeExecution.IsRecipeExecuting);
        }

        [TestMethod]
        public void ShouldCancelRecipe()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var supervisor = new Mock<IEMERecipeService>();
            supervisor.Setup(s => s.StopRecipe())
                .Returns(new Response<VoidResult>())
                .Callback(() => messenger.Send(new RecipeExecutionMessage() { Status = ExecutionStatus.Canceled }));

            var recipeExecution = new RecipeExecutionViewModel(messenger, supervisor.Object, new EMERecipeVM());
            // When
            recipeExecution.ExecuteRecipeCommand.Execute(null);
            recipeExecution.CancelRecipeCommand.Execute(null);

            // Then
            supervisor.Verify(x => x.StopRecipe());
        }
        [TestMethod]
        public void GetEMERecipe_ShouldReturnMappedEMERecipe()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var supervisor = new Mock<IEMERecipeService>();          
             var editedRecipeMock = new EMERecipeVM
            {
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
                },
                Execution = new ExecutionSettings() 
                            { ConvertTo8Bits = true, CorrectDistortion= false, NormalizePixelValue = true, ReduceResolution = false, RunStitchFullImage = false, RunAutoExposure = true, RunAutoFocus = false, Strategy= AcquisitionStrategy.Serpentine, RunBwa = true},
                IsSaveResultsEnabled = true
            };
            var expectedRecipe = new EMERecipe
            {
                Acquisitions = editedRecipeMock.Acquisitions,
                Execution = editedRecipeMock.Execution,
                IsSaveResultsEnabled = editedRecipeMock.IsSaveResultsEnabled
            };

            var recipeExecution = new RecipeExecutionViewModel(messenger, supervisor.Object, editedRecipeMock);

            // Act
            var result = recipeExecution.GetEMERecipe();
            bool areEqual = expectedRecipe.Acquisitions.Count == editedRecipeMock.Acquisitions.Count &&
                   expectedRecipe.Acquisitions.Zip(editedRecipeMock.Acquisitions, (a1, a2) => a1.Name == a2.Name
                                                           && a1.ExposureTime == a2.ExposureTime
                                                           && a1.Filter == a2.Filter
                                                           && a1.LightDeviceId == a2.LightDeviceId).All(b => b);
            // Assert
            Assert.IsNotNull(result);            
            Assert.AreEqual(expectedRecipe.Execution, result.Execution);
            Assert.AreEqual(expectedRecipe.IsSaveResultsEnabled, result.IsSaveResultsEnabled);                      
            Assert.IsTrue(areEqual);

        }
    }
}
