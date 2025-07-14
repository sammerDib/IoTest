using ADCEngine;


namespace BasicModules.MilCharacterization
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class MilCharacterizationFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "MilCharacterization"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MilCharacterizationModule(this, id, recipe);
        }
    }

}
