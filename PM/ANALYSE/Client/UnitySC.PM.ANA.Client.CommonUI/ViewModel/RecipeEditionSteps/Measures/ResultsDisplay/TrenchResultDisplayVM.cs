using MvvmDialogs;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class TrenchResultDisplayVM : StepBaseVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public TrenchResultDisplayVM(TrenchPointResult trenchPointResult, TrenchSettings trenchSettings, string resultFolderPath, DieIndex dieIndex)
        {
            TrenchSettings = trenchSettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;
            TrenchPointResult = trenchPointResult;
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        private TrenchPointResult _trenchPointResult = null;

        public TrenchPointResult TrenchPointResult
        {
            get => _trenchPointResult;
            set
            {
                if (_trenchPointResult != value)
                {
                    _trenchPointResult = value;
                    UpdateTrenchResult();
                    OnPropertyChanged();
                }
            }
        }

        private bool _resultAvailable;

        public bool ResultAvailable
        {
            get => _resultAvailable; set { if (_resultAvailable != value) { _resultAvailable = value; OnPropertyChanged(); } }
        }

        private void UpdateTrenchResult()
        {
            if ((_trenchSettings is null) || (TrenchPointResult is null) || string.IsNullOrEmpty(_resultFolderPath))
                return;

            TrenchResult = new TrenchDetailMeasureInfoVM();
            TrenchResult.Settings = new TrenchResultSettings();
            TrenchResult.Settings.WidthTarget = TrenchSettings.WidthTarget;
            TrenchResult.Settings.WidthTolerance = TrenchSettings.WidthTolerance;
            TrenchResult.Settings.DepthTarget = TrenchSettings.DepthTarget;
            TrenchResult.Settings.DepthTolerance = TrenchSettings.DepthTolerance;
            TrenchResult.Digits = 3;
            TrenchPointResult.GenerateStats();
            TrenchResult.Point = TrenchPointResult;
            OnPropertyChanged(nameof(TrenchResult));
        }

        private TrenchSettings _trenchSettings;

        public TrenchSettings TrenchSettings
        {
            get => _trenchSettings; private set { if (_trenchSettings != value) { _trenchSettings = value; OnPropertyChanged(); } }
        }

        private string _resultFolderPath;

        public string ResultFolderPath
        {
            get => _resultFolderPath; private set { if (_resultFolderPath != value) { _resultFolderPath = value; OnPropertyChanged(); } }
        }

        private DieIndex _dieIndex;

        public DieIndex DieIndex
        {
            get => _dieIndex; private set { if (_dieIndex != value) { _dieIndex = value; OnPropertyChanged(); } }
        }

        private TrenchDetailMeasureInfoVM _trenchResult = null;

        public TrenchDetailMeasureInfoVM TrenchResult
        {
            get => _trenchResult;
            private set => SetProperty(ref _trenchResult, value);
        }

        public bool HasImage => false;

        private bool _useRepeta = false;

        public bool UseRepeta
        {
            get => _useRepeta;
            set => SetProperty(ref _useRepeta, value);
        }
    }
}
