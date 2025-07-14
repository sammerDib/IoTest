using ADCEngine;

namespace SpecificModules.Threshold3DModule
{
    internal class Threshold3DFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Threshold3D"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Binarisation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new Threshold3DModule(this, id, recipe);
        }
    }
}
