using ADCEngine;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class HeightMeasurementClusterFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HeightMeasurementCluster"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HeightMeasurementClusterModule(this, id, recipe);
        }
    }
}
