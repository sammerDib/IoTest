using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Services;

using MvvmDialogs;

using UnitySC.Shared.UI.AutoRelayCommandExt;


namespace DeepLearningSoft48.ViewModels
{
    public enum ToolMode
    {
        NONE,
        PENCIL,
        ERASER,
        FILL,
        LINE,
        POLYGON,
        BOX,
        SELECT,
        CLEAR
    }

    /// <summary>
    /// View Model linked to ImageAnnotationToolsView.xaml
    /// Contains all annotation options, tools, defect categories ans colors.
    /// </summary>
    public class ImageAnnotationToolsViewModel : ObservableRecipient
    {
        //====================================================================
        // ViewModelLocator class's DialogService
        //====================================================================
        private readonly IDialogService _dialogService;

        //====================================================================
        // Defect Categories Collection (Label & Color)
        //====================================================================
        private readonly ObservableCollection<DefectCategoryPair> _defectCategories;
        public IEnumerable<DefectCategoryPair> DefectCategories => _defectCategories;

        public List<DefectCategoryPair> SerialisableDefectCategoriesList { get; set; }

        //====================================================================
        // Selected Color & Category Properties
        //====================================================================

        /// <summary>
        /// Selected Category in the ComboBox.
        /// Set the SelectedColor variable.
        /// </summary>
        private DefectCategoryPair _selectedCategory;
        public DefectCategoryPair SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                SelectedCategoryChanged?.Invoke();
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(CanEdit));
            }
        }

        /// <summary>
        /// Event raised when the SelectedCategory changes.
        /// </summary>
        public event Action SelectedCategoryChanged;

        /// <summary>
        /// Boolean condition to enable the Edit Button.
        /// A category has to be selected.
        /// </summary>
        public bool CanEdit => SelectedCategory != null;

        //====================================================================
        // AddNewDefectCategoryDialogViewModel Initialization
        //====================================================================

        /// <summary>
        /// Dialog opened to add a new defect category in the DefectCategories collection.
        /// </summary>
        public AddNewDefectCategoryDialogViewModel AddNewDefectCategoryDialogViewModel { get; set; }

        //====================================================================
        // Annotation & Detection Type Buttons Properties
        //====================================================================

        /// <summary>
        /// Event raised when the user switches between Object and Segmentation Detection. 
        /// </summary>
        public event Action SelectedAnnotationTypeChanged;

        /// <summary>
        /// Object Detection Button Property
        /// </summary>
        private bool _isObjChecked;
        public bool IsObjChecked
        {
            get
            {
                return _isObjChecked;
            }
            set
            {
                if (value && IsSegmChecked)
                {
                    if (CanAnnotationTypeChanged())
                    {
                        _isObjChecked = value;
                        IsSegmChecked = !value;
                    }
                }
                else
                    _isObjChecked = value;

                SelectedAnnotationTypeChanged.Invoke();
                OnPropertyChanged(nameof(IsSegmChecked));
                OnPropertyChanged(nameof(IsObjChecked));
            }
        }

        /// <summary>
        /// Segmentation Detection Button Property
        /// </summary>
        private bool _isSegmChecked;
        public bool IsSegmChecked
        {
            get
            {
                return _isSegmChecked;
            }
            set
            {
                if (value && IsObjChecked)
                {
                    if (CanAnnotationTypeChanged())
                    {
                        _isSegmChecked = value;
                        IsObjChecked = !value;
                    }
                }
                else
                    _isSegmChecked = value;

                SelectedAnnotationTypeChanged.Invoke();
                OnPropertyChanged(nameof(IsObjChecked));
                OnPropertyChanged(nameof(IsSegmChecked));
            }
        }

        //====================================================================
        // Tools Buttons Properties
        //====================================================================

        /// <summary>
        /// Main Variable
        /// </summary>
        public ToolMode SelectedToolMode { get; set; }

        /// <summary>
        /// Event raised each time the user selects or unselects a tool.
        /// </summary>
        public event Action ToolTypeChanged;

        /// <summary>
        /// Event raised when the user presses the Clear All button.
        /// Pass with it a boolean to detect whether the user confirms the clearance or not. 
        /// </summary>
        public event Action<bool> ClearAllPressed;

        /// <summary>
        /// Selection Cursor Button Property
        /// </summary>
        private bool _isCursorChecked;
        public bool IsCursorChecked
        {
            get
            {
                return _isCursorChecked;
            }
            set
            {
                _isCursorChecked = value;
                if (_isCursorChecked)
                {
                    SelectedToolMode = ToolMode.SELECT;
                    IsPencilChecked = false;
                    IsEraserChecked = false;
                    IsFillChecked = false;
                    IsBoxChecked = false;
                    IsLineChecked = false;
                    IsPolygonChecked = false;
                    IsClearAllChecked = false;
                }
                else if (SelectedToolMode == ToolMode.SELECT)
                {
                    SelectedToolMode = ToolMode.NONE;
                }
                ToolTypeChanged?.Invoke();
                OnPropertyChanged(nameof(IsCursorChecked));
            }
        }

        /// <summary>
        /// Clear All Button Property
        /// </summary>
        private bool _isClearAllChecked;
        public bool IsClearAllChecked
        {
            get
            {
                return _isClearAllChecked;
            }
            set
            {
                if (_isClearAllChecked == value)
                    return;

                _isClearAllChecked = value;

                if (_isClearAllChecked)
                {
                    SelectedToolMode = ToolMode.CLEAR;
                    IsEraserChecked = false;
                    IsPencilChecked = false;
                    IsFillChecked = false;
                    IsBoxChecked = false;
                    IsLineChecked = false;
                    IsPolygonChecked = false;
                    IsCursorChecked = false;

                    ClearAllPressed?.Invoke(false); // pressed but not actual clear (clear not confirmed or cancelled)
                    OnPropertyChanged(nameof(IsClearAllChecked));
                    ClearAll();
                    SelectedToolMode = ToolMode.NONE;
                    _isClearAllChecked = false;
                    OnPropertyChanged(nameof(IsClearAllChecked));
                }

                else if (SelectedToolMode == ToolMode.CLEAR)
                {
                    SelectedToolMode = ToolMode.NONE;
                    OnPropertyChanged(nameof(IsClearAllChecked));
                }

                ToolTypeChanged?.Invoke();
            }
        }

        /// <summary>
        /// Eraser Button Property
        /// </summary>
        private bool _isEraserChecked;
        public bool IsEraserChecked
        {
            get
            {
                return _isEraserChecked;
            }
            set
            {
                _isEraserChecked = value;
                if (_isEraserChecked)
                {
                    SelectedToolMode = ToolMode.ERASER;
                    IsPencilChecked = false;
                    IsFillChecked = false;
                    IsBoxChecked = false;
                    IsLineChecked = false;
                    IsPolygonChecked = false;
                    IsCursorChecked = false;
                    IsClearAllChecked = false;
                }
                else
                {
                    if (SelectedToolMode == ToolMode.ERASER)
                        SelectedToolMode = ToolMode.NONE;
                }
                ToolTypeChanged?.Invoke();
                OnPropertyChanged(nameof(IsEraserChecked));
            }
        }

        /// <summary>
        /// Fill tool Button Property
        /// </summary>
        private bool _isFillChecked;
        public bool IsFillChecked
        {
            get
            {
                return _isFillChecked;
            }
            set
            {
                _isFillChecked = value;
                if (_isFillChecked)
                {
                    SelectedToolMode = ToolMode.FILL;
                    IsPencilChecked = false;
                    IsEraserChecked = false;
                    IsBoxChecked = false;
                    IsLineChecked = false;
                    IsPolygonChecked = false;
                    IsCursorChecked = false;
                    IsClearAllChecked = false;
                }
                else
                {
                    if (SelectedToolMode == ToolMode.FILL)
                        SelectedToolMode = ToolMode.NONE;
                }
                ToolTypeChanged?.Invoke();
                OnPropertyChanged(nameof(IsFillChecked));
            }
        }

        /// <summary>
        /// Segmentation: Pencil (free hand line) Button Property
        /// </summary>
        private bool _isPencilChecked;
        public bool IsPencilChecked
        {
            get
            {
                return _isPencilChecked;
            }
            set
            {
                _isPencilChecked = value;
                if (_isPencilChecked)
                {
                    SelectedToolMode = ToolMode.PENCIL;
                    IsEraserChecked = false;
                    IsFillChecked = false;
                    IsBoxChecked = false;
                    IsLineChecked = false;
                    IsPolygonChecked = false;
                    IsCursorChecked = false;
                    IsClearAllChecked = false;
                }
                else
                {
                    if (SelectedToolMode == ToolMode.PENCIL)
                        SelectedToolMode = ToolMode.NONE;
                }
                ToolTypeChanged?.Invoke();
                OnPropertyChanged(nameof(IsPencilChecked));
            }
        }

        /// <summary>
        /// Segmentation: Line Button Property
        /// </summary>
        private bool _isLineChecked;
        public bool IsLineChecked
        {
            get
            {
                return _isLineChecked;
            }
            set
            {
                _isLineChecked = value;
                if (_isLineChecked)
                {
                    SelectedToolMode = ToolMode.LINE;
                    IsPencilChecked = false;
                    IsEraserChecked = false;
                    IsBoxChecked = false;
                    IsFillChecked = false;
                    IsCursorChecked = false;
                    IsPolygonChecked = false;
                    IsCursorChecked = false;
                }
                else
                {
                    if (SelectedToolMode == ToolMode.LINE)
                        SelectedToolMode = ToolMode.NONE;
                }
                ToolTypeChanged?.Invoke();
                OnPropertyChanged(nameof(IsLineChecked));
            }
        }

        /// <summary>
        /// Segmentation: Polygon Button Property
        /// </summary>
        private bool _isPolygonChecked;
        public bool IsPolygonChecked
        {
            get
            {
                return _isPolygonChecked;
            }
            set
            {
                _isPolygonChecked = value;
                if (_isPolygonChecked)
                {
                    SelectedToolMode = ToolMode.POLYGON;
                    IsPencilChecked = false;
                    IsEraserChecked = false;
                    IsLineChecked = false;
                    IsBoxChecked = false;
                    IsCursorChecked = false;
                    IsFillChecked = false;
                    IsCursorChecked = false;
                }
                else
                {
                    if (SelectedToolMode == ToolMode.POLYGON)
                        SelectedToolMode = ToolMode.NONE;
                }
                ToolTypeChanged?.Invoke();
                OnPropertyChanged(nameof(IsPolygonChecked));
            }
        }

        /// <summary>
        /// Object: Bounding Box Button Property
        /// </summary>
        private bool _isBoxChecked;
        public bool IsBoxChecked
        {
            get
            {
                return _isBoxChecked;
            }
            set
            {
                _isBoxChecked = value;
                if (_isBoxChecked)
                {
                    SelectedToolMode = ToolMode.BOX;
                    IsPencilChecked = false;
                    IsEraserChecked = false;
                    IsLineChecked = false;
                    IsFillChecked = false;
                    IsCursorChecked = false;
                    IsPolygonChecked = false;
                    IsFillChecked = false;
                }
                else
                {
                    if (SelectedToolMode == ToolMode.BOX)
                        SelectedToolMode = ToolMode.NONE;
                }
                ToolTypeChanged?.Invoke();
                OnPropertyChanged(nameof(IsBoxChecked));
            }
        }

        //====================================================================
        // SizeButton View Properties
        //====================================================================
        public Dictionary<double, string> SizePossibleValues { get; set; }

        /// <summary>
        /// Pair corresponding to the selected item in the pop up.
        /// Set the SelectedThickness variable.
        /// </summary>
        private KeyValuePair<double, string> _selectedEntry;
        public KeyValuePair<double, string> SelectedEntry
        {
            get
            {
                return _selectedEntry;
            }
            set
            {
                _selectedEntry = value;
                SelectedThickness = SelectedEntry.Key;
            }
        }

        /// <summary>
        /// Variable set by the SelectedEntry variable.
        /// </summary>
        private double _selectedThickness = 3;
        public double SelectedThickness
        {
            get
            {
                return _selectedThickness;
            }
            set
            {
                _selectedThickness = value;
                SelectedThicknessChanged?.Invoke();
            }
        }

        /// <summary>
        /// Event raised when the SelectedEntry changes.
        /// </summary>
        public event Action SelectedThicknessChanged;

        //====================================================================
        // Constructor
        //====================================================================
        public ImageAnnotationToolsViewModel()
        {
            _dialogService = new DialogService();

            _defectCategories = new ObservableCollection<DefectCategoryPair>();
            _defectCategories.CollectionChanged += DefectCategories_CollectionChanged;
            SelectedAnnotationTypeChanged += ImageAnnotationToolsViewModel_SelectedAnnotationTypeChanged;
            ClearAllPressed += ImageAnnotationToolsViewModel_ClearAllPressed;

            SerialisableDefectCategoriesList = new List<DefectCategoryPair>();

            SelectedToolMode = ToolMode.NONE;
            SizePossibleValues = new Dictionary<double, string>
            {
                { 1, "1" },
                { 2, "2" },
                { 3, "3" }, // Default value is 3px
                { 5, "5" },         // |
                { 8, "8" }          // |
            };                      // V
            _selectedEntry = SizePossibleValues.ElementAt(2);

            LoadSavedDefectCategories();
        }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Asynchronous Clear All method that would keep the Clear All button highlighted 
        /// when the confirmation dialog asking the user whether they really want to clear appears.
        /// </summary>
        public void ClearAll()
        {
            if (WaferService.SelectedWafer != null && CanWaferDefectAnnotationsBeCleared(WaferService.SelectedWafer))
            {
                WaferService.ClearAllDefectAnnotations();
                ClearAllPressed?.Invoke(true); // pressed but actual clear (clear confirmed)
            }
        }

        /// <summary>
        /// Load saved defect categories whilst handling first time usage of app whereby user never saved anything before.
        /// </summary>
        public void LoadSavedDefectCategories()
        {
            try
            {
                SerialisableDefectCategoriesList = XmlService.DeserializeListDefectCategories();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                Console.WriteLine("This must be your first time saving a DefectCategories file. Creating the file...");
                XmlService.SerializeListDefectCategories(SerialisableDefectCategoriesList);
                SerialisableDefectCategoriesList = XmlService.DeserializeListDefectCategories();
            }

            ReconstructAddedDefectCategoriesList();
        }

        /// <summary>
        /// Reconstruct the list of added defect categories.
        /// </summary>
        public void ReconstructAddedDefectCategoriesList()
        {
            if (SerialisableDefectCategoriesList != null)
            {
                foreach (DefectCategoryPair defectCategory in SerialisableDefectCategoriesList)
                {
                    if (defectCategory != null)
                        _defectCategories.Add(defectCategory);
                }
            }
        }

        /// <summary>
        /// Add a given category to the current list.
        /// </summary>
        /// <param name="defectCategory"></param>
        public void AddNewDefectCategory(DefectCategoryPair defectCategory)
        {
            if (!DefectCategories.Contains(defectCategory) && !SerialisableDefectCategoriesList.Contains(defectCategory))
            {
                _defectCategories.Add(defectCategory);
                SerialisableDefectCategoriesList.Add(defectCategory);
            }

            XmlService.SerializeListDefectCategories(SerialisableDefectCategoriesList);
        }


        /// <summary>
        /// Condition asking the user to confirm that they want to switch from an annotation type to another.
        /// </summary>
        /// <returns></returns>
        private bool CanAnnotationTypeChanged()
        {
            if (MessageBox.Show("You are going to switch of annotation type. Are you sure?", "Confirmation Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                return true;
            return false;
        }

        /// <summary>
        /// Condition asking user to confirm they really want to clear all the defect annotations on the selected wafer.
        /// </summary>
        /// <returns></returns>
        private bool CanWaferDefectAnnotationsBeCleared(Wafer selectedWafer)
        {
            if (selectedWafer.DefectsAnnotationsCollection.Count > 0)
            {
                string message = $"You are about to clear all defect annotations on the selected wafer: {selectedWafer.BaseName}. This action is irreversible, are you sure you want to proceed?";
                MessageBoxResult result = MessageBox.Show(message, "Confirmation Message", MessageBoxButton.YesNo);
                return result == MessageBoxResult.Yes;
            }
            else
            {
                string message = $"The selected wafer: {selectedWafer.BaseName} doesn't have any defect annotations to clear.";
                MessageBox.Show(message, "Confirmation Message", MessageBoxButton.OK);
            }
            return false;
        }

        //====================================================================
        // Commands
        //====================================================================

        /// <summary>
        /// Open the the dialog to add a new defect category.
        /// </summary>
        private AutoRelayCommand _addNewDefectCategoryCommand;
        public AutoRelayCommand AddNewDefectCategoryCommand
        {
            get
            {
                return _addNewDefectCategoryCommand ?? (_addNewDefectCategoryCommand = new AutoRelayCommand(
                () =>
                {
                    AddNewDefectCategoryDialogViewModel = new AddNewDefectCategoryDialogViewModel(_defectCategories);
                    var success = _dialogService.ShowDialog<Views.PopUpWindows.AddNewDefectCategoryDialog>(this, AddNewDefectCategoryDialogViewModel);
                    if (success == true)
                    {
                        try
                        {
                            // Create input pair
                            DefectCategoryPair inputPair = new DefectCategoryPair(AddNewDefectCategoryDialogViewModel.SelectedColor, AddNewDefectCategoryDialogViewModel.DefectLabel.ToUpper());

                            // Add the pair to the collection
                            AddNewDefectCategory(inputPair);

                            // Reset variables while VM is the same instance when called
                            AddNewDefectCategoryDialogViewModel.DefectCategoryFormViewModel.Clear();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            Debug.WriteLine(ex);
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// Open the the dialog to edit a new defect category.
        /// </summary>
        private AutoRelayCommand _editDefectCategoryCommand;
        public AutoRelayCommand EditDefectCategoryCommand
        {
            get
            {
                return _editDefectCategoryCommand ?? (_editDefectCategoryCommand = new AutoRelayCommand(
                () =>
                {
                    var dialogViewModel = new EditDefectCategoryDialogViewModel(SerialisableDefectCategoriesList, _defectCategories, SelectedCategory.Label, SelectedCategory.Color);

                    var success = _dialogService.ShowDialog<Views.PopUpWindows.EditDefectCategoryDialog>(this, dialogViewModel);
                    if (success == true)
                    {
                        try
                        {
                            // Update the pair to the collection
                            DefectCategoryPair pair = _defectCategories.FirstOrDefault(y => y?.Label == dialogViewModel.OldDefectLabel);

                            int currentIndex = _defectCategories.IndexOf(pair);

                            DefectCategoryPair inputPair = new DefectCategoryPair(dialogViewModel.SelectedColor, dialogViewModel.DefectLabel.ToUpper());
                            // Add the pair to the collection  
                            _defectCategories.RemoveAt(currentIndex);
                            _defectCategories.Insert(currentIndex, inputPair);

                            // Reset variables while VM is the same instance when called
                            dialogViewModel.DefectCategoryFormViewModel.Clear();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }));
            }
        }


        //====================================================================
        // Events & Dispose
        //====================================================================

        /// <summary>
        /// Event raised by the collection whenever an item is added or removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefectCategories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                SelectedCategory = e.NewItems[0] as DefectCategoryPair;

            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(DefectCategories));
        }

        /// <summary>
        /// Event raised when the annotation type has changed.
        /// </summary>
        private void ImageAnnotationToolsViewModel_SelectedAnnotationTypeChanged()
        {
            SelectedToolMode = ToolMode.NONE;

            IsPolygonChecked = false;
            IsPencilChecked = false;
            IsEraserChecked = false;
            IsLineChecked = false;
            IsBoxChecked = false;
            IsCursorChecked = false;
            IsFillChecked = false;
        }

        private void ImageAnnotationToolsViewModel_ClearAllPressed(bool isPressed)
        {
            SelectedToolMode = ToolMode.NONE;
            IsPolygonChecked = false;
            IsPencilChecked = false;
            IsEraserChecked = false;
            IsLineChecked = false;
            IsBoxChecked = false;
            IsCursorChecked = false;
            IsFillChecked = false;
        }

        /// <summary>
        /// Unsubscribe to events and free memory.
        /// </summary>
        public void Dispose()
        {
            _defectCategories.CollectionChanged -= DefectCategories_CollectionChanged;
        }
    }
}
