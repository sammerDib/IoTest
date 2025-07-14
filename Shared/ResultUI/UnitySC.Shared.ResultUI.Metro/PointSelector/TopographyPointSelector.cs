using System;

namespace UnitySC.Shared.ResultUI.Metro.PointSelector
{
    public class TopographyPointSelector : PointSelectorBase
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
