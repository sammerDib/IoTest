using ADCEngine;

namespace BasicModules.NoiseReduction
{
    internal class MilBlobFilteringFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "MilBlobFiltering"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MilBlobFilteringModule(this, id, recipe);
        }
    }
}
