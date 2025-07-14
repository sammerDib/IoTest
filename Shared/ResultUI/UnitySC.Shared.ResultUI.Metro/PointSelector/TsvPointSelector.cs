using System;

using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv;

namespace UnitySC.Shared.ResultUI.Metro.PointSelector
{
    public class TsvPointSelector : PointSelectorBase
    {
        #region Properties

        private TsvResultViewerType _viewerType = TsvResultViewerType.Depth;

        public TsvResultViewerType ViewerType
        {
            get { return _viewerType; }
            set
            {
                if (SetProperty(ref _viewerType, value))
                {
                    ViewerTypeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Event Handlers

        public event EventHandler ViewerTypeChanged;

        #endregion Event Handlers

        public void RaiseViewerTypeChangedEvents()
        {
            OnPropertyChanged(nameof(ViewerType));
            ViewerTypeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
