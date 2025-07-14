using System;

using ADCEngine;

namespace HazeModule
{
    // DARkVIEW HAZE VERSION
    [Obsolete("Only For DarkviewModule - use Haze LS otherwise", false)]
    internal class HazeMeasurementFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HazeDKFMeasurement"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HazeMeasurementModule(this, id, recipe);
        }
    }
}
