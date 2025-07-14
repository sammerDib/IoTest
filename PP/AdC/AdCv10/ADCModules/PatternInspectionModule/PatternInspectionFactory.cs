using ADCEngine;

namespace PatternInspectionModule
{
    internal class PatternInspectionFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "PatternInspection"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_PatternInspection; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PatternInspectionModule(this, id, recipe);
        }
    }
}
