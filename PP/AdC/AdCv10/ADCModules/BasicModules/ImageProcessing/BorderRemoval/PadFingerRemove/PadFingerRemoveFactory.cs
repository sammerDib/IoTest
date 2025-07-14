using ADCEngine;

namespace BasicModules.BorderRemoval
{
    internal class PadFingerRemoveFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "PadFingerRemove"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PadFingerRemoveModule(this, id, recipe);
        }
    }
}
