using ADCEngine;

namespace SpecificModules.LightSpeedModule
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class CharacterizationLSEFactory : IModuleFactory
    {
        //public static Characteristic IsSaturatedCharacteristic;

        public override string ModuleName
        {
            get { return "Characterization-LSE"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            //if (IsSaturatedCharacteristic == null)
            //    IsSaturatedCharacteristic = new Characteristic(typeof(bool), "IsSaturated");
            return new CharacterizationLSEModule(this, id, recipe);
        }
    }
}
