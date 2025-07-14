using ADCEngine;

namespace ExpertModules.NoiseReduction.ExpertMedianFilter
{
    internal class ExprtMedianFilterFactoy : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ExpertMedianFilter"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_NoiseReduction; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ExpertMedianFilterModule(this, id, recipe);
        }
    }
}
