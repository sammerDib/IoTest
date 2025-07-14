using ADCEngine;

namespace HazeLSModule
{
    internal class HazeLSMaskFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HazeLSMask"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HazeLSMaskModule(this, id, recipe);
        }
    }
}
