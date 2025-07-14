using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Format.Metro;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType
{
    public class MeasureTypeCategoryVM : ObservableObject
    {
        private readonly MeasureTypeCategoriesVM _measureTypeCategories;

        #region Properties

        public MeasureState MeasureState { get; }

        private bool? _isSelected = false;

        public bool? IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected.Equals(value)) return;
                _isSelected = value;
                OnPropertyChanged();
                _measureTypeCategories.OnChangeCategorySelection(this);
            }
        }

        public string HumanizedCategoryName { get => MeasureState.ToHumanizedString(); }

        public int PointsNumber { get; set; }

        public int CurrentCount { get; set; }

        public bool IsEnabled => PointsNumber > 0;

        #endregion

        #region Commands

        public ICommand ToggleSelectionCommand { get; }

        private void ToggleSelectionCommandExecute()
        {
            if (IsSelected == true)
            {
                IsSelected = false;
                return;
            }

            if (IsSelected == false)
            {
                IsSelected = true;
                return;
            }
            if (IsSelected == null) IsSelected = false;
        }

        #endregion

        public MeasureTypeCategoryVM(MeasureTypeCategoriesVM measureTypeCategories, MeasureState measureState)
        {
            _measureTypeCategories = measureTypeCategories;
            MeasureState = measureState;

            ToggleSelectionCommand = new AutoRelayCommand(ToggleSelectionCommandExecute);
        }

        #region Public Methods

        public void SelectWithoutNotification(bool? isSelected)
        {
            _isSelected = isSelected;
            OnPropertyChanged(nameof(IsSelected));
        }

        #endregion
    }
}
