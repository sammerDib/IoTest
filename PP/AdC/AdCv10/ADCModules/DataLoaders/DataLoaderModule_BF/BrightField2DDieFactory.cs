using ADCEngine;


namespace DataLoaderModule_BF
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class BrightField2DDieFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "BF2D-Die"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new BrightField2DDieModule(this, id, recipe);
        }
    }

}
