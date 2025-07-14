using ADCEngine;

namespace BasicModules.ImageProcessing.Mathematic.Average
{
    public class AverageFactory : IModuleFactory
    {
        public override string ModuleName { get { return "Average"; } }
        public override eModuleType ModuleType { get { return eModuleType.en_Mathematic; } }
        public override bool AcceptMultipleParents { get { return true; } }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new AverageModule(this, id, recipe);
        }
    }
}
