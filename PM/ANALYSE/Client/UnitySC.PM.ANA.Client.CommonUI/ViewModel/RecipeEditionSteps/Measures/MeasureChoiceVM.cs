using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class MeasureChoiceVM : ObservableObject
    {
        public MeasureChoiceVM(MeasureType type, string name, string description, BitmapSource icon)
        {
            Type = type;
            Name = name;
            Description = description;
            Icon = icon;
        }

        private MeasureType _type;

        public MeasureType Type
        {
            get => _type; set { if (_type != value) { _type = value; OnPropertyChanged(); } }
        }

        private string _name;

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private string _description;

        public string Description
        {
            get => _description; set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }

        private BitmapSource _icon;

        public BitmapSource Icon
        {
            get => _icon; set { if (_icon != value) { _icon = value; OnPropertyChanged(); } }
        }
    }
}
