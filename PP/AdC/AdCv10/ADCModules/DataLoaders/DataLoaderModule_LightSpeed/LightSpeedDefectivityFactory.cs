using ADCEngine;

namespace DataLoaderModule_LightSpeed
{
    ///////////////////////////////////////////////////////////////////////
    // Factory: LightSpeed
    ///////////////////////////////////////////////////////////////////////
    internal class LightSpeedDefectivityFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "LightSpeed-Defectivity"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new LightSpeedDefectivityModule(this, id, recipe);
        }
    }
}
