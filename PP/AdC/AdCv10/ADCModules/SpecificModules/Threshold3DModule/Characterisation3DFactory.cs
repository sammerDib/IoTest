using AdcBasicObjects;

using ADCEngine;

namespace Threshold3DModule
{
    internal class Characterisation3DFactory : IModuleFactory
    {
        // needed for classification Loading purpose and avoid classification loading failure (if Module is Loaded after classif)
        static private Characteristic dummyInitCaracClusterType = Cluster3DCharacteristics.BareHeightAverage;
        static private Characteristic dummyInitCaracBlobType = Blob3DCharacteristics.HeightMicron;

        public override string ModuleName
        {
            get { return "Characterisation3D"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new Characterisation3DModule(this, id, recipe);
        }
    }
}
