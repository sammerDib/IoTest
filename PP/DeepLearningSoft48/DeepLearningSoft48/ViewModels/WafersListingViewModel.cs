using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Services;
using DeepLearningSoft48.Utils;
using DeepLearningSoft48.Utils.Enums;

using UnitySC.Shared.LibMIL;

using Microsoft.Win32;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;

using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to WafersListingView.xaml
    /// Permits to display the of wafers contained in the selected folder.
    /// Corresponds to the top-left located component of the application.
    /// </summary>
    public class WafersListingViewModel : ObservableRecipient
    {
        //====================================================================
        // Browse Folder, Open and Save file properties
        //====================================================================
        private DialogOwnerService _dialogOwnerService;
        private FolderBrowserDialogSettings _folderBrowserDialogSettings;
        private OpenFileDialog _openFileDialog = new OpenFileDialog();
        private SaveFileDialog _saveFileDialog = new SaveFileDialog();

        //====================================================================
        // Properties
        //====================================================================

        /// <summary>
        /// Event that the AnnotateWaferLayerViewModle subscribes to, in order to know it's time to clear the DefectsAnnotation collection when a dismiss has been confirmedly pressed.
        /// </summary>
        public static event Action<bool> DismissPressed;

        /// <summary>
        /// Variable to know if we are in the LearningTab or the TestTab.
        /// </summary>
        private TabType CurrentTabType { get; set; }

        /// <summary>
        /// Variable linked to the OpenFolder command to display the progress bar.
        /// </summary>
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged((nameof(IsLoading)));
                OnPropertyChanged((nameof(HasSelectedFolder)));
            }
        }

        /// <summary>
        /// Variable used to determine whether or not a Progress File (saved/loadeed) is present. If it's the case, it'll display the respective file path. 
        /// (bound to BooleanVisibiltyConverter in LearningTabView.xaml)
        /// </summary>
        private bool _isProgressFilePresent;
        public bool IsProgressFilePresent
        {
            get
            {
                return _isProgressFilePresent;
            }
            set
            {
                if (_isProgressFilePresent != value)
                {
                    _isProgressFilePresent = value;
                    OnPropertyChanged(nameof(IsProgressFilePresent));

                }
            }
        }

        /// <summary>
        /// Variable used to display the progress file path (the Wafer DefectAnnotations xml file). 
        /// (in LearningTabView.xaml)
        /// </summary>
        private string _progressFileName;
        public string ProgressFileName
        {
            get { return _progressFileName; }
            set
            {
                if (_progressFileName != value)
                {
                    _progressFileName = value;
                    OnPropertyChanged(nameof(ProgressFileName));
                }
            }
        }

        //====================================================================
        // Main Tabs ViewModels
        //==================================================================== 
        private readonly LearningTabViewModel _learningTabViewModel;
        private readonly TestTabViewModel _testTabViewModel;

        public List<Wafer> Wafers;

        //====================================================================
        // Selected Wafer Fields Initialization
        //====================================================================
        public Wafer SelectedWafer;

        public WafersListingItemViewModel SelectedWaferListingItemViewModel
        {
            get
            {
                return _wafersListingItemViewModels.FirstOrDefault(y => y.Wafer?.BaseName == SelectedWafer?.BaseName);
            }
            set
            {
                if (CurrentTabType == TabType.Learning)
                {
                    // Whenever the selected item changes on the view, we grab the linked selected item view model and also the underline Wafer and paste it to the store
                    _learningTabViewModel.SelectedWafer = value?.Wafer;
                    WaferService.SelectedWafer = value?.Wafer;
                }
                if (CurrentTabType == TabType.Test)
                {
                    _testTabViewModel.SelectedWafer = value?.Wafer;
                    WaferService.SelectedWafer = value?.Wafer;
                }
            }
        }

        //====================================================================
        // Folder Path Initialization
        //====================================================================
        private string _folderPath;
        public string FolderPath
        {
            get
            {
                return _folderPath;
            }
            set
            {
                _folderPath = value;
                OnPropertyChanged((nameof(FolderPath)));
            }
        }

        private string _newFolderPath;
        public string NewFolderPath
        {
            get
            {
                return _newFolderPath;
            }
            set
            {
                _newFolderPath = value;
                OnPropertyChanged((nameof(NewFolderPath)));
            }
        }


        /// <summary>
        /// Check whether a folder has been selected.
        /// If the user has selected a folder, returns true.
        /// </summary>
        public bool HasSelectedFolder => FolderPath != null && !IsLoading;

        //====================================================================
        // Wafers VM Collection Initialization
        //====================================================================
        private readonly ObservableCollection<WafersListingItemViewModel> _wafersListingItemViewModels;
        public IEnumerable<WafersListingItemViewModel> WafersListingItemViewModels => _wafersListingItemViewModels;


        //====================================================================-
        // Constructors
        //====================================================================-
        /// <summary>
        /// Used bu the LearningTabViewModel.
        /// </summary>
        /// <param name="learningTabViewModel"></param>
        /// <param name="tabType"></param>
        public WafersListingViewModel(LearningTabViewModel learningTabViewModel, TabType tabType)
        {
            _dialogOwnerService = new DialogOwnerService(this);
            _folderBrowserDialogSettings = new FolderBrowserDialogSettings();

            CurrentTabType = tabType;

            _learningTabViewModel = learningTabViewModel;
            SelectedWafer = _learningTabViewModel.SelectedWafer;
            Wafers = _learningTabViewModel.Wafers;

            _wafersListingItemViewModels = new ObservableCollection<WafersListingItemViewModel>();

            _learningTabViewModel.SelectedWaferChanged += WafersStore_SelectedWaferChanged; //How to unregister?

            _wafersListingItemViewModels.CollectionChanged += WafersListingItemViewModels_CollectionChanged;
        }

        /// <summary>
        /// Used to load the deserialised list of wafers if it already exists.
        /// </summary>
        public async Task<Exception> LoadSavedWafers(string filename)
        {
            try
            {
                (WaferService.DeserialisedFolderPath, WaferService.DeserialisedWafers) = XmlService.DeserializeListWafers(filename);
                if (WaferService.DeserialisedFolderPath != null)
                {
                    NewFolderPath = WaferService.DeserialisedFolderPath;
                    FolderPath = NewFolderPath;
                }

                if (FolderPath != null && FolderPath.Length != 0)
                    await CreateWafers(FolderPath);

                return null; // No exception occured
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().ToString());
                return ex; // Return the caught exception
            }
        }

        /// <summary>
        /// Used bu the LearningTabViewModel.
        /// </summary>
        /// <param name="testTabViewModel"></param>
        /// <param name="tabType"></param>
        public WafersListingViewModel(TestTabViewModel testTabViewModel, TabType tabType)
        {
            _dialogOwnerService = new DialogOwnerService(this);
            _folderBrowserDialogSettings = new FolderBrowserDialogSettings();

            CurrentTabType = tabType;
            _testTabViewModel = testTabViewModel;
            SelectedWafer = _testTabViewModel.SelectedWafer;
            _wafersListingItemViewModels = new ObservableCollection<WafersListingItemViewModel>();

            _testTabViewModel.SelectedWaferChanged += WafersStore_SelectedWaferChanged; //How to unregister?

            _wafersListingItemViewModels.CollectionChanged += WafersListingItemViewModels_CollectionChanged;
        }


        //====================================================================-
        // Commands
        //====================================================================-

        /// <summary>
        /// Command to save a Wafer DefectAnnotations (.xml) file.
        /// </summary>
        private AutoRelayCommand _saveCommand;
        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
                () =>
                {
                    try
                    {
                        _saveFileDialog.Filter = "Xml files (*.xml)|*.xml";
                        if (_saveFileDialog.ShowDialog() == true)
                        {
                            WaferService.SaveProgress(_saveFileDialog.FileName);
                            ProgressFileName = _saveFileDialog.FileName;
                            IsProgressFilePresent = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Debug.WriteLine(ex);
                    }
                }));
            }
        }

        /// <summary>
        /// Command to load a Wafer DefectAnnotations (.xml) file.
        /// </summary>
        private AutoRelayCommand _loadCommand;
        public AutoRelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ?? (_loadCommand = new AutoRelayCommand(
                () =>
                {
                    _openFileDialog.Filter = "Xml files (*.xml)|*.xml";
                    if (_openFileDialog.ShowDialog() == true)
                    {
                        if (FolderPath != null) // If a previous Wafer DefectAnnotations file is loaded
                        {
                            MessageBoxResult result = MessageBox.Show("You're about to dismiss the file you're currently working on. All unsaved changes will be lost. " +
                                "This action is irreversible, are you sure you want to proceed?", "WARNING: Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                            if (result == MessageBoxResult.Yes)
                            {
                                ClearAll();
                                LoadWaferFile();
                            }
                            else
                                Console.WriteLine("Operation aborted. You chose not to load a new file, because you would rather make sure your progress is saved first.");
                        }
                        else if (FolderPath == null) // If no Wafer DefectAnnotations file is currently loaded
                            LoadWaferFile();
                    }
                }));
            }
        }

        /// <summary>
        /// Command to dismiss the current Wafer DefectAnnotations (.xml) file.
        /// </summary>
        private AutoRelayCommand _dismissCommand;
        public AutoRelayCommand DismissCommand
        {
            get
            {
                return _dismissCommand ?? (_dismissCommand = new AutoRelayCommand(
                () =>
                {
                    try
                    {
                        if (FolderPath != null && FolderPath.Length != 0)
                        {
                            if (MessageBox.Show("You're about to dismiss the file you're currently working on. All unsaved changes will be lost. This action is irreversible, are you sure you want to proceed?",
                                "WARNING: Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                                ClearAll();
                            DismissPressed?.Invoke(false); // pressed but not clear (not confirmed)
                        }
                        else
                        {
                            Console.WriteLine("Nothing to dismiss.");
                            MessageBox.Show("Nothing to dismiss: you don't have any Wafer DefectAnnotations file loaded into the app.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Debug.WriteLine(ex);
                    }
                }));
            }
        }

        /// <summary>
        /// Command to open a the file dialog to select a folder.
        /// </summary>
        private AutoRelayCommand _openFolderCommand;
        public AutoRelayCommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand = new AutoRelayCommand(
                async () =>
                {
                    try
                    {
                        // Open folder dialog
                        if (_dialogOwnerService.ShowFolderBrowserDialog(_folderBrowserDialogSettings) == true)
                        {
                            Debug.WriteLine("The following folder has been selected: " + _folderBrowserDialogSettings.SelectedPath);

                            // if not first time, warn users before they decide to proceed, and if confirmed, clear all before setting new FolderPath
                            if (FolderPath != null)
                            {
                                MessageBoxResult result = MessageBox.Show("You're about to dismiss the file you're currently working on. All unsaved changes will be lost. " +
                                "This action is irreversible, are you sure you want to proceed?", "WARNING: Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                                if (result == MessageBoxResult.Yes)
                                {
                                    ClearAll();
                                    FolderPath = _folderBrowserDialogSettings.SelectedPath;
                                    WaferService.DeserialisedFolderPath = FolderPath;
                                }
                                else
                                    Console.WriteLine("Operation aborted. You chose not to load a new file, because you would rather make sure your progress is saved first.");
                            }
                            else if (FolderPath == null)
                            {
                                FolderPath = _folderBrowserDialogSettings.SelectedPath;
                                WaferService.DeserialisedFolderPath = FolderPath;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Debug.WriteLine(ex);
                    }

                    // Gets all the wafers from the selected path.
                    await CreateWafers(FolderPath);

                    IsLoading = false;
                }));
            }
        }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Loads the respective Wafer DefectAnnotations (.xml) file, whilst handling ProgressFileName, IsProgressFilePresent and possible exceptions.
        /// </summary>
        public async void LoadWaferFile()
        {
            var exception = await LoadSavedWafers(_openFileDialog.FileName);

            if (exception != null)
            {
                Console.WriteLine(exception.Message);
                MessageBox.Show("The file you selected is invalid, please select a valid Wafers DefectAnnotation (.xml) file. It should begin with: " +
                    "\n<Wafers xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"> ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ProgressFileName = _openFileDialog.FileName;
                IsProgressFilePresent = true;
            }
        }


        /// <summary>
        /// Resets previously loaded content, used before loading new Wafer DefectAnnotations (.xml) file.
        /// </summary>
        public void ClearAll()
        {
            // 1. Clear Wafers List
            FolderPath = null;
            IsLoading = false;
            if (!Wafers.IsNullOrEmpty())
                Wafers.Clear();
            _wafersListingItemViewModels.Clear();

            // 2. Clear WaferContentViewModel 
            WaferService.DeserialisedFolderPath = "";
            WaferService.DeserialisedWafers.Clear();
            NewFolderPath = "";
            if (_learningTabViewModel != null)
            {
                _learningTabViewModel.SelectedWafer = null;
                _learningTabViewModel.WaferContentViewModel.SelectedWafer = null;
                _learningTabViewModel.AnnotationsListingViewModel.Sources.Clear(); // clear the Sources ObservableCollection of the DefectsAnnotationCollection
            }

            // 3. Clear DefectsAnnotationCollection
            DismissPressed?.Invoke(true); // pressed but actual clear (clear confirmed)

            // 4. Clear the ProgressFileName
            ProgressFileName = "";
            IsProgressFilePresent = false;
        }


        /// <summary>
        /// Go through a selected folder path and gets all the wafers if there are.
        /// </summary>
        private async Task CreateWafers(string folderPath)
        {
            await Task.Run(() =>
            {
                // Determine which extensions are allowed, here images extensions
                string[] AllowedExtensions = new[] { ".bmp", ".tiff", ".tif", ".png", ".jpg", ".jpeg", ".svg" };

                if (Directory.Exists(folderPath))
                {
                    try
                    {
                        IEnumerable<string> searchResult = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly).Where(s => AllowedExtensions.Contains(Path.GetExtension(s).ToLower()));
                        Dictionary<string, string> belongsTo = new Dictionary<string, string>();

                        if (searchResult.Any())
                        {
                            foreach (string file in searchResult)
                            {
                                // If the path contains a certain number of '_', we consider it as a Wafer layer image
                                if (Path.GetFileNameWithoutExtension(file).Count(c => (c == '_')) == 8)
                                {
                                    // So we split it to get the corresponding Wafer base name
                                    string[] items = Path.GetFileNameWithoutExtension(file).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                                    string waferBaseName = string.Join("_", items.Take(8));

                                    // Finally, we add the layer full path and its Wafer base name to the dictionary
                                    belongsTo.Add(Path.GetFullPath(file), waferBaseName);
                                }
                            }

                            Wafers = CreateWafersFromDictionary(belongsTo);
                        }
                        else
                            throw new Exception("Folder is empty or does not only contain wafer images, please select a new one.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Debug.WriteLine(ex);
                    }
                }
            });

            SelectedWafer = null;
            CreateWaferVMCollection();
        }

        /// <summary>
        /// When WaferStore is ready and wafers has been created, we build the view models collection.
        /// </summary>
        private void CreateWaferVMCollection()
        {
            _wafersListingItemViewModels.Clear();

            if (Wafers != null && Wafers.Any())
            {
                foreach (Wafer wafer in Wafers)
                {
                    WafersListingItemViewModel itemViewModel = new WafersListingItemViewModel(wafer, CurrentTabType);
                    itemViewModel.WaferStateChanged += ItemViewModel_WaferStateChanged;
                    _wafersListingItemViewModels.Add(itemViewModel);
                }
                OnPropertyChanged((nameof(HasSelectedFolder)));
            }
        }

        /// <summary>
        /// Function to load all wafers from the dictionary built from the selected folder.
        /// </summary>
        /// <param name="belongsTo"></param>
        public List<Wafer> CreateWafersFromDictionary(Dictionary<string, string> belongsTo)
        {
            List<Wafer> wafers = new List<Wafer>();

            foreach (string waferBaseNameValue in belongsTo.Values.Distinct())
            {
                Dictionary<string, List<MilImage>> waferImagesList = new Dictionary<string, List<MilImage>>();

                List<string> imagePathsList = new List<string>();

                foreach (string key in belongsTo.Keys)
                {
                    if (key.Contains(waferBaseNameValue))
                    {
                        imagePathsList.Add(key);
                    }
                }

                foreach (string imagePathKey in imagePathsList)
                {
                    string RepresentationName = RepresentationNameIdentifier.GetRepresentationName(imagePathKey);

                    MilImage milImage = new MilImage();
                    milImage.Restore(imagePathKey);

                    List<MilImage> originalImages = new List<MilImage>
                    {
                        milImage
                    };

                    waferImagesList.Add(RepresentationName, originalImages);
                }

                Wafer wafer = new Wafer(waferBaseNameValue, waferImagesList);

                wafers.Add(wafer);


                foreach (Wafer _wafer in wafers)
                {
                    if (!WaferService.DeserialisedWafers.Contains(_wafer))
                        WaferService.DeserialisedWafers.Add(_wafer);
                }
            }

            // todo: check relevancy of code below (to delete? - double check), was relevant back when we were saving/restoring automatically
            // First time loading path, the NewFolderPath is null
            //if (NewFolderPath == null)
            //{
            //    NewFolderPath = FolderPath;
            //    XmlService.SerializeListWafers(wafers, FolderPath);
            //    NewFolderPath = "";
            //}

            // todo: check relevancy of code below (to delete? - double check), was relevant back when we were saving/restoring automatically
            // Only serialise if the FolderPath has changed
            //if (!NewFolderPath.Equals(FolderPath))
            //{
            //    XmlService.SerializeListWafers(wafers, FolderPath);
            //    NewFolderPath = "";
            //}

            return wafers;
        }

        //====================================================================
        // Events & Dispose Method
        //====================================================================

        /// <summary>
        /// Raised whenever the SelectedWafer changes.
        /// </summary>
        private void WafersStore_SelectedWaferChanged()
        {
            OnPropertyChanged((nameof(SelectedWaferListingItemViewModel)));
        }

        /// <summary>
        /// Event raised by the collection whenever an item is added or removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WafersListingItemViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged((nameof(SelectedWaferListingItemViewModel)));
        }

        /// <summary>
        /// Event raised when a wafer is locked or unlocked.
        /// </summary>
        /// <param name="updatedWafer"></param>
        private void ItemViewModel_WaferStateChanged(Wafer updatedWafer)
        {
            if (SelectedWaferListingItemViewModel?.Wafer.BaseName == updatedWafer.BaseName)
            {
                WafersListingItemViewModel itemViewModel = new WafersListingItemViewModel(updatedWafer, CurrentTabType);

                SelectedWaferListingItemViewModel = itemViewModel;
            }
        }

        /// <summary>
        /// Unsubscribe to events and free memory.
        /// </summary>
        public void Dispose()
        {
            _wafersListingItemViewModels.CollectionChanged -= WafersListingItemViewModels_CollectionChanged;

            foreach (WafersListingItemViewModel itemViewModel in _wafersListingItemViewModels)
            {
                itemViewModel.WaferStateChanged -= ItemViewModel_WaferStateChanged;
            }
        }
    }
}
