using System;

using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun
{
    public class RecipeRunMeasureResultVM : TabViewModelBase, IDisposable
    {
        public RecipeRunMeasureResultVM(string measureName, ResultType resType)
        {
            Title = measureName;
            ResType = resType;
            IsEnabled = false;
        }

        public void Dispose()
        {
            MeasureResult?.Dispose();
        }

        #region Properties

        private MetroResultVM _measureResult = null;

        public MetroResultVM MeasureResult
        {
            get => _measureResult; set { if (_measureResult != value) { _measureResult = value; OnPropertyChanged(); } }
        }

        private ResultType _resType;

        public ResultType ResType
        {
            get => _resType; set { if (_resType != value) { _resType = value; OnPropertyChanged(); } }
        }

        #endregion Properties
    }
}
