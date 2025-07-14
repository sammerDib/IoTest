using ADCEngine;

namespace AdvancedModules.NoiseReduction
{
    internal class TrimmingFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Trimming"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new TrimingModule(this, id, recipe);
        }
    }
}
