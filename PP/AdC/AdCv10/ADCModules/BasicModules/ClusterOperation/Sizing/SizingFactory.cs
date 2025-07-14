using ADCEngine;

namespace BasicModules.Sizing
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class SizingFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Sizing"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new SizingModule(this, id, recipe);
        }
    }
}
