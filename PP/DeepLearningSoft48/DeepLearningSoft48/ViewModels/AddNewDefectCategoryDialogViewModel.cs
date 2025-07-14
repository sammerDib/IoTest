using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;

using MvvmDialogs;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Converters;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to AddNewDefectCategoryDialog.xaml 
    /// Permits to add a defect category to the ImageAnnotationTools.xaml component.
    /// </summary>
    public class AddNewDefectCategoryDialogViewModel : ObservableRecipient, IModalDialogViewModel
    {
        //====================================================================
        // Commands and DialogResult for the ViewModelLocator class
        //====================================================================
        private readonly AutoRelayCommand _addCategoryCommand;
        public ICommand AddCategoryCommand => _addCategoryCommand;

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
        /// Defect Category Form inputs.
        /// </summary>
        public string DefectLabel => DefectCategoryFormViewModel.DefectLabel;
        public System.Drawing.Color SelectedColor => ColorToSolidBrushConverter.ColorToColor(DefectCategoryFormViewModel.SelectedColor);

        /// <summary>
        /// Condition to check if we can add.
        /// </summary>
        public bool IsAddButtonEnable => !string.IsNullOrEmpty(DefectLabel) && !DefectCategoryFormViewModel.SelectedColor.Equals(Color.FromScRgb(0, 0, 0, 0));

        //====================================================================
        // Main Property: Defect Categories List
        //====================================================================
        private List<DefectCategoryPair> _defectCategories;

        //====================================================================
        // Constructor
        //====================================================================
        public AddNewDefectCategoryDialogViewModel(ObservableCollection<DefectCategoryPair> defectCategories)
        {
            _defectCategories = defectCategories.ToList();

            DefectCategoryFormViewModel = new DefectCategoryFormViewModel(string.Empty, Color.FromArgb(255,0,0,0));

            DefectCategoryFormViewModel.DefectCategoryChanged += DefectCategoryFormViewModel_DefectCategoryChanged;

            _addCategoryCommand = new AutoRelayCommand(Add);
        }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Method linked to the called command allonwing to add a category.
        /// </summary>
        private void Add()
        {
            if (CanAdd())
            {
                DialogResult = true;
                Dispose();
            }
            else
                ErrorMessage = "Label or color already used. Please choose another one.";
        }

        /// <summary>
        /// Method to ckeck if the conditions are fulfilled to add a category.
        /// </summary>
        /// <returns></returns>
        private bool CanAdd()
        {
            return !_defectCategories.Any(y => y.Label.Equals(DefectLabel.ToUpper()))
                && !_defectCategories.Any(y => y.Color.R == SelectedColor.R &&
                y.Color.G == SelectedColor.G && y.Color.B == SelectedColor.B);
        }

        //====================================================================
        // Events & Dispose
        //====================================================================
        private void DefectCategoryFormViewModel_DefectCategoryChanged()
        {
            OnPropertyChanged(nameof(DefectLabel));
            OnPropertyChanged(nameof(SelectedColor));
            OnPropertyChanged(nameof(IsAddButtonEnable));
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
