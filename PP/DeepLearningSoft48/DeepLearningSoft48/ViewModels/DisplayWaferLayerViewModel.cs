using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.LibMIL;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to WaferLayerView.xaml 
    /// View used when no layer is selected.
    /// Permits to only display each layer.
    /// </summary>
    public class DisplayWaferLayerViewModel : ObservableRecipient
    {
        //====================================================================
        // Name of the Current Layer
        //====================================================================
        private string _representationName;
        public string RepresentationName
        {
            get
            {
                return _representationName;
            }
            set
            {
                _representationName = value;
                OnPropertyChanged(nameof(RepresentationName));
            }
        }

        //====================================================================
        // List containing:
        //      First -> Original Image at position 0
        //      Then -> All results from processed image in order
        //====================================================================
        public List<MilImage> MilImages { get; set; }

        //====================================================================
        // Displayed Image's Source of the Selected Layer
        //====================================================================
        private ImageSource _displayedWaferImageSource;
        public ImageSource DisplayedWaferImageSource
        {
            get
            {
                return _displayedWaferImageSource;
            }
            set
            {
                _displayedWaferImageSource = value;
                OnPropertyChanged(nameof(DisplayedWaferImageSource));
            }
        }

        //====================================================================
        // Property Called To Display Image With the Right Size
        //====================================================================
        private Rect _visibleRect = Rect.Empty;
        public Rect VisibleRect
        {
            get { return _visibleRect; }
            set
            {
                if (_visibleRect == value)
                    return;
                _visibleRect = value;
                OnPropertyChanged();
            }
        }

        //====================================================================
        // Property
        //====================================================================

        /// <summary>
        /// GraphView control is used to zoom in and zoom out.
        /// It should be collapse when the user wants to annotate
        /// defects, otherwise the canvas is not accessible while 
        /// it is behind the GraphView control.
        /// </summary>
        public bool IsGraphViewVisible { get { return true; } }

        //====================================================================
        // Constructor
        //====================================================================
        public DisplayWaferLayerViewModel(KeyValuePair<string, List<MilImage>> layerImagesList)
        {
            // Set Layer Name
            RepresentationName = layerImagesList.Key;

            // Get Layer Img List
            MilImages = layerImagesList.Value;

            // Create Bitmap from Original Image
            int index = MilImages.Count - 1; // Get last image in the list
            Bitmap btm = MilImages[index].ConvertToBitmap();

            // Create Image source
            DisplayedWaferImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
               btm.GetHbitmap(),
               IntPtr.Zero,
               Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());

            // Allocating a new value to called linked event and resize image
            VisibleRect = new Rect(0.0, 0.0, DisplayedWaferImageSource.Width, DisplayedWaferImageSource.Height);
        }
    }
}
