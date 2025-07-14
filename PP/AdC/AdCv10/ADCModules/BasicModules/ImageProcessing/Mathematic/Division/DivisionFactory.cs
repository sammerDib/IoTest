using ADCEngine;

namespace BasicModules.Mathematic
{
    internal class DivisionFactory : IModuleFactory
    {
        public override string ModuleName { get { return "Division"; } }
        public override eModuleType ModuleType { get { return eModuleType.en_Mathematic; } }
        public override bool AcceptMultipleParents { get { return true; } }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DivisionModule(this, id, recipe);
        }
    }
}
