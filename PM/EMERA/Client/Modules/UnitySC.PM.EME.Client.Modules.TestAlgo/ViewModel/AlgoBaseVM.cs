using System;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public abstract class AlgoBaseVM : ObservableObject, IWizardNavigationItem, IDisposable
    {
        public AlgoBaseVM(string name)
        {
            Name = name;
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~AlgoBaseVM()
        {
            Dispose(false);
        }
        public string Name { get; set; }
        bool IWizardNavigationItem.IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; }
        public bool IsValidated { get; set; }
        public Rect RoiRect { get; set; } = Rect.Empty;
        public bool IsCenteredROI { get; set; }                       
        protected abstract void Dispose(bool disposing);      
    }
}
