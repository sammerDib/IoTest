using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class ChosenMeasureVM : ObservableObject
    {
        public ChosenMeasureVM(MeasureType type, string name, bool isActive = true)
        {
            Type = type;
            Name = name; 
            PreviousName = name;
            IsActive = isActive;
        }

        private MeasureType _type;

        public MeasureType Type
        {
            get => _type; set { if (_type != value) { _type = value; OnPropertyChanged(); } }
        }

        private string _name;

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value.TrimStart().TrimEnd(); OnPropertyChanged(); } }
        }

        private string _previousName;

        public string PreviousName
        {
            get => _previousName; set { if (_previousName != value) { _previousName = value; OnPropertyChanged(); } }
        }
        private bool _isActive = true;

        public bool IsActive
        {
            get => _isActive; set { if (_isActive != value) { _isActive = value; OnPropertyChanged(); } }
        }

        private bool _isSelected = false;

        public bool IsSelected
        {
            get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }
    }
}
