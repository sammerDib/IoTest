using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // Classe de base, utilisée pour la réflexion
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public abstract class RenderingViewModelBase : ObservableRecipient
    {
        public ModuleBase Module { get; set; }

        public ObservableCollection<ObjectBase> RenderingObjects => Module.RenderingObjects;

        public RenderingViewModelBase(ModuleBase module)
        {
            Module = module;
        }
    }
}
