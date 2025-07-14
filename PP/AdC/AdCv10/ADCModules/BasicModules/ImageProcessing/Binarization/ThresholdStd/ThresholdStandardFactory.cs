using ADCEngine;

namespace BasicModules.Binarisation
{
    internal class ThresholdStandardFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ThresholdStandard"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Binarisation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ThresholdStandardModule(this, id, recipe);
        }
    }
}
