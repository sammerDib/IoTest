using ADCEngine;

namespace AdvancedModules.Edition.VirtualGrid
{
    internal class VirtualGridFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "VirtualGrid"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new VirtualGridModule(this, id, recipe);
        }

        public override bool AcceptMultipleParents { get { return true; } }
    }
}
