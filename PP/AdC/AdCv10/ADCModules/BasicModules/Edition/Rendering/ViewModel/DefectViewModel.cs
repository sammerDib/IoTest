using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Edition.Rendering.ViewModel
{
    /// <summary>
    /// ViewModel to display a defect
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DefectViewModel : ObservableRecipient
    {
        /// <summary>
        /// Visual Width
        /// </summary>
		public double Width { get; set; }

        /// <summary>
        /// Visual Height
        /// </summary>
		public double Height { get; set; }

        public int Id { get; internal set; }


        /// <summary>
        /// Defect Class 
        /// </summary>
		private ClassViewModel _classVM;
        public ClassViewModel ClassVM
        {
            get => _classVM;
            set
            {
                if (_classVM != value)
                {
                    _classVM = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///  Is visibile in view
        /// </summary>
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible; set { if (_isVisible != value) { _isVisible = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Rough Bin Number
        /// </summary>
        private int? _roughBinNum;
        public int? RoughBinNum
        {
            get => _roughBinNum; set { if (_roughBinNum != value) { _roughBinNum = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Is selected
        /// </summary>
        private bool _isSelected;

        public DefectViewModel(DefectResult defect, ClassViewModel classViewModel)
        {
            Id = defect.Id;
            MicronRect = defect.MicronRect;
            _classVM = classViewModel;
            _roughBinNum = defect.RoughBinNum;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Top position in canvas
        /// </summary>
        private double _topPostion;
        public double TopPosition
        {
            get => _topPostion; set { if (_topPostion != value) { _topPostion = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Left position in canvas
        /// </summary>
        private double _leftPosition;
        public double LeftPosition
        {
            get => _leftPosition; set { if (_leftPosition != value) { _leftPosition = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Real defect size 
        /// </summary>
		public RectangleF MicronRect { get; internal set; }
    }
}
