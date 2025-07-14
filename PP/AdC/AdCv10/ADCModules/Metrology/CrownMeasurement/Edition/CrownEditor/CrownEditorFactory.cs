using ADCEngine;

namespace CrownMeasurementModule.CrownEditor
{
    internal class CrownEditorFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CrownEditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CrownEditorModule(this, id, recipe);
        }
        //------
        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }
    }
}
