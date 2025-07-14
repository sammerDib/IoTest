using ADCEngine;

namespace AdvancedModules.BorderRemoval
{
    internal class ClusterEdgeRemoveByBlobFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ClusterEdgeRemoveByBlob"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ClusterEdgeRemoveByBlobModule(this, id, recipe);
        }
    }
}
