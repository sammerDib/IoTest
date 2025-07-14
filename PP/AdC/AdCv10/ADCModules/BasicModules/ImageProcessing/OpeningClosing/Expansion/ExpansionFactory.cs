using ADCEngine;

namespace BasicModules.OpeningClosing
{
    internal class ExpansionFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Expansion"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_OpeningClosing; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ExpansionModule(this, id, recipe);
        }
    }
}
