using ADCEngine;

namespace DataLoaderModule_3D
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class MultiPictures3DFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "3D-MultiPictures"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MultiPictures3DModule(this, id, recipe);
        }
    }

}
