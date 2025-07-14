using AdcModuleBase;
using AdcTools;


namespace BasicModules
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    class ThresholdModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Threshold"; }
        }

        public override ModuleBase FactoryMethod(int id, IRecipe recipe)
        {
            return new ThresholdModule(this, id, recipe);
        }
    }
}
