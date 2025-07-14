using ADCEngine;

namespace AdcBasicObjects
{
	class ModuleBaseImageFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ModuleBaseImage"; }
        }
		public override ModuleBase FactoryMethod(int id, Recipe recipe)
		{
			return new ModuleBaseImage(this, id, recipe);
		}


		public override eModuleType ModuleType
		{
			get { return eModuleType.en_ImageModule; }
		}
	}
}
