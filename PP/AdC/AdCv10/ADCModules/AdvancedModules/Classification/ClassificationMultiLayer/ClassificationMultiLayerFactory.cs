using ADCEngine;


namespace AdvancedModules.ClassificationMultiLayer
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class ClassificationMultiLayerFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ClassificationMultiLayer"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Merge; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ClassificationMultiLayerModule(this, id, recipe);
        }
    }
}
