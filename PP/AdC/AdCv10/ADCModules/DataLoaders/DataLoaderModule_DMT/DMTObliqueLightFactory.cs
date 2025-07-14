using ADCEngine;

namespace DataLoaderModule_DMT
{
    ///////////////////////////////////////////////////////////////////////
    // Factory: Demeter ObliqueLight
    ///////////////////////////////////////////////////////////////////////
    internal class DMTObliqueLightFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "DMT-ObliqueLight"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DMTObliqueLightModule(this, id, recipe);
        }

    }
}
