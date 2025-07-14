using ADCEngine;

namespace BasicModules.Mathematic
{
    internal class InversionFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Inversion"; }
        }

        public override eModuleType ModuleType
        {
            get
            {
                return eModuleType.en_Mathematic;
            }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new InversionModule(this, id, recipe);
        }
    }
}
