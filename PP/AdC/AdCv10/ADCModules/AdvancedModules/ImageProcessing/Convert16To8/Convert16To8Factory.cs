using ADCEngine;
using AdcTools;

namespace AdvancedModules.Convert16To8
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    class Convert16To8ModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Convert16To8"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_OtherOperation; }
        }


        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new Convert16To8Module(this, id, recipe);
        }

    }
}
