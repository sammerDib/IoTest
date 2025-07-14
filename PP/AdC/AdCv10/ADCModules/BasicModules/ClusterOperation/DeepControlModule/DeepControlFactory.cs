using System;

using ADCEngine;

namespace BasicModules.DeepControl
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class DeepControlModuleFactory : IModuleFactory
    {
        public override bool AcceptMultipleParents { get { return true; } }

        public override string ModuleName
        {
            get { return "DeepControl"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DeepControlModule(this, id, recipe);
        }

    }
}
