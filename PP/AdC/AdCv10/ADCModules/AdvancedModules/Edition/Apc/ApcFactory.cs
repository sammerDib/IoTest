using ADCEngine;

namespace AdvancedModules.Edition.Apc
{
    internal class ApcFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Apc"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override DataProducerType DataProducer => DataProducerType.NoData;

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ApcModule(this, id, recipe);
        }

    }
}
