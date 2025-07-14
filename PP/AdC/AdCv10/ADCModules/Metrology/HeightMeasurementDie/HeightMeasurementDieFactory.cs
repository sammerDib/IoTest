using AdcBasicObjects;

using ADCEngine;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class HeightMeasurementDieFactory : IModuleFactory
    {
        // needed for classification Loading purpose and avoid classification loading failure (if Module is Loaded after classif)
        static private Characteristic dummyInitCaracClusterType = Cluster3DCharacteristics.BareHeightAverage;
        static private Characteristic dummyInitCaracBlobType = Blob3DCharacteristics.HeightMicron;

        public override string ModuleName
        {
            get { return "HeightMeasurementDie"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HeightMeasurementDieModule(this, id, recipe);
        }
    }
}
