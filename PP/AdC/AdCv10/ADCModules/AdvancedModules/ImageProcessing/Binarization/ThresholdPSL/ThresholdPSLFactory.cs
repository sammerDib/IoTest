using ADCEngine;

namespace AdvancedModules.Binarisation
{
    internal class ThresholdPSLFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ThresholdPSL"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Binarisation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ThresholdPSLModule(this, id, recipe);
        }
    }
}
