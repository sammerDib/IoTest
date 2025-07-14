using ADCEngine;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class EdgeRemoveModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "EdgeRemove"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new EdgeRemoveModule(this, id, recipe);
        }
    }

}
