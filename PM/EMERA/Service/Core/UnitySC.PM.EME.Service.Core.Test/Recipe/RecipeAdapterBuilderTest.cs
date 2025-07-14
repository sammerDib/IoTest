using System;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Core.Test.Calibration;
using UnitySC.PM.EME.Service.Core.Test.Camera;
using UnitySC.PM.EME.Service.Core.Test.Recipe.Fixtures;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class RecipeAdapterBuilderTest
    {
        [TestMethod]
        public void ShouldSucceed()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var recipe = EmeRecipeFixture.CreateRecipe();
            var systemUnderTest = new RecipeAdapterBuilder(new SerilogLogger<RecipeAdapterBuilder>(), messenger,
                new FakeEmeraCamera(), new FakeHardwareManager(recipe), new FakeCalibrationManager());

            // When
            var adaptedRecipe = systemUnderTest.ValidateAndBuild(recipe);

            // Then
            var expectedLights = recipe.Acquisitions.Select(acquisition => acquisition.LightDeviceId).ToList();
            var actualLights = adaptedRecipe.Acquisitions.Select(acquisition => acquisition.Light.DeviceID).ToList();
            CollectionAssert.AreEquivalent(expectedLights, actualLights);

            var expectedFilters = recipe.Acquisitions.Select(acquisition => acquisition.Filter).ToList();
            var actualFilters = adaptedRecipe.Acquisitions.Select(acquisition => acquisition.Filter.Type).ToList();
            CollectionAssert.AreEquivalent(expectedFilters, actualFilters);
        }

        [TestMethod]
        public void ShouldFail_WhenAcquisitionFilterNameDontMatchInstalledFilters()
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

            var recipe = EmeRecipeFixture.CreateRecipeWithInvalidFilter();

            // When
            var systemUnderTest =
                new RecipeAdapterBuilder(new SerilogLogger<RecipeAdapterBuilder>(), messenger, new FakeEmeraCamera(),
                    new FakeHardwareManager(recipe), new FakeCalibrationManager());
            Assert.ThrowsException<Exception>(() => systemUnderTest.ValidateAndBuild(recipe));

            // Then
            Assert.AreEqual(ExecutionStatus.Failed, status);
            Assert.AreEqual("The filter Unknown in the acquisition does not match any of the available filters.",
                errorMessage);
        }

        [TestMethod]
        public void ShouldFail_WhenAcquisitionLightIdDontMatchInstalledLights()
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

            var recipe = EmeRecipeFixture.CreateRecipeWithInvalidLight();

            // When
            var systemUnderTest =
                new RecipeAdapterBuilder(new SerilogLogger<RecipeAdapterBuilder>(), messenger, new FakeEmeraCamera(),
                    new FakeHardwareManager(EmeRecipeFixture.CreateRecipe()), new FakeCalibrationManager());
            Assert.ThrowsException<Exception>(() => systemUnderTest.ValidateAndBuild(recipe));

            // Then
            Assert.AreEqual(ExecutionStatus.Failed, status);
            Assert.AreEqual("The light Unknown in the acquisition does not match any of the available lights.",
                errorMessage);
        }

    }
}
