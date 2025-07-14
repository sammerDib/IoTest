using ADCEngine;

namespace AdvancedModules.ComplexTransformation
{
    internal class ConvolveFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Convolve"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ComplexTransformation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ConvolveModule(this, id, recipe);
        }
    }
}
