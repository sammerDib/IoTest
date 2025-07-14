using ADCEngine;

namespace AdvancedModules.Edition.VID
{
    internal class CrownVIDFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CrownVID"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override DataProducerType DataProducer => DataProducerType.NoData;

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CrownVIDModule(this, id, recipe);
        }

    }
}
