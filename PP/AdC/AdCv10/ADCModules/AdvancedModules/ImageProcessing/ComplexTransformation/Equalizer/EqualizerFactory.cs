using ADCEngine;

namespace AdvancedModules.ComplexTransformation
{
    internal class EqualizerFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Equalizer"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ComplexTransformation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new EqualizerModule(this, id, recipe);
        }
    }
}
