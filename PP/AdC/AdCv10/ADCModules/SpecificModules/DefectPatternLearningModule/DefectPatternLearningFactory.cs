using AdcBasicObjects;

using ADCEngine;

namespace DefectPatternLearning
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class DefectPatternLearningFactory : IModuleFactory
    {
        public static Characteristic ClassCharacteristic;
        public static Characteristic ConfidenceP1Characteristic;
        public static Characteristic ConfidenceP2Characteristic;

        public DefectPatternLearningFactory()
        {
            if (ClassCharacteristic == null)
                ClassCharacteristic = new Characteristic(typeof(string), "Class");

            if (ConfidenceP1Characteristic == null)
                ConfidenceP1Characteristic = new Characteristic(typeof(double), "ConfidenceP1");

            if (ConfidenceP2Characteristic == null)
                ConfidenceP2Characteristic = new Characteristic(typeof(double), "ConfidenceP2");
        }

        public override string ModuleName
        {
            get { return "DefectPatternLearning"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DefectPatternLearningModule(this, id, recipe);
        }

        public override bool ModifiesData { get { return false; } }
    }
}
