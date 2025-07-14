using ADCEngine;

namespace AdvancedModules.Edition.VID
{
    internal class BF3DVIDFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "BF3DVID"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override DataProducerType DataProducer => DataProducerType.NoData;

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new BF3DVIDModule(this, id, recipe);
        }

    }
}
