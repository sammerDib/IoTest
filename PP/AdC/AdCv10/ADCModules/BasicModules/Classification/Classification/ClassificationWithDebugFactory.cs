using ADCEngine;


namespace BasicModules.Classification
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class ClassificationWithDebugFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ClassificationWithDebugOutput"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ClassificationWithDebugModule(this, id, recipe);
        }
    }

}
