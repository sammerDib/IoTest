using ADCEngine;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class KlarfEditor3DHMModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "KlarfEditor3DHM"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new KlarfEditor3DHMModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }

    }
}
