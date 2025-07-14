using ADCEngine;

namespace HazeLSModule
{
    internal class AppendHazeResultFactory
    {
        ///////////////////////////////////////////////////////////////////////
        // Factory
        ///////////////////////////////////////////////////////////////////////
        private class AppendHazeResultModuleFactory : IModuleFactory
        {
            public override string ModuleName
            {
                get { return "AppendHazeResult"; }
            }

            public override eModuleType ModuleType
            {
                get { return eModuleType.en_Merge; }
            }

            public override ModuleBase FactoryMethod(int id, Recipe recipe)
            {
                return new AppendHazeResultModule(this, id, recipe);
            }

            public override bool AcceptMultipleParents { get { return true; } }

        }
    }
}
