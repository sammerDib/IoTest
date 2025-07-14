using ADCEngine;

namespace BasicModules.KlarfEditor
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class KlarfEditorModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "KlarfEditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new KlarfEditorModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.OptionnalData; } }
        public override bool ModifiesData { get { return false; } }

    }
}
