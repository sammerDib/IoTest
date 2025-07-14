using ADCEngine;


namespace BasicModules.Classification
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class ClassificationFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Classification"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ClassificationModule(this, id, recipe);
        }
    }

}
