using ADCEngine;

namespace BasicModules.OpeningClosing
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class ClosingFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Closing"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_OpeningClosing; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ClosingModule(this, id, recipe);
        }
    }
}
