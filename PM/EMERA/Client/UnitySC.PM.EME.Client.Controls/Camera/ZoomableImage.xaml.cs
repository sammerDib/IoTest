using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI;
using UnitySC.Shared.UI.Dialog;

using Xceed.Wpf.Toolkit.Zoombox;

namespace UnitySC.PM.EME.Client.Controls.Camera
{
    public partial class ZoombableImage : UserControl
    {
        private readonly IDialogOwnerService _dialogService;

        public ZoombableImage()
        {
            InitializeComponent();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
        }

        //=================================================================
        // Dependency properties
        //=================================================================

        public BitmapSource ImageSource
        {
            get { return (BitmapSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapSource), typeof(ZoombableImage), new PropertyMetadata(null));

        public int CanvasWidth
        {
            get { return (int)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        public static readonly DependencyProperty CanvasWidthProperty =
            DependencyProperty.Register("CanvasWidth", typeof(int), typeof(ZoombableImage), new PropertyMetadata(0));

        public int CanvasHeight
        {
            get { return (int)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        public static readonly DependencyProperty CanvasHeightProperty =
            DependencyProperty.Register("CanvasHeight", typeof(int), typeof(ZoombableImage), new PropertyMetadata(0));

        public Point ContentPosition
        {
            get => (Point)GetValue(ContentPositionProperty);
            set => SetValue(ContentPositionProperty, value);
        }

        public static readonly DependencyProperty ContentPositionProperty =
            DependencyProperty.Register("ContentPosition", typeof(Point), typeof(ZoombableImage),
                new PropertyMetadata(default(Point), OnContentUpdate));

        public bool UseRoi
        {
            get { return (bool)GetValue(UseRoiProperty); }
            set { SetValue(UseRoiProperty, value); }
        }

        public static readonly DependencyProperty UseRoiProperty =
            DependencyProperty.Register(nameof(UseRoi), typeof(bool), typeof(ZoombableImage), new PropertyMetadata(false));

        private static void OnContentUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoombableImage)d;
            if (view.ImageSource == null)
            {
                return;
            }

            var viewPortion = view.theZoombox.Viewport;
            view.CanvasTopLeftPosition = new Point(-1 * viewPortion.X * view.Scale, -1 * viewPortion.Y * view.Scale);

            view.ImagePortion = new Int32Rect()
            {
                X = (int)(viewPortion.X >= 0 ? viewPortion.X : 0),
                Y = (int)(viewPortion.Y >= 0 ? viewPortion.Y : 0),
                Width = (int)(viewPortion.Width <= view.CanvasWidth ? viewPortion.Width : view.CanvasWidth),
                Height = (int)(viewPortion.Height <= view.CanvasHeight ? viewPortion.Height : view.CanvasHeight)
            };
        }

        public Point CanvasTopLeftPosition
        {
            get => (Point)GetValue(CanvasTopLeftPositionProperty);
            set => SetValue(CanvasTopLeftPositionProperty, value);
        }

        public static readonly DependencyProperty CanvasTopLeftPositionProperty =
            DependencyProperty.Register("CanvasTopLeftPosition", typeof(Point), typeof(ZoombableImage),
                new PropertyMetadata(default(Point)));

        public Int32Rect ImageCropArea
        {
            get => (Int32Rect)GetValue(ImageCropAreaProperty);
            set => SetValue(ImageCropAreaProperty, value);
        }

        public static readonly DependencyProperty ImageCropAreaProperty =
            DependencyProperty.Register("ImageCropArea", typeof(Int32Rect), typeof(ZoombableImage),
                new PropertyMetadata(default(Int32Rect)));

        public Int32Rect ImagePortion
        {
            get => (Int32Rect)GetValue(ImagePortionProperty);
            set => SetValue(ImagePortionProperty, value);
        }

        public static readonly DependencyProperty ImagePortionProperty =
            DependencyProperty.Register("ImagePortion", typeof(Int32Rect), typeof(ZoombableImage),
                new PropertyMetadata(default(Int32Rect)));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ZoombableImage), new PropertyMetadata(0.0));

        public Rect RoiRect
        {
            get { return (Rect)GetValue(RoiRectProperty); }
            set { SetValue(RoiRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoiRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoiRectProperty =
            DependencyProperty.Register(nameof(RoiRect), typeof(Rect), typeof(ZoombableImage), new PropertyMetadata(new Rect(0, 0, 0, 0)));

        //=================================================================
        // Routed Commands
        //=================================================================

        static ZoombableImage()
        {
            // On rend accessibles certaines commandes de la Zoombox
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoombableImage),
                new CommandBinding(Zoombox.ZoomIn,
                    (s, e) => Zoombox.ZoomIn.Execute(e, ((ZoombableImage)s).theZoombox),
                    (s, e) => e.CanExecute = Zoombox.ZoomIn.CanExecute(e, ((ZoombableImage)s).theZoombox)
                    ));
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoombableImage),
                new CommandBinding(Zoombox.ZoomOut,
                    (s, e) => Zoombox.ZoomOut.Execute(e, ((ZoombableImage)s).theZoombox),
                    (s, e) => e.CanExecute = Zoombox.ZoomOut.CanExecute(e, ((ZoombableImage)s).theZoombox)
                    ));
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoombableImage),
                new CommandBinding(Zoombox.Fit,
                    (s, e) => Zoombox.Fit.Execute(e, ((ZoombableImage)s).theZoombox),
                    (s, e) => e.CanExecute = Zoombox.Fit.CanExecute(e, ((ZoombableImage)s).theZoombox)
                    ));
        }

        //=================================================================
        // Event Callbacks
        //=================================================================

        private void Fit_OnViewLoaded(object sender, RoutedEventArgs e)
        {
            SetInitialZoom();
        }

        private void Fit_OnImageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetInitialZoom();
        }

        private void UpdateMinScale_WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ImageSource == null)
            {
                return;
            }

            theZoombox.MinScale = Math.Min(e.NewSize.Width / CanvasWidth, e.NewSize.Height / CanvasHeight);
        }

        private bool _zoomBoxLoaded;

        private void TheZoombox_Loaded(object sender, RoutedEventArgs e)
        {
            if (_zoomBoxLoaded)
                return;
            _zoomBoxLoaded = true;

            var showViewFinderGlyphImage = (Image)theZoombox.FindVisualChildByName("ShowViewFinderGlyphImage");
            if (showViewFinderGlyphImage != null)
            {
                var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitySC.PM.EME.Client.Controls.Resource.ViewFinder.png");
                showViewFinderGlyphImage.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        private void SetInitialZoom()
        {
            if (!IsLoaded || ImageSource == null)
            {
                return;
            }

            theZoombox.MinScale = 0;
            theZoombox.FitToBounds();
            theZoombox.MinScale = theZoombox.Scale;
        }

        //-----------------------------------------------------------------
        // Menu contextuel
        //-----------------------------------------------------------------

        private int _count = 0;

        private void OpenExternally_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filename = ImageSource.ToString();
                if (filename.StartsWith("file:///"))
                {
                    filename = new Uri(filename).AbsolutePath;
                }
                else
                {
                    filename = @"c:\temp\fdpsd-" + _count++ + ".tif";
                    ImageSource.Save(filename);
                }
                System.Diagnostics.Process.Start(filename);
            }
            catch (Exception ex)
            {
                _dialogService.ShowException(ex, "Failed to export image to external viewer");
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var settingsSaveFile = new SaveFileDialogSettings
            {
                Filter = "BMP file (*.bmp)|*.bmp|Tiff file (*.tiff)|*.tiff;*.tif|Png file (*.png)|*.png|All files (*.*)|*.*",
                OverwritePrompt = true,
                CheckFileExists = false,
                AddExtension = true
            };

            try
            {
                if (_dialogService.ShowSaveFileDialog(settingsSaveFile) == true)
                {
                    BusyHourglass.SetBusyState();
                    ImageSource.Save(settingsSaveFile.FileName);
                }
            }
            catch (Exception ex)
            {
                string msg = "Failed to save \"" + settingsSaveFile.FileName + "\"";
                _dialogService.ShowException(ex, msg);
            }
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(ImageSource);
        }
    }
}
