using ADCEngine;

namespace HazeLSModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class HazeLSEditorModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HazeLSEditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HazeLSEditorModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }


    }
}
