using ADCEngine;

namespace BasicModules.NoiseReduction
{
    internal class SmoothingFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Smoothing"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new SmoothingModule(this, id, recipe);
        }
    }
}
