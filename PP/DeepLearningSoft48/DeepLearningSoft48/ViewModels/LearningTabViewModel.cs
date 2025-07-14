using System;
using System.Collections.Generic;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Modules;
using DeepLearningSoft48.Modules.AdvancedModules.ImageProcessing.ComplexTransformation.AdjustLevels;
using DeepLearningSoft48.Modules.BasicModules.ImageProcessing.Binarization.ThresholdStd;
using DeepLearningSoft48.Modules.BasicModules.ImageProcessing.ShapeDetection;
using DeepLearningSoft48.Utils.Enums;

using UnitySC.Shared.LibMIL;

using MvvmDialogs;

using Serilog;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// One of the 2 main tab.
    /// View Model of the Learning Tab.
    /// </summary>
    public class LearningTabViewModel : ObservableRecipient
    {
        //====================================================================
        // ViewModelLocator class's DialogService
        //====================================================================
        private readonly IDialogService _dialogService;

        //====================================================================
        // Mil Instanciation
        //====================================================================
        public Mil MilSrc = Mil.Instance;

        //====================================================================
        // Wafers List Field Initialization
        //====================================================================
        public List<Wafer> Wafers;

        //====================================================================
        // Selected Wafer Fields Initialization
        //====================================================================
        private Wafer _selectedWafer;
        public Wafer SelectedWafer
        {
            get
            {
                return _selectedWafer;
            }
            set
            {
                if (WaferContentViewModel.SelectedLayerViewModel.CanWaferChange())
                    _selectedWafer = value;
                OnPropertyChanged(nameof(SelectedWafer));
                SelectedWaferChanged?.Invoke();
            }
        }
        public event Action SelectedWaferChanged;

        //====================================================================
        // Wafer's Property
        //====================================================================

        /// <summary>
        /// Boolean to check the state of the SelectedWafer to enable edition 
        /// components as ApplyImageProcessesViewModel or ImageAnnotationToolsViewModel.
        /// </summary>
        public bool IsWaferUnlocked => SelectedWafer == null || !SelectedWafer.IsLocked;

        //====================================================================
        // View Models Fields Initialization
        //====================================================================

        /// <summary>
        /// Contains the list of wafers.
        /// Corresponds to the top-left located component of the application.
        /// </summary>
        public WafersListingViewModel WafersListingViewModel { get; }

        /// <summary>
        /// Permits to display the Selected Wafer's information (Name & Images).
        /// Corresponds to the central located component of the application.
        /// </summary>
        public WaferContentViewModel WaferContentViewModel { get; }

        /// <summary>
        /// Contains Annotation Tools Buttons and Defect Categories.
        /// Corresponds to the bottom-right located component of the application.
        /// </summary>
        public ImageAnnotationToolsViewModel ImageAnnotationToolsViewModel { get; }

        /// <summary>
        /// Contains the button to apply image processes.
        /// Corresponds to the middle-right located component of the application.
        /// </summary>
        private ApplyImageProcessesDialogViewModel ApplyImageProcessesDialogViewModel { get; }

        /// <summary>
        /// Contains the list of all annotations made on the mask.
        /// Corresponds to the bottom-right located component of the application.
        /// </summary>
        public AnnotationsListingViewModel AnnotationsListingViewModel { get; }

        //====================================================================
        // Image Processing Modules Initialization
        //====================================================================

        /// <summary>
        /// List of all modules registered in the application.
        /// </summary>
        public List<ModuleBase> ModuleList;

        //====================================================================
        // Constructor
        //====================================================================
        public LearningTabViewModel(TabType tabType, ImageAnnotationToolsViewModel imageAnnotationToolsViewModel)
        {
            _dialogService = new DialogService();

            MilSrc.Free();
            MilSrc.Allocate();

            LoadModules();

            WafersListingViewModel = new WafersListingViewModel(this, tabType);

            ImageAnnotationToolsViewModel = imageAnnotationToolsViewModel;

            WaferContentViewModel = new WaferContentViewModel(this, ImageAnnotationToolsViewModel);

            AnnotationsListingViewModel = new AnnotationsListingViewModel(this);

            ApplyImageProcessesDialogViewModel = new ApplyImageProcessesDialogViewModel(this);
        }

        //====================================================================
        // Commands
        //====================================================================
        /// <summary>
        /// Open the Modal Window to Apply an image processing to selected layer(s).
        /// </summary>
        private AutoRelayCommand _applyImageProcessCommand;
        public AutoRelayCommand ApplyImageProcessCommand
        {
            get
            {
                return _applyImageProcessCommand ?? (_applyImageProcessCommand = new AutoRelayCommand(
              () =>
              {
                  var success = _dialogService.ShowDialog<Views.PopUpWindows.ApplyImageProcessesDialog>(this, ApplyImageProcessesDialogViewModel);
                  if (success == true)
                  {
                      foreach (DisplayWaferLayerViewModel checkedLayer in ApplyImageProcessesDialogViewModel.CheckedLayers)
                      {
                          // Get the last Image Index
                          int ImageToProcessIndex = checkedLayer.MilImages.Count - 1;

                          // Get its MilImage
                          MilImage MilImgSrc = new MilImage();
                          MilImgSrc.Alloc2dCompatibleWith(checkedLayer.MilImages[ImageToProcessIndex]);
                          MilImage.Copy(checkedLayer.MilImages[ImageToProcessIndex], MilImgSrc);

                          // Process Selected Module
                          MilImage MilImgDest = ApplyImageProcessesDialogViewModel.SelectedModule.Process(MilImgSrc);

                          // Add the new processed image to the list
                          checkedLayer.MilImages.Add(MilImgDest);

                          //Get back the Bitmap
                          Bitmap bmp = MilImgDest.ConvertToBitmap();

                          // Display the new processed image
                          checkedLayer.DisplayedWaferImageSource = ApplyImageProcessesDialogViewModel.Bitmap2ImgSrc(bmp);

                          if (checkedLayer.RepresentationName == WaferContentViewModel.SelectedLayerViewModel.SelectedLayerProperties.Key)
                          {
                              WaferContentViewModel.SelectedLayerViewModel.DisplayedWaferImageSource = checkedLayer.DisplayedWaferImageSource;
                          }
                      }

                      ApplyImageProcessesDialogViewModel.CheckedLayers.Clear();
                  }
              }));
            }
        }


        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Method to load all Modules existing in the application.
        /// </summary>
        public void LoadModules()
        {
            ModuleList = new List<ModuleBase>();

            IModuleFactory factory;
            ModuleBase module;

            factory = new SobelFactory();
            module = new SobelModule(factory);
            ModuleList.Add(module);

            factory = new ThresholdStandardFactory();
            module = new ThresholdStandardModule(factory);
            ModuleList.Add(module);

            factory = new AdjustLevelsFactory();
            module = new AdjustLevelsModule(factory);
            ModuleList.Add(module);
        }

        //=================================================================
        // Log
        //=================================================================
        private static bool s_logErrorInit = false;

        public static void log(string msg)
        {
            if (s_logErrorInit) return;
            try
            {
                Log.Information(msg);

            }
            catch
            {
                s_logErrorInit = true;
            }

        }

        public static void logWarning(string msg)
        {
            if (s_logErrorInit) return;
            try
            {
                Log.Warning(msg);

            }
            catch
            {
                s_logErrorInit = true;
            }
        }

        public static void logError(string msg)
        {
            if (s_logErrorInit) return;
            try
            {
                Log.Error(msg);

            }
            catch
            {
                s_logErrorInit = true;
            }
        }
    }
}
