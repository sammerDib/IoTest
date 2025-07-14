using System;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.PointSelector
{
    public class WarpPointSelector : PointSelectorBase
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
            }
        }

        private LengthUnit _currentUnit = LengthUnit.Micrometer;

        public LengthUnit CurrentUnit
        {
            get { return _currentUnit; }
            set { SetProperty(ref _currentUnit, value); }
        }

        #region Event Handlers

        public event EventHandler SelectedOutputChanged;


        #endregion Event Handlers

        public void SetViewerTypeAndRaiseEvents(string output)
        {
            _selectedOutput = output;
            OnPropertyChanged(nameof(SelectedOutput));
            SelectedOutputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
