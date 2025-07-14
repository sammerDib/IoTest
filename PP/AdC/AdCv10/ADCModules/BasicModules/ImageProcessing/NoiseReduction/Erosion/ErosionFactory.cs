using ADCEngine;

namespace BasicModules.NoiseReduction
{
    internal class ErosionFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Erosion"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ErosionModule(this, id, recipe);
        }
    }
}
