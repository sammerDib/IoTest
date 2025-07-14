using System;

using ADCEngine;

namespace HazeModule
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    ///    // DARkVIEW HAZE VERSION
    [Obsolete("Only For DarkviewModule - use Haze LS otherwise", false)]
    internal class HazeEditorModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "HazeDKFEditor"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ReportingEdition; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new HazeEditorModule(this, id, recipe);
        }

        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        public override bool ModifiesData { get { return false; } }

    }
}
