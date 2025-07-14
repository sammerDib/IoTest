using ADCEngine;


namespace BasicModules.Edition.KlarfEditorEdge
{
    internal class KlarfEditorEdgeFactory : IModuleFactory
    {

        public override string ModuleName
        {
            get { return "KlarfEditorEdge"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new KlarfEditorEdgeModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.OptionnalData; } }
        public override bool ModifiesData { get { return false; } }
    }
}
