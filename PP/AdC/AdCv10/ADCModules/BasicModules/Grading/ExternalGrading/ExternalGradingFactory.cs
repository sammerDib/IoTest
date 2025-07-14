using ADCEngine;

namespace BasicModules.Grading.ExternalGrading
{
    public class ExternalGradingFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ExternalGrading"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ExternalGradingModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }
    }
}
