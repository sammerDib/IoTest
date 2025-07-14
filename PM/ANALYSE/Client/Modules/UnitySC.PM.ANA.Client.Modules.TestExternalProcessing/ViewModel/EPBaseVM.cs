using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.TestExternalProcessing.ViewModel
{
    public abstract class EPBaseVM : ObservableObject, IWizardNavigationItem
    {
        public EPBaseVM(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        bool IWizardNavigationItem.IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; }
        public bool IsValidated { get; set; }

        public abstract void Init();
    }
}
