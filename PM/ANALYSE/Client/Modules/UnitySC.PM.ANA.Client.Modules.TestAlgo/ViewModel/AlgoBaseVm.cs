using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel
{
    public abstract class AlgoBaseVM : ObservableObject, IWizardNavigationItem
    {
        public AlgoBaseVM(string name)
        {
            Name = name;

            var toolService = new ServiceInvoker<IToolService>("ToolService",
             ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(),
             ClientConfiguration.GetDataAccessAddress());
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage = "Algo in progress.. ";

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }

        private bool _canDoAutofocus = true;

        public bool CanDoAutofocus
        {
            get => _canDoAutofocus; set { if (_canDoAutofocus != value) { _canDoAutofocus = value; OnPropertyChanged(); } }
        }

        public string Name { get; set; }
        bool IWizardNavigationItem.IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; }
        public bool IsValidated { get; set; }
        public Rect RoiRect { get; set; }
        public bool IsCenteredROI { get; set; }
        public abstract void Dispose();
    }
}
