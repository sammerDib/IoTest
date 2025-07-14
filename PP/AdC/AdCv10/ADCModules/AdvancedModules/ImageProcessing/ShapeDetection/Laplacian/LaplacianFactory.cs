using ADCEngine;

namespace AdvancedModules.ShapeDetection
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class LaplacianModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Laplacian"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ShapeDetection; }
        }


        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new LaplacianModule(this, id, recipe);
        }

    }
}
