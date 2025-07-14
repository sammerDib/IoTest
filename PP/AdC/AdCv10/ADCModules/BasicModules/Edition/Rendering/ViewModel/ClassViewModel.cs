using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Edition.Rendering.ViewModel
{
    /// <summary>
    /// ViewModel to display one defect class
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ClassViewModel : ObservableRecipient
    {

        public ClassViewModel(ClassificationViewModel classifVm, string className, bool isSelected, int? roughBinNum)
        {
            _classifVm = classifVm;
            _className = className;
            _isSelected = isSelected;
            _roughBinNum = roughBinNum;
            Defects = new ObservableCollection<DefectViewModel>();
        }

        /// <summary>
        /// Number of defects
        /// </summary>
        public int NbDefects => Defects.Count();

        /// <summary>
        /// Parent viemmodel
        /// </summary>
        private ClassificationViewModel _classifVm;

        public ClassViewModel SelectedClass
        {
            get; set;
        }

        private int? _roughBinNum;
        public int? RoughBinNum
        {
            get => _roughBinNum; set { if (_roughBinNum != value) { _roughBinNum = value; OnPropertyChanged(); } }
        }

        private string _className;
        public string ClassName
        {
            get => _className;
            set
            {
                if (_className != value)
                {
                    _className = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    _classifVm.RefreshUI();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///  Select th class without message notification
        /// </summary>
        /// <param name="isSelected"></param>
        public void SelectWithoutNotification(bool isSelected)
        {
            _isSelected = isSelected;
            OnPropertyChanged(nameof(IsSelected));
        }


        /// <summary>
        /// Class color
        /// </summary>
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    _classifVm.RefreshUI();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// List of defects for this clas
        /// </summary>
        public ObservableCollection<DefectViewModel> Defects { get; private set; }


        /// <summary>
        /// Update the number of defects 
        /// </summary>
        public void UpdateNbDefects()
        {
            OnPropertyChanged(nameof(NbDefects));
        }
    }
}
