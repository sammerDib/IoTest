using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Models.DefectAnnotations;
using DeepLearningSoft48.Services;
using DeepLearningSoft48.Utils;
using DeepLearningSoft48.ViewModels.DefectAnnotations;

using UnitySC.Shared.LibMIL;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnitySC.Shared.Tools;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to AnnotateWaferLayerView.xaml 
    /// View used when a layer is selected.
    /// Permits to annotate defects thanks to ImageAnnotationTools.xaml component tools.
    /// </summary>
    public class AnnotateWaferLayerViewModel : ObservableRecipient
    {
        //====================================================================
        // Mapper Property
        //====================================================================
        private Mapper _mapper = ClassLocator.Default.GetInstance<Mapper>();

        //====================================================================
        // GraphView Property
        //====================================================================

        /// <summary>
        /// GraphView control is used to zoom in and zoom out.
        /// It should be collapse when the user wants to annotate
        /// defects, otherwise the canvas is not accessible while 
        /// it is behind the GraphView control.
        /// </summary>
        public bool IsGraphViewVisible => ToolMode == ToolMode.NONE;

        //====================================================================
        // Main Tabs ViewModels & Selected Wafer
        //==================================================================== 
        private readonly LearningTabViewModel _learningTabViewModel;
        private readonly TestTabViewModel _testTabViewModel;

        //====================================================================
        // Defect Annotations Collection for the canvas 
        //==================================================================== 
        private ObservableCollection<DefectAnnotationVM> _defectsAnnotationsCollection;
        public ObservableCollection<DefectAnnotationVM> DefectsAnnotationsCollection
        {
            get
            {
                return _defectsAnnotationsCollection;
            }
            set
            {
                if (_defectsAnnotationsCollection != value)
                    _defectsAnnotationsCollection = value;
            }
        }

        //====================================================================
        // Selected Layer's Properties
        // (string: RepresentationName (Ex: CX, CY...), List<MilImage>: Images)
        //====================================================================
        private KeyValuePair<string, List<MilImage>> _selectedLayerProperties;
        public KeyValuePair<string, List<MilImage>> SelectedLayerProperties
        {
            get
            {
                return _selectedLayerProperties;
            }
            set
            {
                _selectedLayerProperties = value;
                OnPropertyChanged();
                SelectedLayerChanged?.Invoke();
            }
        }

        /// <summary>
        /// Event raised when the Selected Layer has changed.
        /// </summary>
        public event Action SelectedLayerChanged;

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
        // Cursor coordinates on active mask (canvas)
        //====================================================================
        private int _xCoordinate;
        public int XCoordinate
        {
            get
            {
                return _xCoordinate;
            }
            set
            {
                _xCoordinate = value;
                OnPropertyChanged();
            }
        }

        private int _yCoordinate;
        public int YCoordinate
        {
            get
            {
                return _yCoordinate;
            }
            set
            {
                _yCoordinate = value;
                OnPropertyChanged();
            }
        }

        //====================================================================
        // ImageAnnotationToolsViewModel Initialization
        //====================================================================

        /// <summary>
        /// Contains Annotation Tools Buttons and Defect Categories.
        /// Corresponds to the bottom-right located component of the application.
        /// </summary>
        protected ImageAnnotationToolsViewModel ImageAnnotationToolsViewModel;

        /// <summary>
        /// Selected Tool
        /// </summary>
        public ToolMode ToolMode => ImageAnnotationToolsViewModel.SelectedToolMode;

        /// <summary>
        /// Selected Tool's speciafications.
        /// </summary>
        public DefectCategoryPair SelectedCategory => ImageAnnotationToolsViewModel.SelectedCategory;
        public double SelectedThickness => ImageAnnotationToolsViewModel.SelectedThickness;


        //====================================================================
        // Constructors
        //====================================================================

        /// <summary>
        /// Used bu the LearningTabViewModel.
        /// </summary>
        /// <param name="learningTabViewModel"></param>
        /// <param name="imageAnnotationToolsViewModel"></param>
        public AnnotateWaferLayerViewModel(LearningTabViewModel learningTabViewModel, ImageAnnotationToolsViewModel imageAnnotationToolsViewModel)
        {
            _learningTabViewModel = learningTabViewModel;
            _learningTabViewModel.SelectedWaferChanged += _mainViewModel_SelectedWaferChanged;

            // Catch whenever the selected layer changes
            SelectedLayerChanged += AnnotationModeWaferLayerViewModel_SelectedLayerChanged;

            // Get View Model with Commands Events to Draw
            ImageAnnotationToolsViewModel = imageAnnotationToolsViewModel;
            ImageAnnotationToolsViewModel.SelectedAnnotationTypeChanged += ImageAnnotationToolsViewModel_SelectedAnnotationTypeChanged;
            ImageAnnotationToolsViewModel.ClearAllPressed += ImageAnnotationToolsViewModel_ClearAllPressed;
            ImageAnnotationToolsViewModel.ToolTypeChanged += ImageAnnotationToolsViewModel_ToolTypeChanged;
            ImageAnnotationToolsViewModel.SelectedThicknessChanged += ImageAnnotationToolsViewModel_SelectedThicknessChanged;
            ImageAnnotationToolsViewModel.SelectedCategoryChanged += ImageAnnotationToolsViewModel_SelectedCategoryChanged;
            WafersListingViewModel.DismissPressed += WafersListingViewModel_DismissPressed;

            DefectsAnnotationsCollection = new ObservableCollection<DefectAnnotationVM>();
        }

        /// <summary>
        /// Used bu the TestTabViewModel.
        /// </summary>
        /// <param name="testTabViewModel"></param>
        /// <param name="imageAnnotationToolsViewModel"></param>
        public AnnotateWaferLayerViewModel(TestTabViewModel testTabViewModel, ImageAnnotationToolsViewModel imageAnnotationToolsViewModel)
        {
            _testTabViewModel = testTabViewModel;
            _testTabViewModel.SelectedWaferChanged += _mainViewModel_SelectedWaferChanged;

            // Catch whenever the selected layer changes
            SelectedLayerChanged += AnnotationModeWaferLayerViewModel_SelectedLayerChanged;

            // Get View Model with Commands Events to Draw
            ImageAnnotationToolsViewModel = imageAnnotationToolsViewModel;
            ImageAnnotationToolsViewModel.SelectedAnnotationTypeChanged += ImageAnnotationToolsViewModel_SelectedAnnotationTypeChanged;
            ImageAnnotationToolsViewModel.ClearAllPressed += ImageAnnotationToolsViewModel_ClearAllPressed;
            ImageAnnotationToolsViewModel.ToolTypeChanged += ImageAnnotationToolsViewModel_ToolTypeChanged;
            ImageAnnotationToolsViewModel.SelectedThicknessChanged += ImageAnnotationToolsViewModel_SelectedThicknessChanged;
            ImageAnnotationToolsViewModel.SelectedCategoryChanged += ImageAnnotationToolsViewModel_SelectedCategoryChanged;
            WafersListingViewModel.DismissPressed += WafersListingViewModel_DismissPressed;

            DefectsAnnotationsCollection = new ObservableCollection<DefectAnnotationVM>();
        }

        public AnnotateWaferLayerViewModel() { }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Method to read the JSON file received as a result of the detection in the Test Tab.
        /// It creates all bounding boxes and add them to the canvas.
        /// </summary>
        /// <param name="resultFilePath"></param>
        public void GetDetectionMaskResult(string resultFilePath)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(resultFilePath));
            JTokenEqualityComparer jTokenEqualityComparer = new JTokenEqualityComparer();

            foreach (var jsonchildren in jsonFile)
            {
                int id = 0;
                string layer;
                string defectClass;

                foreach (var obj in jsonchildren)
                {
                    switch (id)
                    {
                        case 0:        // layer
                            layer = (string)obj;
                            break;

                        case 1:        // defect class
                            defectClass = (string)obj;
                            break;
                        case 2:        // list of blobs
                            {
                                foreach (var bbox in obj)
                                {

                                    //WaferService.SelectedWafer.DefectsAnnotationsList.Add(
                                    //    new BoundingBox((double)bbox[0], (double)bbox[1], (double)bbox[2], (double)bbox[3], defectClass, RepresentationNameIdentifier.GetRepresentationName((string)fileName)));

                                }
                            }
                            break;
                    }
                    id++;
                }
                //JToken fileName = jsonFile[i].SelectToken("CX");
                //JToken id = jsonFile["images"][i].SelectToken("id");

                //Debug.WriteLine($"Wafer's layer: {fileName}");
                //Debug.WriteLine($"Layer's id: {id}");

                //for (int j = 0; j < jsonFile["annotations"].Count; j++)
                //{
                //    JToken imageId = jsonFile["annotations"][j].SelectToken("image_id");
                //    JToken bbox = jsonFile["annotations"][j].SelectToken("bbox");
                //    JToken categoryId = jsonFile["annotations"][j].SelectToken("category_id");

                //    string categoryName = "Unknown";
                //    DefectCategoryPair categoryPair;

                //    if (jTokenEqualityComparer.Equals(imageId, id))
                //    {
                //        for (int k = 0; k < jsonFile["categories"].Count; k++)
                //        {
                //            if (jTokenEqualityComparer.Equals(categoryId, jsonFile["categories"][k]["id"]))
                //                categoryName = (string)jsonFile["categories"][k]["name"];
                //        }

                //        if (ImageAnnotationToolsViewModel.DefectCategories.Any(y => y.Label == categoryName.ToUpper()))
                //            categoryPair = ImageAnnotationToolsViewModel.DefectCategories.FirstOrDefault(y => y.Label == categoryName.ToUpper());
                //        else
                //        {
                //            Random random = new Random();
                //            System.Drawing.Color newCategoryColor;
                //            do
                //            {
                //                newCategoryColor = System.Drawing.Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                //            }
                //            while (ImageAnnotationToolsViewModel.DefectCategories.Any(y => y.Color.Equals(newCategoryColor)));

                //            categoryPair = new DefectCategoryPair(newCategoryColor, categoryName.ToUpper());
                //            ImageAnnotationToolsViewModel.AddNewDefectCategory(categoryPair);
                //        }

                //        WaferService.SelectedWafer.DefectsAnnotationsList.Add(
                //            new BoundingBox((double)bbox[0], (double)bbox[1], (double)bbox[2], (double)bbox[3], categoryPair, RepresentationNameIdentifier.GetRepresentationName((string)fileName)));
                //    }
                //}
            }

            TestSectionDefectListToVMCollection();
        }

        public void GetDetectionMaskResult_old(string resultFilePath)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText(resultFilePath));
            JTokenEqualityComparer jTokenEqualityComparer = new JTokenEqualityComparer();

            for (int i = 0; i < jsonFile[0].Count; i++)
            {
                JToken fileName = jsonFile[i].SelectToken("CX");
                JToken id = jsonFile["images"][i].SelectToken("id");

                Debug.WriteLine($"Wafer's layer: {fileName}");
                Debug.WriteLine($"Layer's id: {id}");

                for (int j = 0; j < jsonFile["annotations"].Count; j++)
                {
                    JToken imageId = jsonFile["annotations"][j].SelectToken("image_id");
                    JToken bbox = jsonFile["annotations"][j].SelectToken("bbox");
                    JToken categoryId = jsonFile["annotations"][j].SelectToken("category_id");

                    string categoryName = "Unknown";
                    DefectCategoryPair categoryPair;

                    if (jTokenEqualityComparer.Equals(imageId, id))
                    {
                        for (int k = 0; k < jsonFile["categories"].Count; k++)
                        {
                            if (jTokenEqualityComparer.Equals(categoryId, jsonFile["categories"][k]["id"]))
                                categoryName = (string)jsonFile["categories"][k]["name"];
                        }

                        if (ImageAnnotationToolsViewModel.DefectCategories.Any(y => y.Label == categoryName.ToUpper()))
                            categoryPair = ImageAnnotationToolsViewModel.DefectCategories.FirstOrDefault(y => y.Label == categoryName.ToUpper());
                        else
                        {
                            Random random = new Random();
                            System.Drawing.Color newCategoryColor;
                            do
                            {
                                newCategoryColor = System.Drawing.Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                            }
                            while (ImageAnnotationToolsViewModel.DefectCategories.Any(y => y.Color.Equals(newCategoryColor)));

                            categoryPair = new DefectCategoryPair(newCategoryColor, categoryName.ToUpper());
                            ImageAnnotationToolsViewModel.AddNewDefectCategory(categoryPair);
                        }

                        WaferService.SelectedWafer.DefectsAnnotationsList.Add(
                            new BoundingBox((double)bbox[0], (double)bbox[1], (double)bbox[2], (double)bbox[3], categoryPair, RepresentationNameIdentifier.GetRepresentationName((string)fileName)));
                    }
                }
            }

            TestSectionDefectListToVMCollection();
        }

        /// <summary>
        /// Permits to convert the received list from the selected wafer to a collection for the canvas, but in the Testing section.
        /// </summary>
        public void TestSectionDefectListToVMCollection()
        {
            if (WaferService.SelectedWafer != null && WaferService.SelectedWafer.DefectsAnnotationsList.Any())
            {
                foreach (DefectAnnotation annotation in WaferService.SelectedWafer.DefectsAnnotationsList)
                {
                    switch (annotation)
                    {
                        case BoundingBox boundingBox:
                            DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<BoundingBoxVM>(boundingBox));
                            break;

                        case PolylineAnnotation polylineAnnotation:
                            DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<PolylineAnnotationVM>(polylineAnnotation));
                            break;

                        case LineAnnotation lineAnnotation:
                            DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<LineAnnotationVM>(lineAnnotation));
                            break;

                        case PolygonAnnotation polygonAnnotation:
                            DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<PolygonAnnotationVM>(polygonAnnotation));
                            break;

                        default:
                            Console.WriteLine("AnnotateWaferLayerViewModel - DefectListToVMCollection(): an error occured.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Permits to populate the DefectsAnnotationsCollection based on the DefectAnnotationsList of EACH wafer. 
        /// Called immediately after deserialisation.
        /// </summary>
        public void DeserialiseDefectListToVMCollection(List<Wafer> wafers, Wafer selectedWafer = null)
        {
            // Upon changing the Selected Wafer
            if (wafers != null && selectedWafer != null)
            {
                WaferService.FillWafersProperties();
                LoadDefectAnnotationsIntoCanvas();
            }
        }

        /// <summary>
        /// When changing Selected Wafers, we clear the canvas from the DefectAnnotations of the previous Selected Wafer, 
        /// and then load the DefectAnnotations into the canvas of the concerned newly Selected Wafer
        /// </summary>
        public void LoadDefectAnnotationsIntoCanvas()
        {
            // Clear the DefectsAnnotation list to display the Defect Annotations of the newly changed SelectedWafer
            DefectsAnnotationsCollection.Clear();

            foreach (DefectAnnotationVM annotation in WaferService.SelectedWafer.DefectsAnnotationsCollection)
            {
                switch (annotation)
                {
                    case BoundingBoxVM boundingBoxVM:
                        DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<BoundingBoxVM>(boundingBoxVM));
                        break;

                    case PolylineAnnotationVM polylineAnnotationVM:
                        DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<PolylineAnnotationVM>(polylineAnnotationVM));
                        break;

                    case LineAnnotationVM lineAnnotationVM:
                        DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<LineAnnotationVM>(lineAnnotationVM));
                        break;

                    case PolygonAnnotationVM polygonAnnotationVM:
                        DefectsAnnotationsCollection.Add(_mapper.AutoMap.Map<PolygonAnnotationVM>(polygonAnnotationVM));
                        break;

                    default:
                        Console.WriteLine("AnnotateWaferLayerViewModel - LoadDefectAnnotationsIntoCanvas(): an error occured.");
                        break;
                }
            }
        }

        /// <summary>
        /// Check if saving condition is fulfilled before wafer changes.
        /// </summary>
        /// <returns></returns>
        public bool CanWaferChange()
        {
            if (DefectsAnnotationsCollection.Any())
            {
                // Do nothing
            }
            return true;
        }


        //====================================================================
        // Events & Dispose
        //====================================================================

        /// <summary>
        /// Event raised  when the Selected Wafer has changed.
        /// </summary>
        private void _mainViewModel_SelectedWaferChanged()
        {
            if (_learningTabViewModel != null && _learningTabViewModel.SelectedWafer != null)
                WaferService.SelectedWafer = _learningTabViewModel.SelectedWafer;

            if (_testTabViewModel != null && _testTabViewModel.SelectedWafer != null)
                WaferService.SelectedWafer = _testTabViewModel.SelectedWafer;

            DeserialiseDefectListToVMCollection(WaferService.DeserialisedWafers, WaferService.SelectedWafer);
        }

        /// <summary>
        /// Event raised  when the Selected Layer has changed.
        /// </summary>
        private void AnnotationModeWaferLayerViewModel_SelectedLayerChanged()
        {
            if (SelectedLayerProperties.Key != null && SelectedLayerProperties.Value != null)
            {
                // Get Layer Img List
                List<MilImage> MilImages = SelectedLayerProperties.Value;

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

        /// <summary>
        /// Event raised when the user switched from a annotation type to another (object or segmentation).
        /// </summary>
        private void ImageAnnotationToolsViewModel_SelectedAnnotationTypeChanged()
        {
            // Do nothing, for now.
        }

        /// <summary>
        /// Event raised when user presses the clear button.
        /// </summary>
        private void ImageAnnotationToolsViewModel_ClearAllPressed(bool isReleased)
        {
            if (WaferService.IsClearCommanded && isReleased)
            {
                // By using Application.Current.Dispatcher.Invoke, ensure collection modifications are performed on the correct UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DefectsAnnotationsCollection.Clear();
                    WaferService.IsClearCommanded = false;
                    Console.WriteLine("All DefectAnnotations on " + WaferService.SelectedWafer.BaseName + " cleared.");
                });
            }

            else if (!isReleased)
                Console.WriteLine("None of the DefectAnnotations on " + WaferService.SelectedWafer.BaseName + " were cleared.");
        }

        private void WafersListingViewModel_DismissPressed(bool isConfirmed)
        {
            if (isConfirmed)
            {
                // By using Application.Current.Dispatcher.Invoke, ensure collection modifications are performed on the correct UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DefectsAnnotationsCollection.Clear();
                    Console.WriteLine("All DefectAnnotations were dismissed.");
                });
            }

            else if (!isConfirmed)
                Console.WriteLine("Nothing was dismissed.");
        }

        /// <summary>
        /// Events related to tool choice and its speciafications.
        /// </summary>
        private void ImageAnnotationToolsViewModel_ToolTypeChanged()
        {
            OnPropertyChanged(nameof(ToolMode));
            OnPropertyChanged(nameof(IsGraphViewVisible));
        }

        private void ImageAnnotationToolsViewModel_SelectedThicknessChanged()
        {
            OnPropertyChanged(nameof(SelectedThickness));
        }

        private void ImageAnnotationToolsViewModel_SelectedCategoryChanged()
        {
            OnPropertyChanged(nameof(SelectedCategory));
        }

        /// <summary>
        /// Unsubscribe to events and free memory.
        /// </summary>
        public void Dispose()
        {
            SelectedLayerChanged -= AnnotationModeWaferLayerViewModel_SelectedLayerChanged;
            ImageAnnotationToolsViewModel.SelectedAnnotationTypeChanged -= ImageAnnotationToolsViewModel_SelectedAnnotationTypeChanged;
            ImageAnnotationToolsViewModel.ClearAllPressed -= ImageAnnotationToolsViewModel_ClearAllPressed;
            ImageAnnotationToolsViewModel.ToolTypeChanged -= ImageAnnotationToolsViewModel_ToolTypeChanged;
            ImageAnnotationToolsViewModel.SelectedThicknessChanged -= ImageAnnotationToolsViewModel_SelectedThicknessChanged;
            ImageAnnotationToolsViewModel.SelectedCategoryChanged -= ImageAnnotationToolsViewModel_SelectedCategoryChanged;
            WafersListingViewModel.DismissPressed -= WafersListingViewModel_DismissPressed;
        }
    }
}
