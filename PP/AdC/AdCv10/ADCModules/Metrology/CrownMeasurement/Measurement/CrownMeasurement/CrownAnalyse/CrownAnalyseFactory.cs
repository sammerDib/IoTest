using ADCEngine;

namespace CrownMeasurementModule.CrownAnalyse
{
    internal class CrownAnalyseFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "CrownAnalyse"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CrownAnalyseModule(this, id, recipe);
        }
    }
}
