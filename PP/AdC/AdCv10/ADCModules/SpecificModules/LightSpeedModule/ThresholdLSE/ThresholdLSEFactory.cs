using ADCEngine;

namespace SpecificModules.LightSpeedModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class ThresholdLSEFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Threshold-LSE"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Binarisation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ThresholdLSEModule(this, id, recipe);
        }
    }
}
