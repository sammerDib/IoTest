using ADCEngine;

namespace AdvancedModules.BorderRemoval
{
    internal class MaskFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Mask"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_BorderRemoval; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MaskModule(this, id, recipe);
        }
    }
}
