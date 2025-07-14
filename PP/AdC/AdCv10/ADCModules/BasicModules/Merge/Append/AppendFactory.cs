using ADCEngine;

namespace BasicModules.Append
{
    internal class AppendFactory
    {
        ///////////////////////////////////////////////////////////////////////
        // Factory
        ///////////////////////////////////////////////////////////////////////
        private class AppendModuleFactory : IModuleFactory
        {
            public override string ModuleName
            {
                get { return "Append"; }
            }

            public override eModuleType ModuleType
            {
                get { return eModuleType.en_Merge; }
            }

            public override ModuleBase FactoryMethod(int id, Recipe recipe)
            {
                return new AppendModule(this, id, recipe);
            }

            public override bool AcceptMultipleParents { get { return true; } }

        }
    }
}
