using System.Windows.Media.Imaging;

using MvvmDialogs;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class BowResultDisplayVM : StepBaseVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public BowResultDisplayVM(BowPointResult bowPointResult, BowSettings bowSettings, string resultFolderPath, DieIndex dieIndex)
        {
            BowResult = bowPointResult;
            BowSettings = bowSettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;
            UpdateBowDetailResult();
        }

        private BowPointResult _bowResult = null;

        public BowPointResult BowResult
        {
            get => _bowResult;
            private set
            {
                if (_bowResult != value)
                {
                    _bowResult = value;
                    UpdateBowDetailResult();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateBowDetailResult()
        {
            if ((BowSettings is null) || (BowResult is null) || string.IsNullOrEmpty(ResultFolderPath))
                return;

            BowDetailResult = new BowDetailMeasureInfoVM();
            BowDetailResult.Settings = new BowResultSettings();
            BowDetailResult.Settings.BowTargetMin = BowSettings.BowMin;
            BowDetailResult.Settings.BowTargetMax = BowSettings.BowMax;

            BowResult.GenerateStats();
            BowDetailResult.Point = BowResult;
            if (!(DieIndex is null))
                BowDetailResult.DieIndex = $"Column  {DieIndex.Column}    Row  {DieIndex.Row}";
            OnPropertyChanged(nameof(BowDetailResult));
        }

        private BowSettings _bowSettings = null;

        public BowSettings BowSettings
        {
            get => _bowSettings; private set { if (_bowSettings != value) { _bowSettings = value; OnPropertyChanged(); } }
        }

        private string _resultFolderPath;

        public string ResultFolderPath
        {
            get => _resultFolderPath; private set { if (_resultFolderPath != value) { _resultFolderPath = value; OnPropertyChanged(); } }
        }

        private BitmapImage _resultImage;

        public BitmapImage ResultImage
        {
            get => _resultImage; private set { if (_resultImage != value) { _resultImage = value; OnPropertyChanged(); } }
        }

        private DieIndex _dieIndex;

        public DieIndex DieIndex
        {
            get => _dieIndex; private set { if (_dieIndex != value) { _dieIndex = value; OnPropertyChanged(); } }
        }

        private BowDetailMeasureInfoVM _bowDetailResult = null;

        public BowDetailMeasureInfoVM BowDetailResult
        {
            get => _bowDetailResult; private set { if (_bowDetailResult != value) { _bowDetailResult = value; OnPropertyChanged(); } }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
