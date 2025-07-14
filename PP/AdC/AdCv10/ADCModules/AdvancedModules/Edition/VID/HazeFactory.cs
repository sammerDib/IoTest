using ADCEngine;

namespace AdvancedModules.Edition.VID
{
    internal class HazeVIDFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HazeVID"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override DataProducerType DataProducer => DataProducerType.NoData;

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HazeVIDModule(this, id, recipe);
        }

    }
}
