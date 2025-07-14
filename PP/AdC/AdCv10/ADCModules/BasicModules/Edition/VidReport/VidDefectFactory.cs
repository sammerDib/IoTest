using ADCEngine;

namespace BasicModules.VidReport
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class VidDefectFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "VidDefect"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new VidDefectModule(this, id, recipe);
        }

        public override bool ModifiesData { get { return false; } }
        public override DataProducerType DataProducer { get { return DataProducerType.OptionnalData; } }

    }
}
