using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Modules;
using DeepLearningSoft48.Services;

using UnitySC.Shared.LibMIL;

using MvvmDialogs;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to ApplyImageProcesses.xaml
    /// Permits to apply images processes on wafer's images.
    /// </summary>
    public class ApplyImageProcessesDialogViewModel : ObservableRecipient, IModalDialogViewModel
    {
        //====================================================================
        // Commands and DialogResult for the ViewModelLocator class
        //====================================================================
        private readonly IDialogService _dialogService;

        private readonly AutoRelayCommand _applyProcessCommand;
        public ICommand ApplyProcessCommand => _applyProcessCommand;

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        ///====================================================================
        // Learning Tab ViewModel
        //==================================================================== 
        private readonly LearningTabViewModel _learningTabViewModel;

        //====================================================================
        // Mil Instanciation
        //====================================================================
        protected Mil MilSrc => _learningTabViewModel.MilSrc;

        //====================================================================
        // WaferContentViewModel Initialization
        //====================================================================

        /// <summary>
        /// Permits to display the Selected Wafer's information (Name & Images).
        /// Corresponds to the central located component of the application.
        /// </summary>
        public WaferContentViewModel WaferContentViewModel { get; }

        /// <summary>
        /// List of Wafer Layer's ViewModels.
        /// </summary>
        public IEnumerable<DisplayWaferLayerViewModel> WaferLayersViewModels => WaferContentViewModel.WaferLayersViewModels;

        //====================================================================
        // AddImageProcessDialogViewModel Initialization
        //====================================================================

        /// <summary>
        /// Dialog opened to add new processes in the AddedModulesList collection.
        /// </summary>
        public AddImageProcessDialogViewModel AddImageProcessDialogViewModel { get; set; }

        //====================================================================
        // Image Processing Modules Collection Initialization
        //====================================================================

        /// <summary>
        /// This collection represents the list of all the modules added by the 
        /// user as the most frequently used modules.
        /// </summary>
        public ObservableCollection<ModuleBase> AddedModulesList { get; set; }

        public List<Module> SerialisableModulesList { get; set; }

        public List<ModuleBase> ExistingModulesList { get; set; }

        /// <summary>
        /// Check if the list is empty or not.
        /// </summary>
        public bool HasModuleAdded => AddedModulesList != null && AddedModulesList.Count != 0;

        //====================================================================
        // Selected Module Property from the Collection
        //====================================================================
        private ModuleBase _selectedModule;
        public ModuleBase SelectedModule
        {
            get
            {
                return _selectedModule;
            }
            set
            {
                _selectedModule = value;
                SelectedModuleChanged?.Invoke();
            }
        }
        /// <summary>
        /// Event raised when the SelectedModule has changed.
        /// </summary>
        public event Action SelectedModuleChanged;

        /// <summary>
        /// Check if a module has been selected.
        /// </summary>
        public bool IsModuleSelected => _selectedModule != null;

        /// <summary>
        /// List of the layer(s) on which the SelectedModule has to be applied.
        /// </summary>
        private List<DisplayWaferLayerViewModel> _checkedLayers = new List<DisplayWaferLayerViewModel>();
        public List<DisplayWaferLayerViewModel> CheckedLayers
        {
            get
            {
                return _checkedLayers;
            }
            set
            {
                _checkedLayers = value;
                OnPropertyChanged(nameof(CheckedLayers));
                OnPropertyChanged(nameof(CanApplyProcess));
            }
        }

        /// <summary>
        /// Check whether the SelectedModule can be applied. 
        /// </summary>
        public bool CanApplyProcess => IsModuleSelected && CheckedLayers.Count != 0;

        //====================================================================
        // Constructor
        //====================================================================
        public ApplyImageProcessesDialogViewModel(LearningTabViewModel learningTabViewModel)
        {
            _dialogService = new DialogService();
            _applyProcessCommand = new AutoRelayCommand(Apply);

            _learningTabViewModel = learningTabViewModel;
            WaferContentViewModel = learningTabViewModel.WaferContentViewModel;

            AddImageProcessDialogViewModel = new AddImageProcessDialogViewModel(learningTabViewModel.ModuleList);
            AddedModulesList = new ObservableCollection<ModuleBase>();

            SerialisableModulesList = new List<Module>();
            ExistingModulesList = learningTabViewModel.ModuleList;

            AddedModulesList.CollectionChanged += Modules_CollectionChanged;
            SelectedModuleChanged += ApplyImageProcessViewModel_SelectedModuleChanged;

            LoadSavedModules();
        }

        //====================================================================
        // Commands
        //====================================================================

        /// <summary>
        /// Open the AddImageProcessDialog to add a module to the AddedModulesList collection.
        /// </summary>
        private AutoRelayCommand _addImageProcessCommand;
        public AutoRelayCommand AddImageProcessCommand
        {
            get
            {
                return _addImageProcessCommand ?? (_addImageProcessCommand = new AutoRelayCommand(
              () =>
              {
                  var success = _dialogService.ShowDialog<Views.PopUpWindows.AddImageProcessDialog>(this, AddImageProcessDialogViewModel);
                  if (success == true)
                  {
                      AddModule(AddImageProcessDialogViewModel.SelectedItems);
                      AddImageProcessDialogViewModel.SelectedItems.Clear();
                  }
              }));
            }
        }

        /// <summary>
        /// Remove a module from the AddedModulesList collection.
        /// </summary>
        private AutoRelayCommand _deleteImageProcessCommand;
        public AutoRelayCommand DeleteImageProcessCommand
        {
            get
            {
                return _deleteImageProcessCommand ?? (_deleteImageProcessCommand = new AutoRelayCommand(
              () =>
              {
                  DeleteModuleByName();
              }));
            }
        }

        /// <summary>
        /// Remove all modules from the AddedModulesList Collection
        /// </summary>
        private AutoRelayCommand _deleteAllProcessesCommand;
        public AutoRelayCommand DeleteAllProcessesCommand
        {
            get
            {
                return _deleteAllProcessesCommand ?? (_deleteAllProcessesCommand = new AutoRelayCommand(
              () =>
              {
                  DeleteAllModules();
              }));
            }
        }

        /// <summary>
        /// Command to add a layer to the list of layers to be processed.
        /// </summary>
        private AutoRelayCommand<DisplayWaferLayerViewModel> _checkLayerCommand;
        public AutoRelayCommand<DisplayWaferLayerViewModel> CheckLayerCommand
        {
            get
            {
                return _checkLayerCommand ?? (_checkLayerCommand = new AutoRelayCommand<DisplayWaferLayerViewModel>(
              layerViewModel =>
              {
                  CheckedLayers.Add(layerViewModel);
                  OnPropertyChanged(nameof(CanApplyProcess));

              }));
            }
        }

        /// <summary>
        /// Command to remove a layer to the list of layers to be processed.
        /// </summary>
        private AutoRelayCommand<DisplayWaferLayerViewModel> _uncheckLayerCommand;
        public AutoRelayCommand<DisplayWaferLayerViewModel> UncheckLayerCommand
        {
            get
            {
                return _uncheckLayerCommand ?? (_uncheckLayerCommand = new AutoRelayCommand<DisplayWaferLayerViewModel>(
              layerViewModel =>
              {
                  CheckedLayers.Remove(layerViewModel);
                  OnPropertyChanged(nameof(CanApplyProcess));

              }));
            }
        }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Method linked to the called command allonwing to apply a process.
        /// </summary>
        private void Apply()
        {
            if (DialogResult != null)
            {
                DialogResult = null;
            }
            if (CanApplyProcess)
            {
                DialogResult = true;
            }
        }

        /// <summary>
        /// Converter to get a ImageSource from a Bitmap.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns> ImageSource </returns>
        public ImageSource Bitmap2ImgSrc(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               bitmap.GetHbitmap(),
               IntPtr.Zero,
               Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Add module(s) and serialise.
        /// </summary>
        public void AddModule(List<ModuleBase> selectedItems)
        {
            foreach (ModuleBase module in selectedItems)
            {
                Module moduleToAdd = new Module();
                moduleToAdd.Name = module.DisplayName;

                if (!AddedModulesList.Contains(module) && !SerialisableModulesList.Contains(moduleToAdd))
                {
                    AddedModulesList.Add(module);
                    SerialisableModulesList.Add(moduleToAdd);
                    OnPropertyChanged(nameof(HasModuleAdded));
                }
                else
                {
                    MessageBox.Show("Module(s) has already added: " + module);
                }
            }
            XmlService.SerializeListModules(SerialisableModulesList);
        }

        /// <summary>
        /// Delete a module and serialise.
        /// </summary>
        public void DeleteModuleByName()
        {
            List<Module> updatedModulesList = new List<Module>();

            if (_selectedModule != null && SerialisableModulesList != null)
            {
                string moduleToRemove = _selectedModule.DisplayName;

                foreach (Module module in SerialisableModulesList)
                {
                    if (module.Name != moduleToRemove)
                    {
                        updatedModulesList.Add(module);
                    }
                }
            }

            SerialisableModulesList = updatedModulesList;
            AddedModulesList.Remove(_selectedModule);
            XmlService.SerializeListModules(SerialisableModulesList);
        }

        /// <summary>
        /// Delete all modules and serialise.
        /// </summary>
        public void DeleteAllModules()
        {
            if (AddedModulesList != null && SerialisableModulesList != null)
            {
                AddedModulesList.Clear();
                SerialisableModulesList.Clear();
                XmlService.SerializeListModules(SerialisableModulesList);
            }
        }

        /// <summary>
        /// Load saved modules whilst handling first time usage of app whereby user never saved anything before.
        /// </summary>
        public void LoadSavedModules()
        {
            try
            {
                SerialisableModulesList = XmlService.DeserializeListModules();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                Console.WriteLine("This must be your first time saving a ModulesList file. Creating the file...");
                XmlService.SerializeListModules(SerialisableModulesList);
                SerialisableModulesList = XmlService.DeserializeListModules();
            }

            ReconstructAddedModulesList();
        }

        /// <summary>
        /// Reconstruct the AddedModulesList when loading saved modules from the XML file. 
        ///  This approach was necessary due to the conflict between the list types(AddedModulesList vs. SerialisableModulesList),
        ///  and because it easily fetches the corresponding properties of each module without having to serialise each one(which was deemed unecessary).
        /// </summary>
        public void ReconstructAddedModulesList()
        {
            if (SerialisableModulesList != null)
            {
                foreach (Module moduleName in SerialisableModulesList)
                {
                    var matchingModule = ExistingModulesList.FirstOrDefault(m => m.DisplayName == moduleName.Name);
                    if (matchingModule != null)
                    {
                        AddedModulesList.Add(matchingModule);
                    }
                }
            }
        }
        //====================================================================
        // Events & Dispose
        //====================================================================

        /// <summary>
        /// Event raised when the SelectedModule has changed.
        /// </summary>
        public void ApplyImageProcessViewModel_SelectedModuleChanged()
        {
            OnPropertyChanged(nameof(SelectedModule));
            OnPropertyChanged(nameof(IsModuleSelected));
            OnPropertyChanged(nameof(CanApplyProcess));
        }

        /// <summary>
        /// Update linked View when a Module is added or removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Modules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedModule));
            OnPropertyChanged(nameof(IsModuleSelected));
        }

        /// <summary>
        /// Unsubscribe to events and free memory.
        /// </summary>
        public void Dispose()
        {
            AddedModulesList.CollectionChanged -= Modules_CollectionChanged;
            SelectedModuleChanged -= ApplyImageProcessViewModel_SelectedModuleChanged;
        }
    }
}
