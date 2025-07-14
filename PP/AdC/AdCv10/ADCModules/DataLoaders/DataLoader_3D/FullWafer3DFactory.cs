using ADCEngine;

namespace DataLoaderModule_3D
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class FullWafer3DFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "3D-FullWafer"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new FullWafer3DModule(this, id, recipe);
        }
    }

}
