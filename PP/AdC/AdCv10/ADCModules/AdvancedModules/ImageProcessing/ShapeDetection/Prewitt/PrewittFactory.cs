using ADCEngine;

namespace AdvancedModules.ShapeDetection
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class PrewittFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Prewitt"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ShapeDetection; }
        }


        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PrewittModule(this, id, recipe);
        }

    }
}
