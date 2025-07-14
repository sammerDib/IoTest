using ADCEngine;

namespace DataLoaderModule_DMT
{
    ///////////////////////////////////////////////////////////////////////
    // Factory: Demeter Brightfield
    ///////////////////////////////////////////////////////////////////////
    internal class DMTBrightfieldFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "DMT-Brightfield"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DMTBrightfieldModule(this, id, recipe);
        }

    }
}
