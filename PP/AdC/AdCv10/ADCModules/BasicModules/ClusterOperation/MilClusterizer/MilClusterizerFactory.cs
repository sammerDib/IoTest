using ADCEngine;

namespace BasicModules.MilClusterizer
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class MilClusterizerFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "MilClusterizer"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MilClusterizerModule(this, id, recipe);
        }
    }
}
