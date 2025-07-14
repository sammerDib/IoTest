using ADCEngine;

namespace HazeLSModule
{
    internal class HazeLSAddReportFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HazeLSAddReport"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HazeLSAddReportModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }
        public override bool AcceptMultipleParents { get { return false; } }


    }

}
