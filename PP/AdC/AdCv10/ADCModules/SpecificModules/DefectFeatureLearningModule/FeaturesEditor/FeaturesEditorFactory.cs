using ADCEngine;

namespace DefectFeatureLearning.FeaturesEditor
{
    internal class FeaturesEditorFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "FeaturesEditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new FeaturesEditorModule(this, id, recipe);
        }

        public override bool ModifiesData { get { return false; } }
    }
}
