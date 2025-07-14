using ADCEngine;

namespace SpecificModules
{
    internal class FFTRemovePatternFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "FFTRemovePattern"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ComplexTransformation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new FFTRemovePatternModule(this, id, recipe);
        }
    }
}
