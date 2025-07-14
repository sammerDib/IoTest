using ADCEngine;

namespace BasicModules.ShapeDetection
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class SobelModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Sobel"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ShapeDetection; }
        }


        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new SobelModule(this, id, recipe);
        }

    }
}
