using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Core.Recipe.Save;
using UnitySC.PM.EME.Service.Core.Test.Recipe.Fixtures;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class RecipeAcquisitionTemplateComposerTest
    {
        private readonly AcquisitionSettings _acquisition = new AcquisitionSettings()
        {
            Name = "Acquisition 1",
            Filter = new Filter("No Filter", EMEFilter.NoFilter, 0),
            Light = new EMELightConfig { DeviceID = "ddf_0deg", Type = EMELightType.DirectionalDarkField0Degree }
        };

        private readonly Mock<PMConfiguration> _pmConfigurationMock  = new Mock<PMConfiguration>();

        [TestInitialize]
        public void Setup()
        {
            _pmConfigurationMock.Object.Actor = ActorType.EMERA;
            _pmConfigurationMock.Object.OutputAcqServer = ".";
            _pmConfigurationMock.Object.OutputAcqPathTemplate = "{ActorType}\\{Tool}\\{Product}\\{Step}\\{Recipe}";
            _pmConfigurationMock.Object.OutputAcqFileNameTemplate =
                "{WaferCategory}_{Light}_{Filter}_C{Column}_L{Line}.tiff";
            _pmConfigurationMock.Object.OutputAdaFolder = "{Tool}\\{Product}";
            _pmConfigurationMock.Object.OutputAdaFileNameTemplate = "result.ada";
        }

        [TestMethod]
        public void ShouldGetTheVignetteImageSavingFullPath()
        {
            var systemUnderTest = new RecipeAcquisitionTemplateComposer(_pmConfigurationMock.Object, null);

            var position = (5, 3);
            var recipe = RecipeFixture.CreateRecipe();
            recipe.WaferCategory = "MyCategory";
            recipe.Product = "MyProduct";
            recipe.Step = "MyStep";
            string path = systemUnderTest.GetImageDirectory(recipe, _acquisition) +
                          systemUnderTest.GetVignetteImageFileName(recipe, _acquisition, position);
            
            Assert.AreEqual(".\\EMERA\\\\MyProduct\\MyStep\\MyCategory_ddf_0deg_NoFilter_C5_L3.tiff", path);
        }

        [TestMethod]
        public void ShouldGetTheFullImageSavingFullPath()
        {
            var systemUnderTest = new RecipeAcquisitionTemplateComposer(_pmConfigurationMock.Object, null);
            var recipe = RecipeFixture.CreateRecipe();
            recipe.WaferCategory = "MyCategory";
            recipe.Product = "MyProduct";
            recipe.Step = "MyStep";
            string path = systemUnderTest.GetImageDirectory(recipe, _acquisition) +
                          systemUnderTest.GetFullImageFileName(recipe, _acquisition);

            Assert.AreEqual(".\\EMERA\\\\MyProduct\\MyStep\\MyCategory_ddf_0deg_NoFilter.tiff", path);
        }

        [TestMethod]
        public void ShouldGetTheImageBaseName()
        {
            var systemUnderTest = new RecipeAcquisitionTemplateComposer(_pmConfigurationMock.Object, null);

            var recipe = RecipeFixture.CreateRecipe();
            recipe.WaferCategory = "MyCategory";
            recipe.Product = "MyProduct";
            string baseName = systemUnderTest.GetImageBaseName(recipe, _acquisition);
            Assert.AreEqual("MyCategory_ddf_0deg_NoFilter", baseName);
        }

        [TestMethod]
        public void ShouldGetTheAdaFilePath()
        {
            var systemUnderTest = new RecipeAcquisitionTemplateComposer(_pmConfigurationMock.Object, null);

            string result = systemUnderTest.GetAdaFileName(RecipeFixture.CreateRecipe());
            
            Assert.IsTrue(result.EndsWith(".ada"));
        }
    }
}
