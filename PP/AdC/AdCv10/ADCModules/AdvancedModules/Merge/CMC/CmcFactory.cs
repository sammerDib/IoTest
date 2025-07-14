using AdcBasicObjects;

using ADCEngine;

namespace AdvancedModules.CmcNamespace
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class CmcFactory : IModuleFactory
    {
        public static Characteristic NbLayers = new Characteristic(typeof(double), "NbLayers");
        public static Characteristic LayerIsMeasured = new Characteristic(typeof(bool), "LayerIsMeasured");

        public override string ModuleName
        {
            get { return "CMC"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Merge; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new CmcModule(this, id, recipe);
        }

        public override bool AcceptMultipleParents { get { return true; } }
        public override bool NeedAllData { get { return true; } }

    }
}