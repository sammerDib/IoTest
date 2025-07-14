using ADCEngine;

namespace AdvancedModules.MultiLayerClusterDispatcher
{
    internal class MultiLayerClusterDispatcherFactory
    {
        ///////////////////////////////////////////////////////////////////////
        // Factory
        ///////////////////////////////////////////////////////////////////////
        private class ClusterDispatcherModuleFactory : IModuleFactory
        {
            public override string ModuleName
            {
                get { return "MultiLayerClusterDispatcher"; }
            }

            public override eModuleType ModuleType
            {
                get { return eModuleType.en_ClusterOperation; }
            }

            public override ModuleBase FactoryMethod(int id, Recipe recipe)
            {
                return new MultiLayerClusterDispatcherModule(this, id, recipe);
            }

        }
    }
}
