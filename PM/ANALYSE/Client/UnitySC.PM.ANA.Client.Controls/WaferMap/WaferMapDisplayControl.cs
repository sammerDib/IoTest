using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using UnitySC.PM.ANA.Client.Controls.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.Controls
{
    public class WaferMapDisplayControl : FrameworkElement
    {
        public static int DisplayResolution = 15000;

        private WaferVisual _waferVisual;

        private DiesVisual _diesVisual;
        private PointsVisual _pointsVisual;

        private readonly VisualCollection _childWafer;

        static WaferMapDisplayControl()
        {
        }

        public WaferMapDisplayControl()
        {
            Width = DisplayResolution;
            Height = DisplayResolution;
            _childWafer = new VisualCollection(this);
            _waferVisual = new WaferVisual();
            _childWafer.Add(_waferVisual);
          
            _diesVisual = new DiesVisual(this);
            _childWafer.Add(_diesVisual);
            _pointsVisual = new PointsVisual(this);
            _childWafer.Add(_pointsVisual);
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            if (designTime)
            {
                return;
            }

            this.CacheMode = new BitmapCache() { EnableClearType = false, RenderAtScale = 1, SnapsToDevicePixels = false };
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferDimention.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new WaferDimensionalCharacteristic() { Diameter = 300.Millimeters() }, FrameworkPropertyMetadataOptions.AffectsRender));

        public Length EdgeExclusionThickness
        {
            get { return (Length)GetValue(EdgeExclusionThicknessProperty); }
            set { SetValue(EdgeExclusionThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionThicknessProperty =
            DependencyProperty.Register(nameof(EdgeExclusionThickness), typeof(Length), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(0.Millimeters(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush EdgeExclusionBrush
        {
            get { return (Brush)GetValue(EdgeExclusionBrushProperty); }
            set { SetValue(EdgeExclusionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionBrushProperty =
            DependencyProperty.Register(nameof(EdgeExclusionBrush), typeof(Brush), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush WaferBrush
        {
            get { return (Brush)GetValue(WaferBrushProperty); }
            set { SetValue(WaferBrushProperty, value); }
        }

        public static readonly DependencyProperty WaferBrushProperty =
            DependencyProperty.Register(nameof(WaferBrush), typeof(Brush), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush DiesBrush
        {
            get { return (Brush)GetValue(DiesBrushProperty); }
            set { SetValue(DiesBrushProperty, value); }
        }

        public static readonly DependencyProperty DiesBrushProperty =
            DependencyProperty.Register(nameof(DiesBrush), typeof(Brush), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightBlue), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register(nameof(TextBrush), typeof(Brush), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkGray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen WaferBorderPen
        {
            get { return (Pen)GetValue(WaferBorderPenProperty); }
            set { SetValue(WaferBorderPenProperty, value); }
        }

        public static readonly DependencyProperty WaferBorderPenProperty =
            DependencyProperty.Register(nameof(WaferBorderPen), typeof(Pen), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        public DieIndex DieReference
        {
            get { return (DieIndex)GetValue(DieRefrenceProperty); }
            set { SetValue(DieRefrenceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefrenceDie.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DieRefrenceProperty =
            DependencyProperty.Register(nameof(DieReference), typeof(DieIndex), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new DieIndex(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

        public WaferMapResult WaferMap
        {
            get { return (WaferMapResult)GetValue(WaferMapProperty); }
            set { SetValue(WaferMapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferMap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferMapProperty =
            DependencyProperty.Register(nameof(WaferMap), typeof(WaferMapResult), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));



        public List<Point> Points
        {
            get { return (List<Point>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(List<Point>), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        [Browsable(true)]
        public Brush PointsBrush
        {
            get { return (Brush)GetValue(PointsBrushProperty); }
            set { SetValue(PointsBrushProperty, value); }
        }

        public static readonly DependencyProperty PointsBrushProperty =
            DependencyProperty.Register(nameof(PointsBrush), typeof(Brush), typeof(WaferMapDisplayControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));


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

        protected override int VisualChildrenCount => _childWafer.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < _childWafer.Count)
                return _childWafer[index];

            throw new ArgumentOutOfRangeException();
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
            DrawWaferVisual();
            if (!(_diesVisual is null))
            {
                _diesVisual.DrawDies(DiesBrush, TextBrush);
            }

            if (!(_pointsVisual is null))
            {
                _pointsVisual.DrawPoints(Points, PointsBrush, WaferDimentionalCharac);
            }
        }
  
    }
}
