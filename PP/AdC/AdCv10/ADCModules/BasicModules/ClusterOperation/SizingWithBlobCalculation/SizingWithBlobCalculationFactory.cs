using ADCEngine;

namespace BasicModules.Sizing
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class SizingWithBlobCalculationFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "SizingWithBlobCalculation"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new SizingWithBlobCalculationModule(this, id, recipe);
        }
    }
}
