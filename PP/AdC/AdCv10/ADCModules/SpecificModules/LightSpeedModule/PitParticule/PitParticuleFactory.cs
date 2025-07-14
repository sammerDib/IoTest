using AdcBasicObjects;

using ADCEngine;

namespace SpecificModules.LightSpeedModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class PitParticuleFactory : IModuleFactory
    {
        public static Characteristic RatioCharacteristic;

        static PitParticuleFactory()
        {
            RatioCharacteristic = new Characteristic(typeof(double), "Ratio");
        }

        public override string ModuleName
        {
            get { return "PitParticule"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PitParticuleModule(this, id, recipe);
        }
    }
}
