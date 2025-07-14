using ADCEngine;

namespace AdvancedModules.StitchingImage
{
    internal class StitchingImageFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "StitchingImages"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new StitchingImageModule(this, id, recipe);
        }

        public override bool NeedAllData { get { return true; } }

    }
}
