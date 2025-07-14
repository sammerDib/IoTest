using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class CalibrateDialogViewModel : ObservableObject, IModalDialogViewModel

    {
        #region Fields

        private readonly IDialogOwnerService _dialogService;
        private DualLiseCalibParams _calibrateParams;
        private bool? _dialogResult;

        #endregion Fields

        #region Private methods

        private AutoRelayCommand _startCalibrate;

        private bool AllowCalibration()
        {
            bool checkRef = (CalibrateParams.ProbeCalibrationReference != null);
            bool checkNumber = CalibrateParams.NbRepeatCalib > 0;
            return checkRef && checkNumber;
        }

        #endregion Private methods

        #region Properties

        private List<OpticalReferenceDefinition> _references = null;

        public List<OpticalReferenceDefinition> References
        {
            get
            {
                return _references ?? (_references = new List<OpticalReferenceDefinition>());
            }

            set
            {
                if (_references == value)
                {
                    return;
                }
                _references = value;
                OnPropertyChanged(nameof(References));
            }
        }

        #endregion Properties

        #region Public methods

        public CalibrateDialogViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            }

            CalibrateParams.NbRepeatCalib = 16;

            var chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            References = chuckSupervisor.ChuckVM.AnaChuckConfiguration.ReferencesList;
        }

        public DualLiseCalibParams CalibrateParams
        {
            get
            {
                return _calibrateParams ?? (_calibrateParams = new DualLiseCalibParams());
            }
            set
            {
                _calibrateParams = value;
            }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        public AutoRelayCommand StartCalibrate
        {
            get
            {
                return _startCalibrate
                    ?? (_startCalibrate = new AutoRelayCommand(
                    () =>
                    {
                        DialogResult = true;
                    },
                    AllowCalibration
                    ));
            }
        }

        #endregion Public methods
    }
}
