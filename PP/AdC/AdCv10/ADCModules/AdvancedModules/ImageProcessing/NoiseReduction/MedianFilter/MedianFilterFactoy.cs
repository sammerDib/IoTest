using ADCEngine;

namespace AdvancedModules.ImageProcessing.NoiseReduction.MedianFilter
{
    internal class MedianFilterFactoy : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "MedianFilter"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MedianFilterModule(this, id, recipe);
        }
    }
}
