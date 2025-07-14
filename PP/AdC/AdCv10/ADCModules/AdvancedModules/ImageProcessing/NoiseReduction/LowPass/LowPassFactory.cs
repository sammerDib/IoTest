using ADCEngine;

namespace AdvancedModules.NoiseReduction
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class LowPassModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "LowPass"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }


        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new LowPassModule(this, id, recipe);
        }

    }
}
