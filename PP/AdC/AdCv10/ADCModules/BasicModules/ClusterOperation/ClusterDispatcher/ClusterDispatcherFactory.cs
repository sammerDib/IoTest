using ADCEngine;

namespace BasicModules.ClusterDispatcher
{
    internal class DispatcherFactory
    {
        ///////////////////////////////////////////////////////////////////////
        // Factory
        ///////////////////////////////////////////////////////////////////////
        private class ClusterDispatcherModuleFactory : IModuleFactory
        {
            public override string ModuleName
            {
                get { return "ClusterDispatcher"; }
            }

            public override eModuleType ModuleType
            {
                get { return eModuleType.en_ClusterOperation; }
            }

            public override ModuleBase FactoryMethod(int id, Recipe recipe)
            {
                return new ClusterDispatcherModule(this, id, recipe);
            }

        }
    }
}
