using ADCEngine;
using AdcTools;

using BasicModules.CroppepBorder;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    public class CroppeBorderModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CroppeBorder"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CroppeBorderModule(this, id, recipe);
        }
    }

}
