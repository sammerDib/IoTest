using ADCEngine;

namespace BasicModules.Edition.DummyDefectCluster
{
    public class DummyDefectClusterFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "DummyDefectCluster"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DummyDefectClusterModule(this, id, recipe);
        }
    }
}
