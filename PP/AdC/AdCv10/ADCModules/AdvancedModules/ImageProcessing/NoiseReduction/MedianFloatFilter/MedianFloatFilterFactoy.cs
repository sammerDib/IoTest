using ADCEngine;

namespace AdvancedModules.ImageProcessing.NoiseReduction.MedianFloatFilter
{
    internal class MedianFloatFilterFactoy : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "MedianFloatFilter"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MedianFloatFilterModule(this, id, recipe);
        }
    }
}
