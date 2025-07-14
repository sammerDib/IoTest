using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Services;

using MvvmDialogs;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Converters;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to EditDefectCategoryDialog.xaml 
    /// Permits to edit or remove a defect category from the ImageAnnotationTools.xaml component.
    /// </summary>
    public class EditDefectCategoryDialogViewModel : ObservableRecipient, IModalDialogViewModel
    {
        //====================================================================
        // Commands and DialogResult for the ViewModelLocator class
        //====================================================================
        private readonly AutoRelayCommand _editCategoryCommand;
        public ICommand EditCategoryCommand => _editCategoryCommand;

        private readonly AutoRelayCommand _deleteCategoryCommand;
        public ICommand DeleteCategoryCommand => _deleteCategoryCommand;

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        // Error handling properties
        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasErrorMessage));

            }
        }
        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        //====================================================================
        // Defect Category Form View Model Initialization
        //====================================================================

        /// <summary>
        /// This View Model is used both in this VM and in the edit VM.
        /// It permits to the user to enter inputs.
        /// </summary>
        public DefectCategoryFormViewModel DefectCategoryFormViewModel { get; set; }

        /// <summary>
        /// Defect Label before changes.
        /// </summary>
        public string OldDefectLabel { get; set; }

        /// <summary>
        /// Defect Category Form new label input.
        /// </summary>
        public string DefectLabel => DefectCategoryFormViewModel.DefectLabel;

        /// <summary>
        /// Defect Color before changes.
        /// </summary>
        public System.Drawing.Color OldDefectColor { get; set; }

        /// <summary>
        /// Defect Category Form new color input.
        /// </summary>
        public System.Drawing.Color SelectedColor => ColorToSolidBrushConverter.ColorToColor(DefectCategoryFormViewModel.SelectedColor);

        /// <summary>
        /// Check if we can edit.
        /// </summary>
        public bool IsEditButtonEnable => !string.IsNullOrEmpty(DefectLabel) && !DefectCategoryFormViewModel.SelectedColor.Equals(Color.FromScRgb(0, 0, 0, 0));

        //====================================================================
        // Defect Categories Collection Initialization
        //====================================================================

        /// <summary>
        /// Collection from ImageAnnotationToolsViewModel.
        /// </summary>
        private ObservableCollection<DefectCategoryPair> _defectCategoryPairs;
        public List<DefectCategoryPair> SerialisableDefectCategoriesList { get; set; }


        //------------------------------------------------------------
        // Constructor
        //------------------------------------------------------------
        public EditDefectCategoryDialogViewModel(List<DefectCategoryPair> serialisableDefectCategoriesList, ObservableCollection<DefectCategoryPair> defectCategoryPairsCollection, string defectLabel, System.Drawing.Color selectedColor)
        {
            _defectCategoryPairs = defectCategoryPairsCollection;
            DefectCategoryFormViewModel = new DefectCategoryFormViewModel(defectLabel, ColorToSolidBrushConverter.ColorToColor(selectedColor));

            OldDefectLabel = DefectLabel;
            OldDefectColor = SelectedColor;

            DefectCategoryFormViewModel.DefectCategoryChanged += DefectCategoryFormViewModel_DefectCategoryChanged;

            _editCategoryCommand = new AutoRelayCommand(Edit);
            _deleteCategoryCommand = new AutoRelayCommand(Delete);

            SerialisableDefectCategoriesList = serialisableDefectCategoriesList;
        }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Method linked to the called command allonwing to edit a category.
        /// </summary>
        private void Edit()
        {
            if (CanEdit())
            {
                DialogResult = true;
                // Delete the DefectCategory to later on replace it with a new one
                DefectCategoryPair modifiedDefectCategoryPair = new DefectCategoryPair(SelectedColor, DefectLabel.ToUpper());
                SerialisableDefectCategoriesList.RemoveAll(item => item.Label == OldDefectLabel);
                XmlService.SerializeListDefectCategories(SerialisableDefectCategoriesList);

                // Plug-in the new DefectCategory if its label doesn't already exist in the list
                bool labelExists = false;
                foreach (var categoryPair in SerialisableDefectCategoriesList)
                {
                    if (categoryPair.Label == modifiedDefectCategoryPair.Label)
                    {
                        labelExists = true;
                        break;
                    }
                }

                if (!labelExists)
                {
                    SerialisableDefectCategoriesList.Add(modifiedDefectCategoryPair);
                    XmlService.SerializeListDefectCategories(SerialisableDefectCategoriesList);
                }

                Dispose();
            }
            else
                ErrorMessage = "Label or color already used. Please choose another one.";
        }

        /// <summary>
        /// Check if the required conditions are fulfilled to edit a category.
        /// </summary>
        /// <returns></returns>
        private bool CanEdit()
        {
            // If no change has been applied.
            if (DefectLabel.ToUpper().Equals(OldDefectLabel) && SelectedColor.Equals(OldDefectColor))
                return true;

            // If only the label has changed.
            if (DefectLabel.ToUpper().Equals(OldDefectLabel) && !SelectedColor.Equals(OldDefectColor))
                return !_defectCategoryPairs.Any(y => y.Color.R == SelectedColor.R &&
                y.Color.G == SelectedColor.G && y.Color.B == SelectedColor.B);

            // If only the color has changed.
            if (SelectedColor.Equals(OldDefectColor) && !DefectLabel.ToUpper().Equals(OldDefectLabel))
                return !_defectCategoryPairs.Any(y => y.Label.Equals(DefectLabel.ToUpper()));

            // If both have changed.
            return !_defectCategoryPairs.Any(y => y.Label.Equals(DefectLabel.ToUpper()))
                && !_defectCategoryPairs.Any(y => y.Color.R == SelectedColor.R &&
                y.Color.G == SelectedColor.G && y.Color.B == SelectedColor.B);
        }

        /// <summary>
        /// Method linked to the called command allonwing to delete a category.
        /// </summary>
        private void Delete()
        {
            try
            {
                // Update the pair to the collection
                DefectCategoryPair pairToBeDeleted = _defectCategoryPairs.FirstOrDefault(y => y?.Label == OldDefectLabel);

                SerialisableDefectCategoriesList.RemoveAll(item => item.Label == pairToBeDeleted.Label);
                XmlService.SerializeListDefectCategories(SerialisableDefectCategoriesList);

                // Add the pair to the collection
                _defectCategoryPairs.Remove(pairToBeDeleted);

                // Reset variables while VM is the same instance when called
                DefectCategoryFormViewModel.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            DialogResult = false;
        }

        //====================================================================
        // Events & Dispose
        //====================================================================
        private void DefectCategoryFormViewModel_DefectCategoryChanged()
        {
            OnPropertyChanged(nameof(DefectLabel));
            OnPropertyChanged(nameof(SelectedColor));
            OnPropertyChanged(nameof(IsEditButtonEnable));
        }

        /// <summary>
        /// Unsubscribe to events and free memory.
        /// </summary>
        public void Dispose()
        {
            DefectCategoryFormViewModel.DefectCategoryChanged -= DefectCategoryFormViewModel_DefectCategoryChanged;
        }
    }
}
