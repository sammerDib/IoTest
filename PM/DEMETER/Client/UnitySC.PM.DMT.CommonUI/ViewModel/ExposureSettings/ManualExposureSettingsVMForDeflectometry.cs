using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.Measure;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings
{
    internal class ManualExposureSettingsVMForDeflectometry : ManualExposureSettingsVM
    {
        public new DeflectometryVM Measure => (DeflectometryVM)base.Measure;

        public ManualExposureSettingsVMForDeflectometry(string title, MeasureVM measure, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService,
            Mapper mapper, MainRecipeEditionVM mainRecipeEditionVM)
            : base(title, measure, cameraSupervisor, screenSupervisor, calibrationSupervisor, algorithmsSupervisor, dialogService, mapper, mainRecipeEditionVM)
        {
            SelectedScreenColorIndex = 1;
            IsShowingHistogram = false;
            PropertyChanged += ManualExposureSettingsVMForDeflectometry_PropertyChanged;
        }

        private void ManualExposureSettingsVMForDeflectometry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propertyChanged = e.PropertyName;
            if (propertyChanged == nameof(SelectedScreenColorIndex))
                OnPropertyChanged(nameof(CanChangeSlopePrecision));
        }

        public override void SetExposureTime(double exposureTimeMs)
        {
            if (SelectedScreenColorIndex == 1)
                CameraSupervisor.SetExposureTime(Measure.Side, exposureTimeMs, period: Measure.Fringe.Period);
            else
                base.SetExposureTime(exposureTimeMs);
        }

        public bool CanChangeSlopePrecision
        {
            get
            {
                // If we use standard fringes and we are displaying a fringe
                return (Measure.AllowStandardFringes && (SelectedScreenColorIndex == 1));
            }
        }

        private int _slopePrecision;

        public int SlopePrecision
        {
            get
            {
                if (Measure != null)
                    _slopePrecision = Measure.SlopePrecision;
                return _slopePrecision;
            }
            set
            {
                if (_slopePrecision != value)
                {
                    _slopePrecision = value;

                    if (Measure != null)
                        Measure.SlopePrecision = _slopePrecision;

                    SetScreenImage();
                    OnPropertyChanged();
                }
            }
        }

        private double _maxSlopePrecision;

        public double MaxSlopePrecision
        {
            get
            {
                if (Measure != null)
                    _maxSlopePrecision = Measure.MaxSlopePrecision;
                return _maxSlopePrecision;
            }
        }
    }
}
