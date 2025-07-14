using System;

namespace UnitySC.Shared.ResultUI.Metro.PointSelector
{
    public class ThicknessPointSelector : PointSelectorBase
    {
        private string _selectedLayer = string.Empty;

        public string SelectedLayer
        {
            get { return _selectedLayer; }
            set
            {
                if (SetProperty(ref _selectedLayer, value))
                {
                    SelectedLayerChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #region Event Handlers

        public event EventHandler SelectedLayerChanged;

        #endregion Event Handlers

        public void SetLayerAndRaiseEvents(string output)
        {
            _selectedLayer = output;
            OnPropertyChanged(nameof(SelectedLayer));
            SelectedLayerChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
