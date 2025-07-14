using ADCEngine;

namespace ExpertModules.ComplexTransformation.CustomConvolve
{
    internal class CustomConvolveFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CustomConvolve"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ComplexTransformation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CustomConvolveModule(this, id, recipe);
        }
    }
}
