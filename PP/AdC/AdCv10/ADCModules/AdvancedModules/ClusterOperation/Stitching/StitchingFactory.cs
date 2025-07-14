using ADCEngine;

namespace AdvancedModules.ClusterOperation.Stitching
{
    internal class StitchingFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Stitching"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new StitchingModule(this, id, recipe);
        }

        public override bool NeedAllData { get { return true; } }

    }
}
