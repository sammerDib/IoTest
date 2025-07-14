using ADCEngine;

namespace BasicModules.OpeningClosing
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class FillingFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Filling"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_OpeningClosing; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new FillingModule(this, id, recipe);
        }
    }
}
