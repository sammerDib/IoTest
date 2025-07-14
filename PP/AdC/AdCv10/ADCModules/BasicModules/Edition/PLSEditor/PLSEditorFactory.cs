using System;

using ADCEngine;

namespace BasicModules.PLSEditor
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    [Obsolete("IBM acv9 specific, need to refacto full Feature within ADC USP when we have time")]
    internal class PLSEditorModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "PLSEditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PLSEditorModule(this, id, recipe);
        }

        //        public override DataProducerType DataProducer { get { return DataProducerType.OptionnalData; } }
        public override bool ModifiesData { get { return false; } }

    }
}
