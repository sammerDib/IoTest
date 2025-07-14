using ADCEngine;

namespace AdvancedModules.ShapeDetection
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class SharpenModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Sharpen"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ShapeDetection; }
        }


        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new SharpenModule(this, id, recipe);
        }

    }
}
