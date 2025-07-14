using ADCEngine;

namespace BasicModules.Mathematic
{
    internal class AbsoluteValueFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "AbsoluteValue"; }
        }

        public override eModuleType ModuleType
        {
            get
            {
                return eModuleType.en_Mathematic;
            }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new AbsoluteValueModule(this, id, recipe);
        }
    }
}
