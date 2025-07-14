using ADCEngine;

namespace BasicModules.Debug
{
    internal class WaitFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Wait"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Debug; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new WaitModule(this, id, recipe);
        }

        public override bool AcceptMultipleParents { get { return false; } }
    }
}
