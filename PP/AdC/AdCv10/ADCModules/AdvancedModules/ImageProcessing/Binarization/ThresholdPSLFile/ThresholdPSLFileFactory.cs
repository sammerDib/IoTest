using ADCEngine;

namespace AdvancedModules.Binarisation
{
    internal class ThresholdPSLFileFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ThresholdPSLFile"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Binarisation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ThresholdPSLFileModule(this, id, recipe);
        }
    }
}
