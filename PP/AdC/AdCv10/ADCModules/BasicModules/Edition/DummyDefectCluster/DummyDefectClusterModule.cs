using System.Collections.Generic;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.Edition.DummyDefect;

namespace BasicModules.Edition.DummyDefectCluster
{

    /// <summary>
    /// Dummy defect used to not process childen to create custom dummy defect (ex: Merge images from different layers with Mathematic modules)
    /// </summary>
    public class DummyDefectClusterModule : DummyDefectModule, ICharacterizationModule, IClassifierModule
    {
        public List<Characteristic> AvailableCharacteristics => new List<Characteristic>();
        public List<string> DefectClassLabelList => new List<string>();

        public DummyDefectClusterModule(IModuleFactory factory, int id, Recipe recipe) : base(factory, id, recipe)
        {
            ChildrenMustBeProcess = false;
            ModuleProperty = eModuleProperty.Stage;
        }
    }
}
