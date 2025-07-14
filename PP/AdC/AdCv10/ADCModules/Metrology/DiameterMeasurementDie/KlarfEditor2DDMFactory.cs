using ADCEngine;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class KlarfEditor2DDMModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "KlarfEditor2DDM"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new KlarfEditor2DDMModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }

    }
}
