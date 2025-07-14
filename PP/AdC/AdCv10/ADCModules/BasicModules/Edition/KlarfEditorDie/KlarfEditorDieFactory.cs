using ADCEngine;

namespace BasicModules.KlarfEditor_Die
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class KlarfEditorDieModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "KlarfEditorDie"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new KlarfEditorDieModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.OptionnalData; } }
        public override bool ModifiesData { get { return false; } }

    }
}
