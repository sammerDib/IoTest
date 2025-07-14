using ADCEngine;


namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class Edge_Col_exclusionFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Edge_Col_Exclusion"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Loader; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new EdgeModule_Col_Exclusion(this, id, recipe);
        }

    }
}
