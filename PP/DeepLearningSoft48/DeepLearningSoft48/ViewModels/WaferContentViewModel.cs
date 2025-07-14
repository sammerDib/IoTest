using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to WaferContentView.xaml
    /// Permits to display the Selected Wafer's information (Name & Images).
    /// Corresponds to the central located component of the application.
    /// </summary>
    public class WaferContentViewModel : ObservableRecipient
    {
        //====================================================================
        // Main Tabs ViewModels
        //==================================================================== 
        private readonly LearningTabViewModel _learningTabViewModel;
        private readonly TestTabViewModel _testTabViewModel;

        //====================================================================
        // Selected Wafer Fields Initialization
        //====================================================================
        private Wafer _selectedWafer;
        public Wafer SelectedWafer
        {
            get { return _selectedWafer; }
            set
            {
                _selectedWafer = value;
                OnPropertyChanged(nameof(SelectedWafer));
                OnPropertyChanged(nameof(HasSelectedWafer));
                OnPropertyChanged(nameof(IsWaferUnlocked));
            }
        }

        public bool HasSelectedWafer => SelectedWafer != null;
        public bool IsWaferUnlocked => SelectedWafer == null || !SelectedWafer.IsLocked;

        //====================================================================
        // Layers VM Collection Initialization
        //====================================================================
        private readonly ObservableCollection<DisplayWaferLayerViewModel> _waferLayersViewModels;
        public IEnumerable<DisplayWaferLayerViewModel> WaferLayersViewModels => _waferLayersViewModels;

        //====================================================================
        // Selected Layer Fields Initialization
        //====================================================================
        private AnnotateWaferLayerViewModel _selectedLayerViewModel;
        public AnnotateWaferLayerViewModel SelectedLayerViewModel
        {
            get
            {
                return _selectedLayerViewModel;
            }
            set
            {
                _selectedLayerViewModel = value;
                OnPropertyChanged(nameof(SelectedLayerViewModel));
            }
        }
        public bool IsLayerSelected => SelectedLayerViewModel?.SelectedLayerProperties.Key != null && SelectedLayerViewModel?.SelectedLayerProperties.Value != null;

        public bool AreAllLayersSelected => !IsLayerSelected;

        //====================================================================
        // Test Tab Fields Initialization for detection state
        //====================================================================
        private bool _hasDetectionStarted = false;
        public bool HasDetectionStarted
        {
            get
            {
                return _hasDetectionStarted;
            }
            set
            {
                _hasDetectionStarted = value;
                if (HasDetectionStarted)
                {
                    IsDetectionInProgress = value;
                }
                OnPropertyChanged(nameof(HasDetectionStarted));
            }
        }

        private bool _isDetectionInProgress = false;
        public bool IsDetectionInProgress
        {
            get
            {
                return _isDetectionInProgress;
            }
            set
            {
                _isDetectionInProgress = value;
                if (!IsDetectionInProgress)
                {
                    DisplayMaskResult();
                }
                OnPropertyChanged(nameof(IsDetectionInProgress));
            }
        }

        //====================================================================
        // Constructors
        //====================================================================

        /// <summary>
        /// Used bu the LearningTabViewModel.
        /// </summary>
        /// <param name="learningTabViewModel"></param>
        /// <param name="imageAnnotationToolsViewModel"></param>
        public WaferContentViewModel(LearningTabViewModel learningTabViewModel, ImageAnnotationToolsViewModel imageAnnotationToolsViewModel)
        {
            _learningTabViewModel = learningTabViewModel;
            SelectedWafer = _learningTabViewModel.SelectedWafer;
            _waferLayersViewModels = new ObservableCollection<DisplayWaferLayerViewModel>();
            SelectedLayerViewModel = new AnnotateWaferLayerViewModel(_learningTabViewModel, imageAnnotationToolsViewModel);

            _learningTabViewModel.SelectedWaferChanged += WafersStore_SelectedWaferChanged;
        }

        /// <summary>
        /// Used bu the TestTabViewModel.
        /// </summary>
        /// <param name="testTabViewModel"></param>
        /// <param name="imageAnnotationToolsViewModel"></param>
        public WaferContentViewModel(TestTabViewModel testTabViewModel, ImageAnnotationToolsViewModel imageAnnotationToolsViewModel)
        {
            _testTabViewModel = testTabViewModel;
            SelectedWafer = _testTabViewModel.SelectedWafer;
            _waferLayersViewModels = new ObservableCollection<DisplayWaferLayerViewModel>();
            SelectedLayerViewModel = new AnnotateWaferLayerViewModel(_testTabViewModel, imageAnnotationToolsViewModel);

            _testTabViewModel.SelectedWaferChanged += WafersStore_SelectedWaferChanged;
        }


        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Load all images belonging to the selected wafer from the store.
        /// </summary>
        private void LoadImagesOnScreen()
        {
            try
            {
                if (SelectedWafer.WaferImagesLists != null)
                {
                    foreach (KeyValuePair<string, List<MilImage>> layerImagesList in SelectedWafer.WaferImagesLists)
                    {
                        DisplayWaferLayerViewModel itemViewModel = new DisplayWaferLayerViewModel(layerImagesList);
                        _waferLayersViewModels.Add(itemViewModel);
                    }
                }
                else
                    throw new Exception("This wafer contains no image.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Permits to get the result of the detection and display it.
        /// </summary>
        private void DisplayMaskResult()
        {
            DisplayLayerCommand.Execute(SelectedWafer.WaferImagesLists.First().Key);
            SelectedLayerViewModel.GetDetectionMaskResult("C:\\Users\\frederic.dulou\\source\\repos\\USP\\PP\\DeepLearningSoft48\\DeepLearningSoft48\\ResultJSONFile\\" + SelectedWafer.BaseName + "_Result.json");
        }

        //====================================================================
        // Commands
        //====================================================================

        /// <summary>
        /// Command to display the selected layer from radio buttons.
        /// </summary>
        private AutoRelayCommand<string> _displayLayerCommand;
        public AutoRelayCommand<string> DisplayLayerCommand
        {
            get
            {
                return _displayLayerCommand ?? (_displayLayerCommand = new AutoRelayCommand<string>(
              representationName =>
              {
                  foreach (KeyValuePair<string, List<MilImage>> layerImagesList in SelectedWafer.WaferImagesLists)
                  {
                      if (layerImagesList.Key == representationName)
                      {
                          SelectedLayerViewModel.SelectedLayerProperties = layerImagesList;
                          OnPropertyChanged(nameof(SelectedLayerViewModel));
                          OnPropertyChanged(nameof(IsLayerSelected));
                      }
                  }
              }));
            }
        }

        /// <summary>
        /// Command to display all layers from radio buttons
        /// </summary>
        private AutoRelayCommand _displayAllLayersCommand;
        public AutoRelayCommand DisplayAllLayersCommand
        {
            get
            {
                return _displayAllLayersCommand ?? (_displayAllLayersCommand = new AutoRelayCommand(
              () =>
              {
                  SelectedLayerViewModel.SelectedLayerProperties = new KeyValuePair<string, List<MilImage>>();
                  OnPropertyChanged(nameof(IsLayerSelected));
              }));
            }
        }

        //====================================================================
        // Events & Dispose
        //====================================================================

        /// <summary>
        /// Informs that the selected wafer has changed and thus clear all its related variables.
        /// </summary>
        private void WafersStore_SelectedWaferChanged()
        {
            if (_learningTabViewModel != null && _learningTabViewModel.SelectedWafer != null)
                SelectedWafer = _learningTabViewModel.SelectedWafer;

            if (_testTabViewModel != null && _testTabViewModel.SelectedWafer != null)
                SelectedWafer = _testTabViewModel.SelectedWafer;

            _waferLayersViewModels.Clear();
            OnPropertyChanged(nameof(_waferLayersViewModels));

            if (SelectedLayerViewModel != null)
            {
                if (SelectedLayerViewModel.SelectedLayerProperties.Key != null && SelectedLayerViewModel.SelectedLayerProperties.Value != null)
                {
                    SelectedLayerViewModel.SelectedLayerProperties = new KeyValuePair<string, List<MilImage>>();
                    OnPropertyChanged(nameof(SelectedLayerViewModel));
                }
            }

            if (SelectedWafer != null && SelectedWafer.WaferImagesLists != null)
            {
                LoadImagesOnScreen();
            }

            OnPropertyChanged(nameof(IsLayerSelected));
            OnPropertyChanged(nameof(AreAllLayersSelected));
        }
    }
}
