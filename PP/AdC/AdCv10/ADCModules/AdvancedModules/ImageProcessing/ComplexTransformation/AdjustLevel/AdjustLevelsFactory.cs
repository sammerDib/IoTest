using ADCEngine;

namespace AdvancedModules.ImageProcessing.ComplexTransformation.AdjustLevel
{
    internal class AdjustLevelsFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "AdjustLevels"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ComplexTransformation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new AdjustLevelsModule(this, id, recipe);
        }
    }
}
