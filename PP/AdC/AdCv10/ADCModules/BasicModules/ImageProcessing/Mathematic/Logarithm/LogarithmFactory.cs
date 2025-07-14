using ADCEngine;

namespace BasicModules.Mathematic
{
    internal class LogarithmFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Logarithm"; }
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
            return new LogarithmModule(this, id, recipe);
        }
    }
}
