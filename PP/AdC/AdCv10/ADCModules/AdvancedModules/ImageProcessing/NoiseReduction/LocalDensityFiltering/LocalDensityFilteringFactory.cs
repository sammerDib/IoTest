using ADCEngine;

namespace AdvancedModules.NoiseReduction
{
    internal class LocalDensityFilteringFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "LocalDensityFiltering"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new LocalDensityFilteringModule(this, id, recipe);
        }
    }
}
