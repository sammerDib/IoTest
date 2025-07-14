using ADCEngine;


namespace DataLoaderModule_BF
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class PhotolumFullImageFactory: IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Photolum"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PhotolumFullImageModule(this, id, recipe);
        }
    }

}
