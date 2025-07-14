using System;
using System.Collections.Generic;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.ComToLearningModel;
using DeepLearningSoft48.Models;
using DeepLearningSoft48.Utils.Enums;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// One of the 2 main tab.
    /// View Model of the Test Tab.
    /// </summary>
    public class TestTabViewModel : ObservableRecipient
    {
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
                OnPropertyChanged(nameof(HasSelectedWafer));
                SelectedWaferChanged?.Invoke();
            }
        }
        public event Action SelectedWaferChanged;
        public bool HasSelectedWafer => SelectedWafer != null;

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
        /// Contains the list of all annotations made on the mask.
        /// Corresponds to the bottom-right located component of the application.
        /// </summary>
        public AnnotationsListingViewModel AnnotationsListingViewModel { get; }

        //====================================================================
        // Constructor
        //====================================================================
        public TestTabViewModel(TabType tabType, ImageAnnotationToolsViewModel imageAnnotationToolsViewModel)
        {
            WafersListingViewModel = new WafersListingViewModel(this, tabType);
            WaferContentViewModel = new WaferContentViewModel(this, imageAnnotationToolsViewModel);
            AnnotationsListingViewModel = new AnnotationsListingViewModel(this);
        }

        //====================================================================
        // Commands
        //====================================================================

        /// <summary>
        /// Command to send the SelectedWafer information to the deep learning
        /// model for analysis and get back the detection result.
        /// 02/03/2023: not completely working and implemented.
        /// </summary>
        private AutoRelayCommand _startDetection;
        public AutoRelayCommand StartDetection
        {
            get
            {
                return _startDetection ?? (_startDetection = new AutoRelayCommand(
              () =>
              {
                  WaferContentViewModel.HasDetectionStarted = true;

                  // Send SelectedWafer base name
                  ObjectSocket.SetBaseName(SelectedWafer.BaseName);
                  ObjectSocket.SendObject(SelectedWafer.BaseName);

                  // Send each layer
                  foreach (KeyValuePair<string, List<MilImage>> layer in SelectedWafer.WaferImagesLists)
                  {
                      // Send layer's name
                      ObjectSocket.SendObject(layer.Key);
                      // Send corresponding image
                      Bitmap bmp = layer.Value[0].ConvertToBitmap();
                      ObjectSocket.SendObject(bmp);
                  }

                  ObjectSocket.SendObject("EOF");

                  WaferContentViewModel.IsDetectionInProgress = false;
              }));
            }
        }
    }
}
