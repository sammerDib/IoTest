using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.PM.DMT.Service.Interface;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class SelectableDeadPixel: ObservableObject
    {
        private bool _isSelected;

        public DeadPixel AssociatedDeadPixel { get; set; }

        public SelectableDeadPixel(DeadPixel deadPixel)
        {
            AssociatedDeadPixel = deadPixel;
        }

        public bool IsSelected
        {
            get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }
    }
}
