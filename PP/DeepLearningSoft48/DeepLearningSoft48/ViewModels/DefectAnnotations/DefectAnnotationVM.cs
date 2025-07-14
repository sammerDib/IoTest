using System;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;

namespace DeepLearningSoft48.ViewModels.DefectAnnotations
{
    public abstract class DefectAnnotationVM : ObservableRecipient
    {
        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set { if (_isVisible != value) { _isVisible = value; OnPropertyChanged(); } }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                IsSelectedChanged.Invoke();
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public event Action IsSelectedChanged = delegate { };

        private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        private double _width;
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private double _originX;
        public double OriginX
        {
            get { return _originX; }
            set
            {
                _originX = value;
                OnPropertyChanged(nameof(OriginX));
            }
        }

        private double _originY;
        public double OriginY
        {
            get { return _originY; }
            set
            {
                _originY = value;
                OnPropertyChanged(nameof(OriginY));
            }
        }
        public DefectCategoryPair Category { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }

        public double GetArea() { return Height * Width; }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of DefectAnnotationVM objects.
        /// Notably useful when we want to check whether an ObservableCollection of DefectAnnotationVMs contains a specific DefectAnnotationVM object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DefectAnnotationVM daVM = (DefectAnnotationVM)obj;
            return OriginX.Equals(daVM.OriginX)
                && OriginY.Equals(daVM.OriginY)
                && Width.Equals(daVM.Width)
                && Height.Equals(daVM.Height)
                && Category.Equals(daVM.Category)
                && Source.Equals(daVM.Source)
                && Type.Equals(daVM.Type);
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of DefectAnnotationVM objects.
        /// Notably useful when we want to check whether an ObservableCollection of DefectAnnotationVMs contains a specific DefectAnnotationVM object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + OriginX.GetHashCode();
                hash = hash * 23 + OriginY.GetHashCode();
                hash = hash * 23 + Width.GetHashCode();
                hash = hash * 23 + Height.GetHashCode();
                hash = hash * 23 + Category.GetHashCode();
                hash = hash * 23 + Source.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();
                return hash;
            }
        }
    }

}
