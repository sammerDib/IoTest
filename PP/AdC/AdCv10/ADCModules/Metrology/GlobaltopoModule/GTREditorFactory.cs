using ADCEngine;

namespace GlobaltopoModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class GTREditorModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "GTREditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new GTREditorModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }
        //  public override bool AcceptMultipleParents { get { return true; } }


    }
}
