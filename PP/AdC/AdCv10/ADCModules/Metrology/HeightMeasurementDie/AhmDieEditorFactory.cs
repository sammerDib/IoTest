using ADCEngine;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class AhmDieEditorFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "AHMDieEditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new AhmDieEditorModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }

    }
}
