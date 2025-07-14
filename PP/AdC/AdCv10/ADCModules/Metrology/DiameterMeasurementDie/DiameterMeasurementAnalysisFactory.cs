using ADCEngine;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class DiameterMeasurementAnalysisFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "DiameterMeasurementAnalysis"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DiameterMeasurementAnalysisModule(this, id, recipe);
        }
    }
}
