using ADCEngine;

namespace AdvancedModules.ClusterOperation.Sieve

{
    internal class SieveFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Sieve"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new SieveModule(this, id, recipe);
        }
    }
}
