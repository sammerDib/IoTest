using ADCEngine;
using AdcTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoaderModule_SICPL
{
    ///////////////////////////////////////////////////////////////////////
    // Factory: PsdNotPsdSiC Photoluminescence
    ///////////////////////////////////////////////////////////////////////
    public class SicPlFactory : IModuleFactory
	{
		public override string ModuleName
		{
			get { return "SiC-Photoluminescence"; }
		}

		public override eModuleType ModuleType
		{
			get { return eModuleType.en_Loader; }
		}

		public override ModuleBase FactoryMethod(int id, Recipe recipe)
		{
			return new SicPlModule(this, id, recipe);
		}

	}
}
