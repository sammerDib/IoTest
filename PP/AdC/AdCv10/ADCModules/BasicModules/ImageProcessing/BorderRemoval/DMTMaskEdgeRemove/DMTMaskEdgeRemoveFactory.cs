using ADCEngine;

namespace BasicModules.BorderRemoval.DMTMaskEdgeRemove
{
    public class DMTMaskEdgeRemoveFactory: IModuleFactory
    {
        public override string ModuleName { get => "DMTMaskEdgeRemove"; }
        
        public override eModuleType ModuleType { get => eModuleType.en_BorderRemoval; }
        
        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new DMTMaskEdgeRemoveModule(this, id, recipe);
        }
        
    }
}
