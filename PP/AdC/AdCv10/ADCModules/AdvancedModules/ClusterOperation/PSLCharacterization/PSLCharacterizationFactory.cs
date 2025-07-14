using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCEngine;
using AdcTools;


namespace BasicModules.PSLCharacterization
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    class PSLCharacterizationFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "PSLCharacterization"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new PSLCharacterizationModule(this, id, recipe);
        }
    }

}
