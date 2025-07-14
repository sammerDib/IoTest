using ADCEngine;

namespace BasicModules.Mathematic
{
    internal class AdditionFactory : IModuleFactory
    {
        public override string ModuleName { get { return "Addition"; } }
        public override eModuleType ModuleType { get { return eModuleType.en_Mathematic; } }
        public override bool AcceptMultipleParents { get { return true; } }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new AdditionModule(this, id, recipe);
        }
    }
}
