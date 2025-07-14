using ADCEngine;

namespace HazeLSModule
{
    internal class HazeLSMeasurementFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HazeLSMeasurement"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HazeLSMeasurementModule(this, id, recipe);
        }
    }
}
