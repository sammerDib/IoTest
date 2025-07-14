using AdcBasicObjects;

using ADCEngine;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class DiameterMeasurementDieFactory : IModuleFactory
    {
        // needed for classification Loading purpose and avoid classification loading failure (if Module is Loaded after classif)
        static private Characteristic dummyInitCaracClusterType = Cluster2DCharacteristics.DiameterAverage;
        static private Characteristic dummyInitCaracBlobType = Blob2DCharacteristics.Diameter;

        public override string ModuleName
        {
            get { return "DiameterMeasurementDie"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DiameterMeasurementDieModule(this, id, recipe);
        }
    }
}
