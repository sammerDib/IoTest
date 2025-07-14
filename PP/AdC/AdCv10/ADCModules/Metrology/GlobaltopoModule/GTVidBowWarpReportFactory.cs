using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCEngine;
using AdcTools;

namespace GlobaltopoModule
{
    class GTVidBowWarpReportFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "GTVidBowWarpReport"; } //bow warp Vid report
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Metrology; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new GTVidBowWarpReportModule(this, id, recipe);
        }
    }
}
