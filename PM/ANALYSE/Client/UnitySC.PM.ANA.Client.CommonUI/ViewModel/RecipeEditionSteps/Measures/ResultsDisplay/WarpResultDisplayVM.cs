using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using MvvmDialogs;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class WarpResultDisplayVM : StepBaseVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public WarpResultDisplayVM(WarpPointResult warpPointResult, WarpSettings warpSettings, string resultFolderPath, DieIndex dieIndex)
        {
            WarpResult = warpPointResult;
            WarpSettings = warpSettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;

            UpdateWarpDetailResult();
        }

        private WarpPointResult _warpResult = null;

        public WarpPointResult WarpResult
        {
            get => _warpResult;
            private set
            {
                if (_warpResult != value)
                {
                    _warpResult = value;
                    UpdateWarpDetailResult();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateWarpDetailResult()
        {
            if ((WarpSettings is null) || (WarpResult is null) || string.IsNullOrEmpty(ResultFolderPath))
                return;

            WarpDetailResult = new WarpDetailMeasureInfoVM();
            WarpDetailResult.Settings = new WarpResultSettings();
            WarpDetailResult.Settings.IsSurfaceWarp = WarpSettings.IsSurfaceWarp;
            WarpDetailResult.Settings.WarpMax = WarpSettings.WarpMax;
            WarpDetailResult.Output = EnumUtils.GetAttribute<DescriptionAttribute>(WarpViewerType.WARP).Description;

            // Warp result not available
            if (WarpResult.Datas.IsNullOrEmpty())
            {
                WarpDetailResult.GlobalState = UnitySC.Shared.Format.Metro.MeasureState.NotMeasured;
            }
            else
            {
                var warpTotalData = (WarpResult.Datas.First() as WarpTotalPointData) ?? new WarpTotalPointData()
                {
                    State = UnitySC.Shared.Format.Metro.MeasureState.NotMeasured,
                    Warp = 0.Micrometers(),
                    TTV = 0.Micrometers(),
                    Message = "Empty warp data",
                };
                WarpDetailResult.QualityScore = warpTotalData.QualityScore;
                WarpDetailResult.WarpResultLength = warpTotalData.Warp;
                WarpDetailResult.GlobalState = warpTotalData.State;
                // TTV result
                if (!WarpSettings.IsSurfaceWarp)
                {
                    WarpDetailResult.TTVResult = LengthToStringConverter.ConvertToString(warpTotalData.TTV, 3, true, "-", LengthUnit.Micrometer);
                }
            }

            WarpDetailResult.Digits = 3;
            WarpResult.GenerateStats();
            WarpDetailResult.Point = WarpResult;

            if (!(DieIndex is null))
            {
                WarpDetailResult.DieIndex = $"Column  {DieIndex.Column}    Row  {DieIndex.Row}";
            }

            OnPropertyChanged(nameof(WarpDetailResult));
        }

        private WarpSettings _warpSettings = null;

        public WarpSettings WarpSettings
        {
            get => _warpSettings;
            private set => SetProperty(ref _warpSettings, value);
        }

        private string _resultFolderPath;

        public string ResultFolderPath
        {
            get => _resultFolderPath;
            private set => SetProperty(ref _resultFolderPath, value);
        }

        private BitmapImage _resultImage;

        public BitmapImage ResultImage
        {
            get => _resultImage;
            private set => SetProperty(ref _resultImage, value);
        }

        private DieIndex _dieIndex;

        public DieIndex DieIndex
        {
            get => _dieIndex;
            private set => SetProperty(ref _dieIndex, value);
        }

        private WarpDetailMeasureInfoVM _warpDetailResult = null;

        public WarpDetailMeasureInfoVM WarpDetailResult
        {
            get => _warpDetailResult;
            private set => SetProperty(ref _warpDetailResult, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
