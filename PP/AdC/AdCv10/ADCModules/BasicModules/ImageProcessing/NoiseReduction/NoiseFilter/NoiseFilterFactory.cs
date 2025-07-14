using ADCEngine;

namespace BasicModules.NoiseReduction
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class NoiseFilterModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "NoiseFilter"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }


        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new NoiseFilterModule(this, id, recipe);
        }
    }

}
