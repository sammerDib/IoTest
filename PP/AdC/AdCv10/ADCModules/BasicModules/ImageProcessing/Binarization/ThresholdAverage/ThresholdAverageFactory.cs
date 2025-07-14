using ADCEngine;

namespace BasicModules.Binarisation
{
    internal class ThresholdAverageFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ThresholdAverage"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Binarisation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ThresholdAverageModule(this, id, recipe);
        }
    }
}
