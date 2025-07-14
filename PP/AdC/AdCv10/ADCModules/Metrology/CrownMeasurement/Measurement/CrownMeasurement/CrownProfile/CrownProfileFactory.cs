using ADCEngine;

namespace CrownMeasurementModule.CrownProfile
{
    internal class CrownProfileFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CrownProfile"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CrownProfileModule(this, id, recipe);
        }
    }
}
