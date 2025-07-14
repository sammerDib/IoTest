using ADCEngine;

namespace GlobaltopoModule
{
    internal class GTMeasurementFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "GTMeasurement"; } //bow warp computation
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new GTMeasurementModule(this, id, recipe);
        }
    }
}
