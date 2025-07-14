using ADCEngine;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class HeightMeasurementAnalysisFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HeightMeasurementAnalysis"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HeightMeasurementAnalysisModule(this, id, recipe);
        }
    }
}
