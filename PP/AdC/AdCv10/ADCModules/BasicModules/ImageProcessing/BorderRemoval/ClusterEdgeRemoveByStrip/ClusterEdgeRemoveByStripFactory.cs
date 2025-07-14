using ADCEngine;

namespace BasicModules.BorderRemoval
{
    internal class ClusterEdgeRemoveByStripFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ClusterEdgeRemoveByStrip"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ClusterEdgeRemoveByStripModule(this, id, recipe);
        }
    }
}
