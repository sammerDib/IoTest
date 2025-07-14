using ADCEngine;

namespace AdvancedModules.Binarisation
{
    internal class ThresholdMedianFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ThresholdMedian"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Binarisation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ThresholdMedianModule(this, id, recipe);
        }
    }
}
