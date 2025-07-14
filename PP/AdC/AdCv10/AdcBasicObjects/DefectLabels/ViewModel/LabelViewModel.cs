using CommunityToolkit.Mvvm.ComponentModel;

namespace AdcBasicObjects.DefectLabels.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class LabelViewModel : ObservableRecipient
    {
        public DefectLabelStoreViewModel Parent { get; set; }

        public LabelViewModel(DefectLabelStoreViewModel parent)
        {
            Parent = parent;
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                if (value == _hasError)
                    return;
                _hasError = value;
                OnPropertyChanged();
            }
        }

        private string _validDefectLabel;
        private string _defectLabel;
        public string DefectLabel
        {
            get { return _defectLabel; }
            set
            {
                if (value == _defectLabel)
                    return;
                string oldlabel = _validDefectLabel;
                if (oldlabel != null)
                {
                    bool valid = Parent.DefectLabelStore.RenameLabel(oldlabel, value);
                    HasError = !valid;
                    if (valid)
                        _validDefectLabel = value;
                    _defectLabel = value;
                    Parent.Validate();
                }
                else
                {
                    _validDefectLabel = _defectLabel = value;
                }

                OnPropertyChanged();
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value == _isEnabled)
                    return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;
                _isSelected = value;
                if (_isSelected)
                    Parent.NbSelectedItems++;
                else
                    Parent.NbSelectedItems--;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return "{" + DefectLabel + "}";
        }
    }
}
