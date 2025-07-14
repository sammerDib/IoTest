using System.Linq;

using ADCEngine;

using DataLoaderModule_BF;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data.Enum;

namespace ADC.Test
{
    [TestClass]
    public class DataloaderTests
    {
        private Recipe _recipe;
        
        [TestInitialize]
        public void Setup() {
            ADCEngine.ADC.Instance.Factories.Clear();
            ADCEngine.ADC.Instance.Factories.Add(new RootModuleFactory().ModuleName, new RootModuleFactory());
            ADCEngine.ADC.Instance.Factories.Add(new TerminationModuleFactory().ModuleName, new TerminationModuleFactory());

            _recipe = new Recipe();
        }

        [TestMethod]
        public void ShouldBeFalseWhenFilterPhotolumDataLoaderEntry()
        {
            // LightType: Photolum by default

            //  Arrange
            var factory = new PhotolumFullImageFactory();
            var photolumDataloader = new PhotolumFullImageModule(factory, 0, _recipe);
            var rootModule = _recipe.ModuleList.First(x => x.Key == -1).Value;
            _recipe.AddModule(rootModule, photolumDataloader);

            var restyp = ResultType.EME_Visible90; // Darkfield type

            

            //  Act
            bool ImageGoodToProcess = photolumDataloader.FilterImage(restyp);
            _recipe.DeleteModule(photolumDataloader);

            //  Assert
            Assert.IsFalse(ImageGoodToProcess);

        }

        [TestMethod]
        public void ShouldPassTheFilterWhenDarkfieldResultType()
        {
            // LightType: Photolum by default

            //  Arrange
            var factory = new PhotolumFullImageFactory();
            var photolumDataloader = new PhotolumFullImageModule(factory, 0, _recipe);
            photolumDataloader.ParamLightAcquisitionType.Value = LightAcquisition.LightAcquisitionType.DDF90;
            var rootModule = _recipe.ModuleList.First(x => x.Key == -1).Value;
            _recipe.AddModule(rootModule, photolumDataloader);

            var restyp = ResultType.EME_Visible90; // Darkfield type


            //  Act
            bool ImageGoodToProcess = photolumDataloader.FilterImage(restyp);
            _recipe.DeleteModule(photolumDataloader);


            //  Assert
            Assert.IsTrue(ImageGoodToProcess);
        }

        [TestMethod]
        public void ShouldFailFilterImageWhenNoPhotolumNorDarkfieldImage()
        {
            // LightType: Photolum by default

            //  Arrange
            var factory = new PhotolumFullImageFactory();
            var photolumDataloader = new PhotolumFullImageModule(factory, 0, _recipe);
            photolumDataloader.ParamLightAcquisitionType.Value = LightAcquisition.LightAcquisitionType.DDF90;
            var rootModule = _recipe.ModuleList.First(x => x.Key == -1).Value;
            _recipe.AddModule(rootModule, photolumDataloader);

            var restyp = ResultType.EME_Visible0; // Not Photolum nor Darkfield90

            //  Act
            bool ImageGoodToProcess = photolumDataloader.FilterImage(restyp);
            _recipe.DeleteModule(photolumDataloader);


            //  Assert
            Assert.IsTrue(ImageGoodToProcess);
        }
    }
}
