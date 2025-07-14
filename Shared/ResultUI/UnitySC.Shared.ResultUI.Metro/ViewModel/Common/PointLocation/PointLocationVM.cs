using System.Collections.Generic;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Format.Metro;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation
{
    public class PointLocationVM : ObservableObject
    {
        private readonly PointsLocationVM _pointsLocationVM;

        #region Properties

        public List<MeasurePointResult> Points { get; } = new List<MeasurePointResult>();

        public double PosXDieRelative { get; }

        public double PosYDieRelative { get; }

        public string Name { get; }

        public int SiteId { get; }

        private bool? _isSelected = true;

        public bool? IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    _pointsLocationVM.UpdateCheckedPoints(Points, value);
                }
            }
        }

        #endregion

        #region Ctor

        public PointLocationVM(PointsLocationVM pointsLocationVM, double posXDieRelative, double posYDieRelative, int siteId, string name)
        {
            _pointsLocationVM = pointsLocationVM;
            PosXDieRelative = posXDieRelative;
            PosYDieRelative = posYDieRelative;
            SiteId = siteId;
            Name = name;

            ToggleSelectionCommand = new AutoRelayCommand(ToggleSelectionCommandExecute);
        }

        #endregion

        #region Commands

        public ICommand ToggleSelectionCommand { get; }

        private void ToggleSelectionCommandExecute()
        {
            switch (IsSelected)
            {
                case true:
                    IsSelected = false;
                    break;
                case false:
                    IsSelected = true;
                    break;
                case null:
                    IsSelected = false;
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void SelectWithoutNotification(bool? isSelected)
        {
            _isSelected = isSelected;
            OnPropertyChanged(nameof(IsSelected));
        }

        public void AddPoint(MeasurePointResult point)
        {
            Points.Add(point);
        }

        #endregion
    }
}
