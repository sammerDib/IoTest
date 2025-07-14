using ADCEngine;

namespace DataLoaderModule_DMT
{
    ///////////////////////////////////////////////////////////////////////
    // Factory: Demeter Deflectometry
    ///////////////////////////////////////////////////////////////////////
    internal class DMTDeflectometryFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "DMT-Deflectometry"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DMTDeflectometryModule(this, id, recipe);
        }

    }
}
