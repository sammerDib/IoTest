using ADCEngine;

namespace DataLoaderModule_3D
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class Die3DFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "3D-Die"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new Die3DModule(this, id, recipe);
        }
    }

}
