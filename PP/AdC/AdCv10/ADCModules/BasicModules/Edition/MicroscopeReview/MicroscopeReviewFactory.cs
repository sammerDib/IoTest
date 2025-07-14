using ADCEngine;

namespace BasicModules.Edition.MicroscopeReview
{
    public class MicroscopeReviewFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "MicroscopeReview"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new MicroscopeReviewModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }
    }
}
