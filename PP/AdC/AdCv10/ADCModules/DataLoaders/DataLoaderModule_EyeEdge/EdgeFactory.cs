using ADCEngine;


namespace DataLoaderModule_EyeEdge
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class EdgeFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Edge"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new EdgeModule(this, id, recipe);
        }

    }
}
