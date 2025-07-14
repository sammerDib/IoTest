using ADCEngine;

namespace AdvancedModules.Edition.VID
{
    internal class GTVIDFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "GTVID"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override DataProducerType DataProducer => DataProducerType.NoData;

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new GTVIDModule(this, id, recipe);
        }

    }
}
