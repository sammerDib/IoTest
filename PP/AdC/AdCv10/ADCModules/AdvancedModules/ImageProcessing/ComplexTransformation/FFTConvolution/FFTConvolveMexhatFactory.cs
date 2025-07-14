using ADCEngine;

namespace AdvancedModules.ComplexTransformation
{
    internal class FFTConvolveMexhatFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "FFTConvolveMexhat"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ComplexTransformation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new FFTConvolveMexhatModule(this, id, recipe);
        }
    }
}
