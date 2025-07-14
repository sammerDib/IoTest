using System;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to DefectCategoryForm.xaml component.
    /// Permits to the user to enter defect category inputs.
    /// Used in both Add & Edit dialogs.
    /// </summary>
    public class DefectCategoryFormViewModel : ObservableRecipient
    {
        //====================================================================
        // Input Properties Initialization
        //====================================================================
        private string _defectLabel;
        public string DefectLabel
        {
            get
            {
                return _defectLabel;
            }
            set
            {
                _defectLabel = value;
                OnPropertyChanged();
                DefectCategoryChanged?.Invoke();
            }
        }

        private Color _selectedColor;
        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                _selectedColor = value;
                OnPropertyChanged();
                DefectCategoryChanged?.Invoke();
            }
        }

        /// <summary>
        /// Event raised when one of the properties has changed.
        /// </summary>
        public event Action DefectCategoryChanged;

        //====================================================================
        // Constructors
        //====================================================================

        /// <summary>
        /// Constructor used by the EditDefectCategoryDialog.
        /// </summary>
        /// <param name="defectLabel"></param>
        /// <param name="selectedColor"></param>
        public DefectCategoryFormViewModel(string defectLabel, Color selectedColor)
        {
            DefectLabel = defectLabel;
            SelectedColor = selectedColor;
        }

        //====================================================================
        // Clear Method
        //====================================================================
        public void Clear()
        {
            DefectLabel = null;
            SelectedColor = Color.FromScRgb(0, 0, 0, 0);
        }
    }
}
