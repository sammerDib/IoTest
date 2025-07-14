using ADCEngine;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class CustomAreaRemoveModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CustomAreaRemove"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CustomAreaRemoveModule(this, id, recipe);
        }
    }

}
