using ADCEngine;

namespace DataLoaderModule_LightSpeed
{
    ///////////////////////////////////////////////////////////////////////
    // Factory: LightSpeed
    ///////////////////////////////////////////////////////////////////////
    internal class LightSpeedLSEFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "LightSpeed-LSE"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new LightSpeedLSEModule(this, id, recipe);
        }
    }
}
