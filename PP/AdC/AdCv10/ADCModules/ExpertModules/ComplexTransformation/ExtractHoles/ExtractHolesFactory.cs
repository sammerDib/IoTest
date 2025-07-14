using ADCEngine;

namespace ExpertModules.ComplexTransformation
{
    internal class ExtractHolesFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ExtractHoles"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ComplexTransformation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ExtractHolesModule(this, id, recipe);
        }
    }
}
