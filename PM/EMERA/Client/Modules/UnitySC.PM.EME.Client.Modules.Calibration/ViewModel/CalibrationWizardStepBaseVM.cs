using System;
using System.Linq;
using System.Threading.Tasks;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public abstract class CalibrationWizardStepBaseVM : ViewModelBaseExt, IWizardNavigationItem, INavigable, IDisposable
    {
        protected readonly ICalibrationService _calibrationService;
        protected CalibrationWizardStepBaseVM(string name, ICalibrationService calibrationService = null)
        {
            _navigationManager = ClassLocator.Default.GetInstance<INavigationManager>();
            _calibrationService = calibrationService;
            Name = name;
            IsEnabled = true;
            IsMeasure = false;
            IsValidated = false;
        }
        public CalibrationWizardStepBaseVM GetNextPageOfSameType()
        {           
            var nextPage = _navigationManager.AllPages.SkipWhile(p => p.GetType() != GetType()).Skip(1).FirstOrDefault();            
            return nextPage as CalibrationWizardStepBaseVM;
        }
        public virtual void ValidateAndEnableNextPage()
        {
            IsValidated = true;
            var nextPage = GetNextPageOfSameType();
            if (nextPage != null)
            {
                nextPage.IsEnabled = true;
            }
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set => SetProperty(ref _isBusy, value);
        }

        private string _busyMessage = "Calibrating";

        public string BusyMessage
        {
            get => _busyMessage; set => SetProperty(ref _busyMessage, value);
        }

        private string _name;
        public string Name
        {
            get => _name; set => SetProperty(ref _name, value);
        }
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled; set => SetProperty(ref _isEnabled, value);
        }
        private bool _isMeasure;
        public bool IsMeasure
        {
            get => _isMeasure; set => SetProperty(ref _isMeasure, value);
        }        

        private bool _isValidated = false;
        public bool IsValidated
        {
            get => _isValidated; set => SetProperty(ref _isValidated, value);
        }
        private DateTime _creationDate;
        public DateTime CreationDate
        {
            get => _creationDate; set => SetProperty(ref _creationDate, value);
        }        
        private INavigationManager _navigationManager;
        public INavigationManager NavigationManager
        {
            get => _navigationManager; set => SetProperty(ref _navigationManager, value);
        }
        public virtual Task PrepareToDisplay()
        {
            return Task.CompletedTask;
        }
        public virtual bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            return true;
        }

        private bool _hasChanged;

        public bool HasChanged
        {
            get => _hasChanged; set => SetProperty(ref _hasChanged, value);
        }
        protected T LoadCalibrationData<T>() where T : class, ICalibrationData
        {
            var calibrations = _calibrationService?.GetCalibrations();
            var calib = calibrations?.Result.OfType<T>().FirstOrDefault();
            IsValidated = (calib != null);
            CreationDate = (calib != null) ? calib.CreationDate : CreationDate;
            return calib;
        }
        public abstract void Init();

        public abstract void CancelChanges();

        public abstract bool CanCancelChanges();

        public abstract void Save();

        public abstract bool CanSave();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Dispose(bool manualDisposing);

    }
}
