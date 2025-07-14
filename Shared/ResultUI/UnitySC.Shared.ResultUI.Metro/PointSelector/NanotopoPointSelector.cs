using System;

using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.PointSelector
{
    public class NanotopoPointSelector : PointSelectorBase
    {
        private string _selectedOutput = string.Empty;

        public string SelectedOutput
        {
            get { return _selectedOutput; }
            set
            {
                if (SetProperty(ref _selectedOutput, value))
                {
                    SelectedOutputChanged?.Invoke(this, EventArgs.Empty);
                }

                switch (value)
                {
                    case NanotopoResultVM.RoughnessOutputName:
                    case NanotopoResultVM.StepHeightOutputName:
                        CurrentUnit = LengthUnit.Nanometer; 
                        break;
                    default:
                        CurrentUnit = LengthUnit.Undefined;
                        break;
                }
            }
        }

        private LengthUnit _currentUnit = LengthUnit.Nanometer;

        public LengthUnit CurrentUnit
        {
            get { return _currentUnit; }
            set { SetProperty(ref _currentUnit, value); }
        }

        #region Event Handlers

        public event EventHandler SelectedOutputChanged;

        #endregion Event Handlers

        public void SetOutputAndRaiseEvents(string output)
        {
            _selectedOutput = output;
            OnPropertyChanged(nameof(SelectedOutput));
            SelectedOutputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
