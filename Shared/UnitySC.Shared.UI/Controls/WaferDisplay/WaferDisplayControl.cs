using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.UI.Controls
{
    public class WaferDisplayControl : FrameworkElement
    {
        private WaferVisual _waferVisual;
        private DrawingVisual _currentPosVisual;

        private readonly VisualCollection _visualCollection;

        static WaferDisplayControl()
        {
        }

        public WaferDisplayControl()
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
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferDimention.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(WaferDisplayControl), new FrameworkPropertyMetadata(new WaferDimensionalCharacteristic() { Diameter = 300.Millimeters() }, FrameworkPropertyMetadataOptions.AffectsRender));

        public Length EdgeExclusionThickness
        {
            get { return (Length)GetValue(EdgeExclusionThicknessProperty); }
            set { SetValue(EdgeExclusionThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionThicknessProperty =
            DependencyProperty.Register(nameof(EdgeExclusionThickness), typeof(Length), typeof(WaferDisplayControl), new FrameworkPropertyMetadata(5.Millimeters(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush EdgeExclusionBrush
        {
            get { return (Brush)GetValue(EdgeExclusionBrushProperty); }
            set { SetValue(EdgeExclusionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionBrushProperty =
            DependencyProperty.Register(nameof(EdgeExclusionBrush), typeof(Brush), typeof(WaferDisplayControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush WaferBrush
        {
            get { return (Brush)GetValue(WaferBrushProperty); }
            set { SetValue(WaferBrushProperty, value); }
        }

        public static readonly DependencyProperty WaferBrushProperty =
            DependencyProperty.Register(nameof(WaferBrush), typeof(Brush), typeof(WaferDisplayControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen WaferBorderPen
        {
            get { return (Pen)GetValue(WaferBorderPenProperty); }
            set { SetValue(WaferBorderPenProperty, value); }
        }

        public static readonly DependencyProperty WaferBorderPenProperty =
            DependencyProperty.Register(nameof(WaferBorderPen), typeof(Pen), typeof(WaferDisplayControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

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

        protected override int VisualChildrenCount => _visualCollection.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var width = availableSize.Width;
            var height = availableSize.Height;
            width = Math.Min(width, height);
            height = width;

            return new Size(width, height);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawWaferVisual();

            base.OnRender(drawingContext);
        }
    }
}
