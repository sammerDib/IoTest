using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.Controls.NavigationControls
{
    public class WaferViewPortControl : FrameworkElement
    {
        private WaferVisual _waferVisual;
        private DrawingVisual _currentPosVisual;

        private readonly VisualCollection _visualCollection;

        //   private static WaferDimensionalCharacteristic _designWaferCharac;

        //Field of view in mm

        private bool _isDraggingViewPort;
        private Size _dragOffset;


        static WaferViewPortControl()
        {
            //_designWaferCharac = new WaferDimensionalCharacteristic();
            //_designWaferCharac.Diameter = 300;
        }

        public WaferViewPortControl()
        {
            _visualCollection = new VisualCollection(this);
            _waferVisual = new WaferVisual();
            _visualCollection.Add(_waferVisual);
            _currentPosVisual = new DrawingVisual();
            _visualCollection.Add(_currentPosVisual);

            

            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            if (designTime)
            {
                return;
            }


            Loaded += WaferViewPortControl_Loaded;
            Unloaded += WaferViewPortControl_Unloaded;
        }

        private void WaferViewPortControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= WaferViewPortControl_Unloaded;
            this.MouseDown -= WaferViewPortControl_MouseDown; 
            this.MouseUp -= WaferViewPortControl_MouseUp;
            this.MouseMove -= WaferViewPortControl_MouseMove;
        }

        private void WaferViewPortControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WaferViewPortControl_Loaded;
            this.MouseDown += WaferViewPortControl_MouseDown;
            this.MouseUp += WaferViewPortControl_MouseUp;
            this.MouseMove += WaferViewPortControl_MouseMove;
        }

       

        private void WaferViewPortControl_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this);
            if (IsOverViewPort(pos))
            { 
                Cursor = Cursors.SizeAll; 
            }
            else
            {
                Cursor = Cursors.Arrow;
            }



            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed && _isDraggingViewPort)
            {
                MovedViewPort = new Rect(pos.X / this.ActualWidth- _dragOffset.Width, pos.Y / this.ActualHeight-_dragOffset.Height, ViewPort.Width, ViewPort.Height);
            }

        }

        private void WaferViewPortControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);

            

            if (IsOverViewPort(pos))
            {
                _isDraggingViewPort = true;
                _dragOffset = new Size(pos.X / ActualWidth - ViewPort.Left, pos.Y / ActualHeight - ViewPort.Top);
                CaptureMouse();

            }
  
        }

        private void WaferViewPortControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private bool IsOverViewPort(Point pos)
        {
            return ViewPort.Contains(new Point(pos.X/ActualWidth,pos.Y/ActualHeight));
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferDimention.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new WaferDimensionalCharacteristic() { Diameter = 300.Millimeters() }, FrameworkPropertyMetadataOptions.AffectsRender));

        public Length EdgeExclusionThickness
        {
            get { return (Length)GetValue(EdgeExclusionThicknessProperty); }
            set { SetValue(EdgeExclusionThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionThicknessProperty =
            DependencyProperty.Register(nameof(EdgeExclusionThickness), typeof(Length), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(5.Millimeters(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush EdgeExclusionBrush
        {
            get { return (Brush)GetValue(EdgeExclusionBrushProperty); }
            set { SetValue(EdgeExclusionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionBrushProperty =
            DependencyProperty.Register(nameof(EdgeExclusionBrush), typeof(Brush), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush WaferBrush
        {
            get { return (Brush)GetValue(WaferBrushProperty); }
            set { SetValue(WaferBrushProperty, value); }
        }

        public static readonly DependencyProperty WaferBrushProperty =
            DependencyProperty.Register(nameof(WaferBrush), typeof(Brush), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen WaferBorderPen
        {
            get { return (Pen)GetValue(WaferBorderPenProperty); }
            set { SetValue(WaferBorderPenProperty, value); }
        }

        public static readonly DependencyProperty WaferBorderPenProperty =
            DependencyProperty.Register(nameof(WaferBorderPen), typeof(Pen), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen CurrentPosBorderPen
        {
            get { return (Pen)GetValue(CurrentPosBorderPenProperty); }
            set { SetValue(CurrentPosBorderPenProperty, value); }
        }

        public static readonly DependencyProperty CurrentPosBorderPenProperty =
            DependencyProperty.Register(nameof(CurrentPosBorderPen), typeof(Pen), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Yellow), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush CurrentPosBrush
        {
            get { return (Brush)GetValue(CurrentPosBrushProperty); }
            set { SetValue(CurrentPosBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPosBrushProperty =
            DependencyProperty.Register(nameof(CurrentPosBrush), typeof(Brush), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));



     



        public Rect ViewPort
        {
            get { return (Rect)GetValue(ViewPortProperty); }
            set { SetValue(ViewPortProperty, value); }
        }

        public static readonly DependencyProperty ViewPortProperty =
            DependencyProperty.Register(nameof(ViewPort), typeof(Rect), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new Rect(0,0,100,100), FrameworkPropertyMetadataOptions.AffectsRender));

        public Rect MovedViewPort
        {
            get { return (Rect)GetValue(MovedViewPortProperty); }
            set { SetValue(MovedViewPortProperty, value); }
        }

        public static readonly DependencyProperty MovedViewPortProperty =
            DependencyProperty.Register(nameof(MovedViewPort), typeof(Rect), typeof(WaferViewPortControl), new FrameworkPropertyMetadata(new Rect(0, 0, 100, 100), FrameworkPropertyMetadataOptions.None));





        private void DrawWaferVisual()
        {
            _waferVisual.Width = ActualWidth;
            _waferVisual.Height = ActualHeight;
            _waferVisual.WaferInfos = WaferDimentionalCharac;
            _waferVisual.WaferBrush = WaferBrush;
            _waferVisual.EdgeExclusionBrush = EdgeExclusionBrush;
            _waferVisual.WaferBorderPen = WaferBorderPen;
            _waferVisual.EdgeExclusionThickness = EdgeExclusionThickness;

            _waferVisual.Draw();
        }

        private void DrawViewPort()
        {
            if (WaferDimentionalCharac == null)
                return;

            using (var drawingContext = _currentPosVisual.RenderOpen())
            {
                var controlPosX = ViewPort.Left * ActualWidth ;
                var controlPosY = ViewPort.Top * ActualHeight;
                var minCurPosWidth = 5;
                var minCurPosHeight = 5;
                drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));

                // We calculate the field of view in pixels
                var fieldOfViewWidth = Math.Max(ViewPort.Width * ActualWidth , minCurPosWidth);
                var fieldOfViewHeight = Math.Max(ViewPort.Height * ActualHeight , minCurPosHeight);

                drawingContext.DrawRectangle(CurrentPosBrush, CurrentPosBorderPen, new Rect(controlPosX, controlPosY, fieldOfViewWidth, fieldOfViewHeight));
                drawingContext.Pop();
            }
        }

        private void DrawBackground()
        {
 
            using (var drawingContext = _currentPosVisual.RenderOpen())
            {
  
                
                
            }
        }


        protected override int VisualChildrenCount => _visualCollection.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var width = Math.Min(availableSize.Width, 1000);
            var height = Math.Min(availableSize.Height, 1000);

            width = Math.Min(width, height);
            height = width;

            return new Size(width, height);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //DrawBackground();
            drawingContext.DrawRectangle(new SolidColorBrush(Colors.Transparent), null, new Rect(0, 0, ActualWidth, ActualHeight));

            DrawWaferVisual();

            DrawViewPort();

            base.OnRender(drawingContext);
        }
    }
}
