
using System.Collections.ObjectModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class DieIndexWithSelectionVM : ObservableObject
    {
        private DieIndex _index = null;

        public DieIndex Index
        {
            get => _index; set { if (_index != value) { _index = value; OnPropertyChanged(); } }
        }

        private bool _isSelected = false;

        public ObservableCollection<DieIndexWithSelectionVM> AllDies { get; set; }

        public DieIndexWithSelectionVM(DieIndex dieIndex, bool isSelected, ObservableCollection<DieIndexWithSelectionVM> allDies)
        {
            Index = dieIndex;
            IsSelected = isSelected;
            AllDies = allDies;
        }

        public bool IsSelected
        {
            get => _isSelected; 
            set 
            { if (_isSelected != value) 
            { 
                    _isSelected = value;
                    if (_isSelected && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        SelectWithPrevious();
                    OnPropertyChanged(); 
            } 
            }
        }

        public void Select()
        {
            _isSelected = true;
            OnPropertyChanged(nameof(IsSelected));
        }

        public void SelectWithPrevious()
        {
            _isSelected = true;
            // We find the die index
            var index=AllDies.IndexOf(this);

            index--;

            while ((index > -1) && !AllDies[index].IsSelected)
            {
                AllDies[index].Select();
                index--;
            }
            OnPropertyChanged(nameof(IsSelected));
        }





    }
}
