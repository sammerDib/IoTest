using DeepLearningSoft48.Resources;

namespace DeepLearningSoft48.Modules
{
    /// <summary>
    /// Modules Factory Interface
    /// </summary>
    public abstract class IModuleFactory
    {
        public abstract ModuleBase FactoryMethod();
        public abstract string ModuleName { get; }
        public override string ToString() { return ModuleName; }

        //=================================================================
        // HMI Text
        //=================================================================
        private string _label;
        public string Label
        {
            get
            {
                if (_label == null)
                {
                    ModuleResource moduleResource = UIResources.Instance.GetModuleResource(ModuleName);
                    if (moduleResource != null && !string.IsNullOrEmpty(moduleResource.UIValue))
                    {
                        _label = moduleResource.UIValue;
                    }
                    else
                        _label = ModuleName;
                }
                return _label;
            }
        }
    }
}
