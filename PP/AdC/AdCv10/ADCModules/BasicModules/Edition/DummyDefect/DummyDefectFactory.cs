using ADCEngine;

namespace BasicModules.Edition.DummyDefect
{
    public class DummyDefectFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "DummyDefect"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DummyDefectModule(this, id, recipe);
        }
    }
}
