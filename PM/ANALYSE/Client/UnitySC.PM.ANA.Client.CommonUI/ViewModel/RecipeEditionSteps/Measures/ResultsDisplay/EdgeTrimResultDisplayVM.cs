using MvvmDialogs;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.EdgeTrim;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class EdgeTrimResultDisplayVM : StepBaseVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public EdgeTrimResultDisplayVM(EdgeTrimPointResult edgeTrimPointResult, EdgeTrimSettings edgeTrimSettings, string resultFolderPath, DieIndex dieIndex)
        {
            EdgeTrimSettings = edgeTrimSettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;
            EdgeTrimPointResult = edgeTrimPointResult;
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        private EdgeTrimPointResult _edgeTrimPointResult = null;

        public EdgeTrimPointResult EdgeTrimPointResult
        {
            get => _edgeTrimPointResult;
            set
            {
                if (_edgeTrimPointResult != value)
                {
                    _edgeTrimPointResult = value;
                    UpdateEdgeTrimResult();
                    OnPropertyChanged();
                }
            }
        }

        private bool _resultAvailable;

        public bool ResultAvailable
        {
            get => _resultAvailable; set { if (_resultAvailable != value) { _resultAvailable = value; OnPropertyChanged(); } }
        }

        private void UpdateEdgeTrimResult()
        {
            if ((_edgeTrimSettings is null) || (EdgeTrimPointResult is null) || string.IsNullOrEmpty(_resultFolderPath))
                return;

            EdgeTrimResult = new EdgeTrimMeasureInfoVM();
            EdgeTrimResult.Settings = new EdgeTrimResultSettings();
            EdgeTrimResult.Settings.WidthTarget = EdgeTrimSettings.WidthTarget;
            EdgeTrimResult.Settings.WidthTolerance = EdgeTrimSettings.WidthTolerance;
            EdgeTrimResult.Settings.HeightTarget = EdgeTrimSettings.HeightTarget;
            EdgeTrimResult.Settings.HeightTolerance = EdgeTrimSettings.HeightTolerance;
            EdgeTrimResult.Digits = 3;
            EdgeTrimPointResult.GenerateStats();
            EdgeTrimResult.Point = EdgeTrimPointResult;
            OnPropertyChanged(nameof(EdgeTrimResult));
        }

        private EdgeTrimSettings _edgeTrimSettings;

        public EdgeTrimSettings EdgeTrimSettings
        {
            get => _edgeTrimSettings; private set { if (_edgeTrimSettings != value) { _edgeTrimSettings = value; OnPropertyChanged(); } }
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

        private EdgeTrimMeasureInfoVM _edgeTrimResult = null;

        public EdgeTrimMeasureInfoVM EdgeTrimResult
        {
            get => _edgeTrimResult;
            private set => SetProperty(ref _edgeTrimResult, value);
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
