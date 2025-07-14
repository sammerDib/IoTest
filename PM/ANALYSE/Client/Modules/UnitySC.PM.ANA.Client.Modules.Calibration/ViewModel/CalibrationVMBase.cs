using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Service.Interface.Calibration;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel
{
    public abstract class CalibrationVMBase : ObservableObject, IWizardNavigationItem, INavigable, IDisposable
    {
        public CalibrationVMBase(string name)
        {
            Name = name;
        }

        public bool IsEnabled { get; protected set; } = true;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage = "Calibrating";

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }

        public string Name { get; set; }
        bool IWizardNavigationItem.IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = false;

        private bool _isValidated = false;

        public bool IsValidated
        {
            get => _isValidated; set { if (_isValidated != value) { _isValidated = value; OnPropertyChanged(); } }
        }

        private bool _hasChanged;

        public bool HasChanged
        {
            get => _hasChanged; set { if (_hasChanged != value) { _hasChanged = value; OnPropertyChanged(); } }
        }

        public abstract void Init();

        public abstract void UpdateCalibration(ICalibrationData calibrationData);

        public abstract void CancelChanges();

        public abstract bool CanCancelChanges();

        public abstract void Save();

        public abstract bool CanSave();

        public abstract void Dispose();

        public abstract Task PrepareToDisplay();

        public abstract bool CanLeave(INavigable nextPage, bool forceClose = false);
    }
}
