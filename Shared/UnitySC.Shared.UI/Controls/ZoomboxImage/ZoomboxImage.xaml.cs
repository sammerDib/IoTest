using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

using Xceed.Wpf.Toolkit.Zoombox;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// Une zoombox avec des propriétés et des commandes en plus:
    /// - Scale
    /// - Status
    /// - gestion de la ROI
    /// - ...
    /// </summary>
    public partial class ZoomboxImage : UserControl
    {
        private readonly IDialogOwnerService _dialogService;

        public ZoomboxImage()
        {
            InitializeComponent();
            theZoombox.CurrentViewChanged += (s, e) => ZoomboxView = e.NewValue;
            theZoombox.CurrentViewChanged += (s, e) =>
            {
                if (e.NewValue.ViewKind == ZoomboxViewKind.Absolute)
                {
                    ImageTopLeftPosition = e.NewValue.Position;
                }
            };
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

            HiliteCrossWidth = HiliteCrossSize;
            HiliteCrossHeight = HiliteCrossSize;
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
            DependencyProperty.Register("ImageSource", typeof(BitmapSource), typeof(ZoomboxImage), new PropertyMetadata(null, ImageSource_Changed));

        private static void ImageSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoomboxImage)d;
            if (view.AutoSize)
            {
                if (view.ImageSource == null)
                {
                    view.ImageWidth = view.ImageHeight = 0;
                }
                else
                {
                    view.ImageWidth = view.ImageSource.Width;
                    view.ImageHeight = view.ImageSource.Height;
                }
            }

            if (double.IsNaN(view.VerticalLine1Position))
            {
                view.VerticalLine1Position = view.ImageWidth / 2;
            }

            if (double.IsNaN(view.VerticalLine2Position))
            {
                view.VerticalLine2Position = view.ImageWidth / 2 + view.ImageWidth / 10;
            }

            if (double.IsNaN(view.HorizontalLine1Position))
            {
                view.HorizontalLine1Position = view.ImageHeight / 2;
            }

            if (double.IsNaN(view.HorizontalLine2Position))
            {
                view.HorizontalLine2Position = view.ImageHeight / 2 + view.ImageHeight / 10;
            }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(0.0));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(0.0));

        public Point ImageTopLeftPosition
        {
            get => (Point)GetValue(ImageTopLeftPositionProperty);
            set => SetValue(ImageTopLeftPositionProperty, value);
        }

        public static readonly DependencyProperty ImageTopLeftPositionProperty =
            DependencyProperty.Register("ImageTopLeftPosition", typeof(Point), typeof(ZoomboxImage),
                new PropertyMetadata(default(Point)));

        public Point ContentPosition
        {
            get => throw new NotImplementedException();
            set
            {
                if (theZoombox.CurrentView.ViewKind != ZoomboxViewKind.Absolute)
                {
                    ImageTopLeftPosition = new Point(value.X * Scale, value.Y * Scale);
                }
            }
        }

        /// <summary>
        /// Text indiquant les infos selon la position de la souris
        /// </summary>
        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(ZoomboxImage), new PropertyMetadata(null));

        public bool HasRoi
        {
            get { return (bool)GetValue(HasRoiProperty); }
            set { SetValue(HasRoiProperty, value); }
        }

        public static readonly DependencyProperty HasRoiProperty =
            DependencyProperty.Register("HasRoi", typeof(bool), typeof(ZoomboxImage), new PropertyMetadata(false));

        public Visibility RoiVisibility
        {
            get { return (Visibility)GetValue(RoiVisibilityProperty); }
            set { SetValue(RoiVisibilityProperty, value); }
        }

        public static readonly DependencyProperty RoiVisibilityProperty =
            DependencyProperty.Register("RoiVisibility", typeof(Visibility), typeof(ZoomboxImage), new PropertyMetadata(Visibility.Hidden));

        public Visibility HiliteVisibility
        {
            get { return (Visibility)GetValue(HiliteVisibilityProperty); }
            set { SetValue(HiliteVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HiliteVisibilityProperty =
            DependencyProperty.Register("HiliteVisibility", typeof(Visibility), typeof(ZoomboxImage), new PropertyMetadata(Visibility.Hidden, HiliteVisibility_Changed));

        private static void HiliteVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoomboxImage)d;
            if (view.HiliteVisibility == Visibility.Visible)
            {
                ScaleZoomBox_Changed(d, e);
            }
        }

        public bool IsHiliteVisible
        {
            get { return (bool)GetValue(IsHiliteVisibleProperty); }
            set { SetValue(IsHiliteVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsHiliteVisibleProperty =
            DependencyProperty.Register("IsHiliteVisible", typeof(bool), typeof(ZoomboxImage), new PropertyMetadata(true));

        public Visibility AlignmentLineHorizontalVisibility
        {
            get { return (Visibility)GetValue(AlignmentLineHorizontalVisibilityProperty); }
            set { SetValue(AlignmentLineHorizontalVisibilityProperty, value); }
        }

        public static readonly DependencyProperty AlignmentLineHorizontalVisibilityProperty =
            DependencyProperty.Register("AlignmentLineHorizontalVisibility", typeof(Visibility), typeof(ZoomboxImage), new PropertyMetadata(Visibility.Hidden));

        public Visibility AlignmentLineVerticalVisibility
        {
            get { return (Visibility)GetValue(AlignmentLineVerticalVisibilityProperty); }
            set { SetValue(AlignmentLineVerticalVisibilityProperty, value); }
        }

        public static readonly DependencyProperty AlignmentLineVerticalVisibilityProperty =
            DependencyProperty.Register("AlignmentLineVerticalVisibility", typeof(Visibility), typeof(ZoomboxImage), new PropertyMetadata(Visibility.Hidden));

        public bool AutoSize
        {
            get { return (bool)GetValue(AutoSizeProperty); }
            set { SetValue(AutoSizeProperty, value); }
        }

        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.Register("AutoSize", typeof(bool), typeof(ZoomboxImage), new PropertyMetadata(true));

        //-----------------------------------------------------------------
        // Gestion du zoom
        //-----------------------------------------------------------------
        public ZoomboxView InitialZoomboxView
        {
            get { return (ZoomboxView)GetValue(InitialZoomboxViewProperty); }
            set { SetValue(InitialZoomboxViewProperty, value); }
        }

        public static readonly DependencyProperty InitialZoomboxViewProperty =
            DependencyProperty.Register("InitialZoomboxView", typeof(ZoomboxView), typeof(ZoomboxImage), new PropertyMetadata(null));

        public ZoomboxView ZoomboxView
        {
            get { return (ZoomboxView)GetValue(ZoomboxViewProperty); }
            set { SetValue(ZoomboxViewProperty, value); }
        }

        public static readonly DependencyProperty ZoomboxViewProperty =
            DependencyProperty.Register("ZoomboxView", typeof(ZoomboxView), typeof(ZoomboxImage), new PropertyMetadata(null));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(0.0, ScaleZoomBox_Changed));

        private static void ScaleZoomBox_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoomboxImage)d;
            view.HiliteCrossWidth = view.HiliteCrossSize / view.Scale;
            view.HiliteCrossHeight = view.HiliteCrossSize / view.Scale;
            view.ScaledLinesThickness = view.LinesThickness / view.Scale;

            // HiliteCross_Changed(d, e); // already called in Hilite changed

            Hilite_Changed(d, e);
        }

        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(100.0));

        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(0.001));

        //-----------------------------------------------------------------
        // ROI
        //-----------------------------------------------------------------
        public RoiType RoiType
        {
            get { return (RoiType)GetValue(RoiTypeProperty); }
            set { SetValue(RoiTypeProperty, value); }
        }

        public static readonly DependencyProperty RoiTypeProperty =
            DependencyProperty.Register("RoiType", typeof(RoiType), typeof(ZoomboxImage), new PropertyMetadata(RoiType.Rectangle));

        public int RoiX
        {
            get { return (int)GetValue(RoiXProperty); }
            set { SetValue(RoiXProperty, value); }
        }

        public static readonly DependencyProperty RoiXProperty =
            DependencyProperty.Register("RoiX", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, ROI_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int RoiMaxX
        {
            get { return (int)GetValue(RoiMaxXProperty); }
            set { SetValue(RoiMaxXProperty, value); }
        }

        public static readonly DependencyProperty RoiMaxXProperty =
            DependencyProperty.Register("RoiMaxX", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int RoiY
        {
            get { return (int)GetValue(RoiYProperty); }
            set { SetValue(RoiYProperty, value); }
        }

        public static readonly DependencyProperty RoiYProperty =
            DependencyProperty.Register("RoiY", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, ROI_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int RoiMaxY
        {
            get { return (int)GetValue(RoiMaxYProperty); }
            set { SetValue(RoiMaxYProperty, value); }
        }

        public static readonly DependencyProperty RoiMaxYProperty =
            DependencyProperty.Register("RoiMaxY", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int RoiWidth
        {
            get { return (int)GetValue(RoiWidthProperty); }
            set { SetValue(RoiWidthProperty, value); }
        }

        public static readonly DependencyProperty RoiWidthProperty =
            DependencyProperty.Register("RoiWidth", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, ROI_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int RoiMaxWidth
        {
            get { return (int)GetValue(RoiMaxWidthProperty); }
            set { SetValue(RoiMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty RoiMaxWidthProperty =
            DependencyProperty.Register("RoiMaxWidth", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int RoiHeight
        {
            get { return (int)GetValue(RoiHeightProperty); }
            set { SetValue(RoiHeightProperty, value); }
        }

        public static readonly DependencyProperty RoiHeightProperty =
            DependencyProperty.Register("RoiHeight", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, ROI_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int RoiMaxHeight
        {
            get { return (int)GetValue(RoiMaxHeightProperty); }
            set { SetValue(RoiMaxHeightProperty, value); }
        }

        public static readonly DependencyProperty RoiMaxHeightProperty =
            DependencyProperty.Register("RoiMaxHeight", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public object Overlay
        {
            get { return GetValue(OverlayProperty); }
            set { SetValue(OverlayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Overlay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlayProperty =
            DependencyProperty.Register("Overlay", typeof(object), typeof(ZoomboxImage), new PropertyMetadata(null));

        private static void ROI_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoomboxImage)d;
            view.RoiMaxX = (int)view.ImageWidth - view.RoiWidth;
            view.RoiMaxY = (int)view.ImageHeight - view.RoiHeight;
            view.RoiMaxWidth = (int)view.ImageWidth - view.RoiX;
            view.RoiMaxHeight = (int)view.ImageHeight - view.RoiY;
            view.HasRoi = view.RoiWidth > 0 && view.RoiHeight > 0;
        }



        public double LinesThickness
        {
            get { return (double)GetValue(LinesThicknessProperty); }
            set { SetValue(LinesThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinesThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinesThicknessProperty =
            DependencyProperty.Register("LinesThickness", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(1d));


        public double ScaledLinesThickness
        {
            get { return (double)GetValue(ScaledLinesThicknessProperty); }
            set { SetValue(ScaledLinesThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinesThumbThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaledLinesThicknessProperty =
            DependencyProperty.Register("ScaledLinesThickness", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(5d, ScaledLinesThicknessChanged));

        private static void ScaledLinesThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoomboxImage)d;
            view.ScaledLinesThumbThickness = 10.0 / view.Scale;
        }



        public double ScaledLinesThumbThickness
        {
            get { return (double)GetValue(ScaledLinesThumbThicknessProperty); }
            set { SetValue(ScaledLinesThumbThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinesThumbThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaledLinesThumbThicknessProperty =
            DependencyProperty.Register("ScaledLinesThumbThickness", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(5d));



        public double VerticalLine1Position
        {
            get { return (double)GetValue(VerticalLine1PositionProperty); }
            set { SetValue(VerticalLine1PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalLine1Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalLine1PositionProperty =
            DependencyProperty.Register("VerticalLine1Position", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(double.NaN));



        public double VerticalLine2Position
        {
            get { return (double)GetValue(VerticalLine2PositionProperty); }
            set { SetValue(VerticalLine2PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalLine2Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalLine2PositionProperty =
            DependencyProperty.Register("VerticalLine2Position", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(double.NaN));



        public double HorizontalLine1Position
        {
            get { return (double)GetValue(HorizontalLine1PositionProperty); }
            set { SetValue(HorizontalLine1PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalLine1Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalLine1PositionProperty =
            DependencyProperty.Register("HorizontalLine1Position", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(double.NaN));




        public double HorizontalLine2Position
        {
            get { return (double)GetValue(HorizontalLine2PositionProperty); }
            set { SetValue(HorizontalLine2PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalLine2Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalLine2PositionProperty =
            DependencyProperty.Register("HorizontalLine2Position", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(double.NaN));



        public Brush LinesBrush
        {
            get { return (Brush)GetValue(LinesBrushProperty); }
            set { SetValue(LinesBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinesBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinesBrushProperty =
            DependencyProperty.Register("LinesBrush", typeof(Brush), typeof(ZoomboxImage), new PropertyMetadata(new SolidColorBrush(Colors.LightGreen)));






        //-----------------------------------------------------------------
        // HILITE - rectangle FRAME on image dedicated to Emcompass and hilight a zone of image (such as a dfect for example)
        //-----------------------------------------------------------------

        public int HiliteX
        {
            get { return (int)GetValue(HiliteXProperty); }
            set { SetValue(HiliteXProperty, value); }
        }

        public static readonly DependencyProperty HiliteXProperty =
            DependencyProperty.Register("HiliteX", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, Hilite_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int HiliteMaxX
        {
            get { return (int)GetValue(HiliteMaxXProperty); }
            set { SetValue(HiliteMaxXProperty, value); }
        }

        public static readonly DependencyProperty HiliteMaxXProperty =
            DependencyProperty.Register("HiliteMaxX", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int HiliteY
        {
            get { return (int)GetValue(HiliteYProperty); }
            set { SetValue(HiliteYProperty, value); }
        }

        public static readonly DependencyProperty HiliteYProperty =
            DependencyProperty.Register("HiliteY", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, Hilite_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int HiliteMaxY
        {
            get { return (int)GetValue(HiliteMaxYProperty); }
            set { SetValue(HiliteMaxYProperty, value); }
        }

        public static readonly DependencyProperty HiliteMaxYProperty =
            DependencyProperty.Register("HiliteMaxY", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int HiliteWidth
        {
            get { return (int)GetValue(HiliteWidthProperty); }
            set { SetValue(HiliteWidthProperty, value); }
        }

        public static readonly DependencyProperty HiliteWidthProperty =
            DependencyProperty.Register("HiliteWidth", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, Hilite_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int HiliteMaxWidth
        {
            get { return (int)GetValue(HiliteMaxWidthProperty); }
            set { SetValue(HiliteMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty HiliteMaxWidthProperty =
            DependencyProperty.Register("HiliteMaxWidth", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int HiliteHeight
        {
            get { return (int)GetValue(HiliteHeightProperty); }
            set { SetValue(HiliteHeightProperty, value); }
        }

        public static readonly DependencyProperty HiliteHeightProperty =
            DependencyProperty.Register("HiliteHeight", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0, Hilite_Changed));

        /// <summary> Cette propriété est utilisée ne interne par la ZoomboxImage, ne pas la binder depuis l'extérieur </summary>
        public int HiliteMaxHeight
        {
            get { return (int)GetValue(HiliteMaxHeightProperty); }
            set { SetValue(HiliteMaxHeightProperty, value); }
        }

        public static readonly DependencyProperty HiliteMaxHeightProperty =
            DependencyProperty.Register("HiliteMaxHeight", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int HiliteBorderX { get { return (int)GetValue(HiliteBorderXProperty); } set { SetValue(HiliteBorderXProperty, value); } }
        public static readonly DependencyProperty HiliteBorderXProperty = DependencyProperty.Register("HiliteBorderX", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));
        public int HiliteBorderY { get { return (int)GetValue(HiliteBorderYProperty); } set { SetValue(HiliteBorderYProperty, value); } }
        public static readonly DependencyProperty HiliteBorderYProperty = DependencyProperty.Register("HiliteBorderY", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));
        public int HiliteBorderWidth { get { return (int)GetValue(HiliteBorderWidthProperty); } set { SetValue(HiliteBorderWidthProperty, value); } }
        public static readonly DependencyProperty HiliteBorderWidthProperty = DependencyProperty.Register("HiliteBorderWidth", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));
        public int HiliteBorderHeight { get { return (int)GetValue(HiliteBorderHeightProperty); } set { SetValue(HiliteBorderHeightProperty, value); } }
        public static readonly DependencyProperty HiliteBorderHeightProperty = DependencyProperty.Register("HiliteBorderHeight", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        private static void Hilite_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoomboxImage)d;
            view.HiliteMaxX = (int)view.ImageWidth - view.HiliteWidth;
            view.HiliteMaxY = (int)view.ImageHeight - view.HiliteHeight;
            view.HiliteMaxWidth = (int)view.ImageWidth - view.HiliteX;
            view.HiliteMaxHeight = (int)view.ImageHeight - view.HiliteY;
            // view.HasHilite = (view.HiliteWidth > 0 && view.HiliteHeight > 0);

            double hiliteborderthickness = ContentResizerControl.Cst_SideThumb_px / view.Scale;
            int nBorder = (int)Math.Round(3.0 * hiliteborderthickness);
            view.HiliteBorderX = view.HiliteX - nBorder - 1;
            view.HiliteBorderY = view.HiliteY - nBorder - 1;
            view.HiliteBorderWidth = view.HiliteWidth + 1 + 2 * nBorder;
            view.HiliteBorderHeight = view.HiliteHeight + 1 + 2 * nBorder;

            HiliteCross_Changed(d, e);
        }

        private static void HiliteCross_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ZoomboxImage)d;
            view.HiliteCrossX = view.HiliteX - (int)(view.HiliteCrossWidth * 0.5) + view.HiliteWidth / 2;
            view.HiliteCrossY = view.HiliteY - (int)(view.HiliteCrossHeight * 0.5) + view.HiliteHeight / 2;

            view.IsHiliteVisible = Math.Max(view.HiliteWidth, view.HiliteHeight) * view.Scale >= view.HiliteModeSwitchSize;
        }

        public int HiliteCrossX
        {
            get { return (int)GetValue(HiliteCrossXProperty); }
            set { SetValue(HiliteCrossXProperty, value); }
        }

        public static readonly DependencyProperty HiliteCrossXProperty =
            DependencyProperty.Register("HiliteCrossX", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public int HiliteCrossY
        {
            get { return (int)GetValue(HiliteCrossYProperty); }
            set { SetValue(HiliteCrossYProperty, value); }
        }

        public static readonly DependencyProperty HiliteCrossYProperty =
            DependencyProperty.Register("HiliteCrossY", typeof(int), typeof(ZoomboxImage), new PropertyMetadata(0));

        public double HiliteCrossWidth
        {
            get { return (double)GetValue(HiliteCrossWidthProperty); }
            set { SetValue(HiliteCrossWidthProperty, value); }
        }

        public static readonly DependencyProperty HiliteCrossWidthProperty =
            DependencyProperty.Register("HiliteCrossWidth", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(0.0));

        public double HiliteCrossHeight
        {
            get { return (double)GetValue(HiliteCrossHeightProperty); }
            set { SetValue(HiliteCrossHeightProperty, value); }
        }

        public static readonly DependencyProperty HiliteCrossHeightProperty =
            DependencyProperty.Register("HiliteCrossHeight", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(0.0));

        public double HiliteCrossSize
        {
            get { return (double)GetValue(HiliteCrossSizeProperty); }
            set { SetValue(HiliteCrossSizeProperty, value); }
        }

        public static readonly DependencyProperty HiliteCrossSizeProperty =
            DependencyProperty.Register("HiliteCrossSize", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(64.0));

        public double HiliteModeSwitchSize
        {
            get { return (double)GetValue(HiliteModeSwitchSizeProperty); }
            set { SetValue(HiliteModeSwitchSizeProperty, value); }
        }

        public static readonly DependencyProperty HiliteModeSwitchSizeProperty =
            DependencyProperty.Register("HiliteModeSwitchSize", typeof(double), typeof(ZoomboxImage), new PropertyMetadata(24.0));

        //=================================================================
        //  mouse Commands
        //=================================================================

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(ZoomboxImage), new PropertyMetadata(null));

        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(ZoomboxImage), new PropertyMetadata(null));

        //=================================================================
        // Routed Commands
        //=================================================================
        public static readonly RoutedCommand ZoomToRoi = new RoutedCommand();

        public static readonly RoutedCommand SetRoiFromView = new RoutedCommand();

        static ZoomboxImage()
        {
            // On rend accessibles certaines commandes de la Zoombox
            //......................................................
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoomboxImage),
                new CommandBinding(Zoombox.ZoomIn,
                    (s, e) => Zoombox.ZoomIn.Execute(e, ((ZoomboxImage)s).theZoombox),
                    (s, e) => e.CanExecute = Zoombox.ZoomIn.CanExecute(e, ((ZoomboxImage)s).theZoombox)
                    ));
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoomboxImage),
                new CommandBinding(Zoombox.ZoomOut,
                    (s, e) => Zoombox.ZoomOut.Execute(e, ((ZoomboxImage)s).theZoombox),
                    (s, e) => e.CanExecute = Zoombox.ZoomOut.CanExecute(e, ((ZoomboxImage)s).theZoombox)
                    ));
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoomboxImage),
                new CommandBinding(Zoombox.Fit,
                    (s, e) => Zoombox.Fit.Execute(e, ((ZoomboxImage)s).theZoombox),
                    (s, e) => e.CanExecute = Zoombox.Fit.CanExecute(e, ((ZoomboxImage)s).theZoombox)
                    ));

            // On ajoute d'autres commandes
            //.............................
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoomboxImage),
                new CommandBinding(ZoomToRoi, ZoomToRoi_ExecutedRoutedEventHandler)
                );
            CommandManager.RegisterClassCommandBinding(
                typeof(ZoomboxImage),
                new CommandBinding(SetRoiFromView, SetRoiFromView_ExecutedRoutedEventHandler)
                );
        }

        private static void ZoomToRoi_ExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var view = (ZoomboxImage)sender;
            var zoombox = view.theZoombox;
            var r = new Rect(view.RoiX, view.RoiY, view.RoiWidth, view.RoiHeight);
            var vp = zoombox.Viewport;

            // On essaye de faire en sorte que la nouvelle zone visible (r) ait le même ratio largeur/hauteur
            // que la ROI sinon la zoombox le digère mal.
            if (vp.Width != 0 && vp.Height != 0)
            {
                double aspect = vp.Width / vp.Height;
                double ratioX = view.RoiWidth / vp.Width;
                double ratioY = view.RoiHeight / vp.Height;
                double w, h;
                if (ratioX < ratioY)
                {
                    w = view.RoiHeight * aspect;
                    h = view.RoiHeight;
                }
                else
                {
                    w = view.RoiWidth;
                    h = view.RoiWidth / aspect;
                }
                w *= 1.1;
                h *= 1.1;
                double x = view.RoiX + view.RoiWidth / 2.0 - w / 2.0;
                double y = view.RoiY + view.RoiHeight / 2.0 - h / 2.0;
                r = new Rect(x, y, w, h);
            }
            zoombox.ZoomTo(r);

            // Les scrollbar de la Zoombox occultent le contenu
            // On essaie de compenser ce bug
            if (zoombox.IsUsingScrollBars)
            {
                r.Width += ScroolBarWidth / zoombox.Scale;
                r.Height += ScroolBarWidth / zoombox.Scale;
            }
            zoombox.ZoomTo(r);
        }

        public static void SetRoiFromView_ExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var view = (ZoomboxImage)sender;
            var zoombox = view.theZoombox;
            var r = zoombox.Viewport;

            // Les scrollbar de la Zoombox occultent le contenu
            // On essaie de compenser ce bug
            if (zoombox.IsUsingScrollBars)
            {
                r.Width -= ScroolBarWidth / zoombox.Scale;
                r.Height -= ScroolBarWidth / zoombox.Scale;
            }

            // On s'assure que la ROI n'est pas plus grande que l'image
            if (r.X < 0)
                r.X = 0;
            if (r.Y < 0)
                r.Y = 0;
            if (r.X + r.Width > view.ImageWidth)
                r.Width = (int)view.ImageWidth - r.X;
            if (r.Y + r.Height > view.ImageHeight)
                r.Height = (int)view.ImageHeight - r.Y;
            view.RoiX = (int)r.X;
            view.RoiY = (int)r.Y;
            view.RoiWidth = (int)r.Width;
            view.RoiHeight = (int)r.Height;
        }

        /// <summary>
        /// Largeur des scrollbars de la zoombox qui occultent l'image :-(
        /// </summary>
        private const int ScroolBarWidth = 17;

        //=================================================================
        // Event Callbacks
        //=================================================================

        private void TheView_Loaded(object sender, RoutedEventArgs e)
        {
            SetInitialZoom();
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetInitialZoom();
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
                var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitySC.Shared.UI.Resource.ViewFinder.png");
                BitmapSource bmp = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                showViewFinderGlyphImage.Source = bmp;
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var mousepos = e.GetPosition(theImage);
            if (ImageSource != null)
            {
                int x = (int)(mousepos.X * ImageSource.Width / theImage.ActualWidth);
                int y = (int)(mousepos.Y * ImageSource.Height / theImage.ActualHeight);
                bool is8BitsGreyColor = ImageSource.Format.BitsPerPixel == 8;

                if (0 <= x && x < ImageSource.Width && 0 <= y && y < ImageSource.Height)
                {
                    var c = ImageSource.GetPixelColor(x, y);
                    if (is8BitsGreyColor)
                        StatusText = $"X: {(int)mousepos.X}    Y: {(int)mousepos.Y}    Grey: {c.B}";
                    else
                    {
                        StatusText = $"X: {(int)mousepos.X}    Y: {(int)mousepos.Y}    Color: R={c.R} G={c.G} B={c.B}";
                        if (c.A != 255)
                            StatusText += $" Alpha={c.A}";
                    }
                }
                else
                    StatusText = string.Empty; //$"outside !! ----> X: {(int)mousepos.X}    Y: {(int)mousepos.Y}";
            }
            else
                StatusText = string.Empty; //"ImageSource == null -----> ";
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            StatusText = string.Empty; //"LEAVE";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageSource is null)
                return;

            if (theImage is null)
                return;

            if (e.ChangedButton == MouseButton.Left)
            {
                var mousepos = e.GetPosition(theImage);
                var mouseposfix = new Point(mousepos.X * ImageSource.Width / theImage.ActualWidth, mousepos.Y * ImageSource.Height / theImage.ActualHeight);

                if (e.ClickCount == 2)
                {
                    // single click event
                    DoubleClickCommand?.Execute(mouseposfix);
                }
                else if (e.ClickCount == 1)
                {
                    // single click event
                    ClickCommand?.Execute(mouseposfix);
                }
            }
        }

        //-----------------------------------------------------------------
        // Menu contextuel
        //-----------------------------------------------------------------
        private int _count = 0;

        private void OpenExternally_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string viewer = ConfigurationManager.AppSettings.Get("Debug.ImageViewer");
                if (viewer == null)
                    viewer = @"C:\Program Files\ImageJ\ImageJ.exe";

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
                System.Diagnostics.Process.Start(viewer, filename);
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

        /// <summary>
        /// Définit le zoom par défaut au démarrage
        /// </summary>
        private void SetInitialZoom()
        {
            // Attention, il ne faut le  faire qu'une fois et au bon momment
            if (IsLoaded && ImageWidth != 0 && ImageHeight != 0)
            {
                if (InitialZoomboxView == null)
                {
                    theZoombox.MinScale = 0;
                    theZoombox.FitToBounds();
                    theZoombox.MinScale = theZoombox.Scale;
                }
                else
                {
                    theZoombox.ZoomTo(InitialZoomboxView);
                    theZoombox.MinScale = Math.Min(theZoombox.Width / ImageWidth, theZoombox.Height / ImageHeight);
                }
            }
        }

        private void Image_Unloaded(object sender, RoutedEventArgs e)
        {
            ImageSource = null;
        }
    }
}
