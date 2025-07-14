using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using UnitySC.PM.ANA.Client.Controls.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Controls
{
    /// <summary>
    /// Interaction logic for WaferMapControl.xaml
    /// </summary>
    public partial class WaferMapControl : UserControl
    {
         
        public WaferMapControl()
        {
            InitializeComponent();

            Loaded += WaferMapControl_Loaded;
            Unloaded += WaferMapControl_Unloaded;
            Width = WaferMapDisplayControl.DisplayResolution;
            Height = WaferMapDisplayControl.DisplayResolution;
        }

        private void WaferMapControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CanvasHover.MouseMove -= CanvasHover_MouseMove;
            CanvasHover.MouseLeave -= CanvasHover_MouseLeave;
            CanvasHover.MouseLeftButtonDown -= CanvasHover_MouseLeftButtonDown;
            Unloaded -= WaferMapControl_Unloaded;
        }

        private void WaferMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasHover.MouseMove += CanvasHover_MouseMove;
            CanvasHover.MouseLeave += CanvasHover_MouseLeave;
            CanvasHover.MouseLeftButtonDown += CanvasHover_MouseLeftButtonDown;
            LayoutUpdated += WaferMapControl_LayoutUpdated;
            Loaded -= WaferMapControl_Loaded;
        }

        private void WaferMapControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (_drawingRatio!=0)
            {
                AddDiesToSelectedDiesPositionPixels(SelectedDies.ToList());
                LayoutUpdated -= WaferMapControl_LayoutUpdated;
            }
        }

        private void CanvasHover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(CanvasHover);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                return;

            // Convert the position to milimeters
            var mousePosmm = new Point(ConvertPixelToMm(mousePos.X), ConvertPixelToMm(mousePos.Y));
            var dieId = GetDieIdFromPos(mousePosmm);

            if (!(dieId is null))
            {
                if (IsMultiSelection)
                {
                    var alreadySelectedDie = SelectedDies.FirstOrDefault(sd => (sd.Row == dieId.Row) && (sd.Column == dieId.Column));

                    if (alreadySelectedDie is null)
                        SelectedDies.Add(dieId);
                    else
                        SelectedDies.Remove(alreadySelectedDie);
                }
                else
                {
                    SelectedDie = dieId;
                }
            }
        }

        private void UpdateDieSelectedDisplay()
        {
            if (SelectedDie is null)
                return;
            Point dieSelectedPosition = GetDiePosFromId(SelectedDie);
            // We set the position of the DieSelectedDisplay
            Canvas.SetLeft(DieSelectedDisplay, ConvertMmToPixels(dieSelectedPosition.X));
            Canvas.SetTop(DieSelectedDisplay, ConvertMmToPixels(dieSelectedPosition.Y));
            DieSelectedDisplay.Width = ConvertMmToPixels(WaferMap.DieDimensions.DieWidth.Millimeters);
            DieSelectedDisplay.Height = ConvertMmToPixels(WaferMap.DieDimensions.DieHeight.Millimeters);
        }

        private void CanvasHover_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private double ConvertPixelToMm(double value)
        {
            if (WaferDimentionalCharac is null)
                return 0;

            return value * WaferDimentionalCharac.Diameter.Millimeters / CanvasHover.ActualWidth;
        }

        private double ConvertMmToPixels(double value)
        {
            if (WaferDimentionalCharac is null)
                return 0;
            return value * CanvasHover.ActualWidth / WaferDimentionalCharac.Diameter.Millimeters;
        }

        private void CanvasHover_MouseMove(object sender, MouseEventArgs e)
        {
            if ((CanvasHover.ActualWidth == 0) || (CanvasHover.ActualHeight == 0))
                return;

            if (WaferMap is null)
                return;

            var mousePos = e.GetPosition(CanvasHover);

            // Convert the position to milimeters
            var mousePosmm = new Point(ConvertPixelToMm(mousePos.X), ConvertPixelToMm(mousePos.Y));
            var dieId = GetDieIdFromPos(mousePosmm);

            if (dieId is null)
                return;

            //Console.WriteLine($"dieId : {dieId.Column} {dieId.Row}");

            var positionText = $"{ dieId.Column - DieReference.Column } : {DieReference.Row-dieId.Row }";
            (DieHoverDisplay.ToolTip as ToolTip).Content = positionText;
            DieHoverPosition.Text = positionText;

            Point dieHoverPosition = GetDiePosFromId(dieId);
            // We set the position of the DieHover
            Canvas.SetLeft(DieHoverDisplay, ConvertMmToPixels(dieHoverPosition.X));
            Canvas.SetTop(DieHoverDisplay, ConvertMmToPixels(dieHoverPosition.Y));
            DieHoverDisplay.Width = ConvertMmToPixels(WaferMap.DieDimensions.DieWidth.Millimeters);
            DieHoverDisplay.Height = ConvertMmToPixels(WaferMap.DieDimensions.DieHeight.Millimeters);
            DieHoverDisplay.Visibility = Visibility.Visible;
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(WaferMapControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public BitmapSource DiesBitmap { get; set; }

        public Length EdgeExclusionThickness
        {
            get { return (Length)GetValue(EdgeExclusionThicknessProperty); }
            set { SetValue(EdgeExclusionThicknessProperty, value); }
        }

        public static readonly DependencyProperty EdgeExclusionThicknessProperty =
            DependencyProperty.Register(nameof(EdgeExclusionThickness), typeof(Length), typeof(WaferMapControl), new FrameworkPropertyMetadata(0.Millimeters(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush EdgeExclusionBrush
        {
            get { return (Brush)GetValue(EdgeExclusionBrushProperty); }
            set { SetValue(EdgeExclusionBrushProperty, value); }
        }

        public static readonly DependencyProperty EdgeExclusionBrushProperty =
            DependencyProperty.Register(nameof(EdgeExclusionBrush), typeof(Brush), typeof(WaferMapControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush WaferBrush
        {
            get { return (Brush)GetValue(WaferBrushProperty); }
            set { SetValue(WaferBrushProperty, value); }
        }

        public static readonly DependencyProperty WaferBrushProperty =
            DependencyProperty.Register(nameof(WaferBrush), typeof(Brush), typeof(WaferMapControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen WaferBorderPen
        {
            get { return (Pen)GetValue(WaferBorderPenProperty); }
            set { SetValue(WaferBorderPenProperty, value); }
        }

        public static readonly DependencyProperty WaferBorderPenProperty =
            DependencyProperty.Register(nameof(WaferBorderPen), typeof(Pen), typeof(WaferMapControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush DiesBrush
        {
            get { return (Brush)GetValue(DiesBrushProperty); }
            set { SetValue(DiesBrushProperty, value); }
        }

        public static readonly DependencyProperty DiesBrushProperty =
            DependencyProperty.Register(nameof(DiesBrush), typeof(Brush), typeof(WaferMapControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightBlue), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register(nameof(TextBrush), typeof(Brush), typeof(WaferMapControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkGray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush DieHoverBrush
        {
            get { return (Brush)GetValue(DieHoverBrushProperty); }
            set { SetValue(DieHoverBrushProperty, value); }
        }

        public static readonly DependencyProperty DieHoverBrushProperty =
            DependencyProperty.Register(nameof(DieHoverBrush), typeof(Brush), typeof(WaferMapControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush DieSelectedBrush
        {
            get { return (Brush)GetValue(DieSelectedBrushProperty); }
            set { SetValue(DieSelectedBrushProperty, value); }
        }

        public static readonly DependencyProperty DieSelectedBrushProperty =
            DependencyProperty.Register(nameof(DieSelectedBrush), typeof(Brush), typeof(WaferMapControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Blue), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public bool DisplayPositions
        {
            get { return (bool)GetValue(DisplayPositionsProperty); }
            set { SetValue(DisplayPositionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayPositions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayPositionsProperty =
            DependencyProperty.Register(nameof(DisplayPositions), typeof(bool), typeof(WaferMapControl), new PropertyMetadata(false));

        public bool IsTextDisplayedOnDies => (!(WaferMap is null)) && (DisplayPositions) && (WaferMap.NbColumns <= 20) && (WaferMap.NbRows <= 40);

        public DieIndex DieReference
        {
            get { return (DieIndex)GetValue(DieRefrenceProperty); }
            set { SetValue(DieRefrenceProperty, value); }
        }

        public static readonly DependencyProperty DieRefrenceProperty =
            DependencyProperty.Register(nameof(DieReference), typeof(DieIndex), typeof(WaferMapControl), new PropertyMetadata(new DieIndex(0, 0), OnDieReferenceChanged));

        private static void OnDieReferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaferMapControl).HideDieHoverDisplay();
            
        }

        private void HideDieHoverDisplay()
        {
            DieHoverDisplay.Visibility = Visibility.Collapsed;
        }

        public DieIndex SelectedDie
        {
            get { return (DieIndex)GetValue(SelectedDieProperty); }
            set { SetValue(SelectedDieProperty, value); }
        }

        public static readonly DependencyProperty SelectedDieProperty =
            DependencyProperty.Register(nameof(SelectedDie), typeof(DieIndex), typeof(WaferMapControl), new PropertyMetadata(new DieIndex(0, 0), OnSelectedDieChanged));

        private static void OnSelectedDieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaferMapControl).UpdateDieSelectedDisplay();
        }

        public bool IsMultiSelection
        {
            get { return (bool)GetValue(IsMultiSelectionProperty); }
            set { SetValue(IsMultiSelectionProperty, value); }
        }

        public static readonly DependencyProperty IsMultiSelectionProperty =
            DependencyProperty.Register(nameof(IsMultiSelection), typeof(bool), typeof(WaferMapControl), new PropertyMetadata(false, OnIsMultiSelectionChanged));

        private static void OnIsMultiSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                (d as WaferMapControl).SelectedDies.CollectionChanged += (d as WaferMapControl).SelectedDies_CollectionChanged;
            else
                (d as WaferMapControl).SelectedDies.CollectionChanged -= (d as WaferMapControl).SelectedDies_CollectionChanged;
        }

        private void SelectedDies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action== System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                SelectedDiesPositionPixels.Clear();
            }
            if (!(e.NewItems is null))
            {
                var indexesList = e.NewItems.Cast<DieIndex>().ToList();
                AddDiesToSelectedDiesPositionPixels(indexesList);
            }
            if (!(e.OldItems is null))
            {
                foreach (DieIndex oldSelectedDie in e.OldItems)
                {
                    var oldSelectedDiePos = SelectedDiesPositionPixels.FirstOrDefault(dpp => (dpp.Position.Row == oldSelectedDie.Row) && (dpp.Position.Column == oldSelectedDie.Column));
                    if (!(oldSelectedDiePos is null))
                        SelectedDiesPositionPixels.Remove(oldSelectedDiePos);
                }
            }
        }

        public void AddDiesToSelectedDiesPositionPixels(List<DieIndex> dieIndex)
        {
            if ((dieIndex is null) || dieIndex.Count == 0 || _drawingRatio==0)
                return;
            var offsetLeft = WaferMap.DieGridTopLeft.X + WaferDimentionalCharac.Diameter.Millimeters / 2;
            var dieHorizontalPitch = WaferMap.DieDimensions.DieWidth.Millimeters + WaferMap.DieDimensions.StreetWidth.Millimeters;
            var offsetTop = WaferDimentionalCharac.Diameter.Millimeters / 2 - WaferMap.DieGridTopLeft.Y;
            var dieVerticalPitch = WaferMap.DieDimensions.DieHeight.Millimeters + WaferMap.DieDimensions.StreetHeight.Millimeters;
            double dieWidthPixels = WaferMap.DieDimensions.DieWidth.Millimeters * _drawingRatio;
            double dieHeightPixels = WaferMap.DieDimensions.DieHeight.Millimeters * _drawingRatio;

            foreach (DieIndex newSelectedDie in dieIndex)
            {
                var left = (offsetLeft + (newSelectedDie.Column * dieHorizontalPitch)) * _drawingRatio;
                var top = (offsetTop + (newSelectedDie.Row * dieVerticalPitch)) * _drawingRatio;

                SelectedDiesPositionPixels.Add(new DiePositionPixels { DieRect = new Rect(left, top, dieWidthPixels, dieHeightPixels), Position = newSelectedDie });
            }
        }

        public ObservableCollection<DieIndex> SelectedDies
        {
            get { return (ObservableCollection<DieIndex>)GetValue(SelectedDiesProperty); }
            set { SetValue(SelectedDiesProperty, value); }
        }

        public static readonly DependencyProperty SelectedDiesProperty =
            DependencyProperty.Register(nameof(SelectedDies), typeof(ObservableCollection<DieIndex>), typeof(WaferMapControl), new PropertyMetadata(new ObservableCollection<DieIndex>(), OnSelectedDiesChanged));

        private static void OnSelectedDiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.OldValue is null))
            {
                (d as WaferMapControl).SelectedDiesPositionPixels.Clear();
                (e.OldValue as ObservableCollection<DieIndex>).CollectionChanged -= (d as WaferMapControl).SelectedDies_CollectionChanged;
            }
            if (!(e.NewValue is null))
            {
                (d as WaferMapControl).SelectedDiesPositionPixels.Clear();
                (d as WaferMapControl).AddDiesToSelectedDiesPositionPixels((d as WaferMapControl).SelectedDies.ToList());
                (e.NewValue as ObservableCollection<DieIndex>).CollectionChanged += (d as WaferMapControl).SelectedDies_CollectionChanged;
            }
        }

        protected ObservableCollection<DiePositionPixels> SelectedDiesPositionPixels
        {
            get { return (ObservableCollection<DiePositionPixels>)GetValue(SelectedDiesPositionPixelsProperty); }
            set { SetValue(SelectedDiesPositionPixelsProperty, value); }
        }

        public static readonly DependencyProperty SelectedDiesPositionPixelsProperty =
            DependencyProperty.Register(nameof(SelectedDiesPositionPixels), typeof(ObservableCollection<DiePositionPixels>), typeof(WaferMapControl), new PropertyMetadata(new ObservableCollection<DiePositionPixels>()));

        public WaferMapResult WaferMap
        {
            get { return (WaferMapResult)GetValue(WaferMapProperty); }
            set { SetValue(WaferMapProperty, value); }
        }

        public static readonly DependencyProperty WaferMapProperty =
            DependencyProperty.Register(nameof(WaferMap), typeof(WaferMapResult), typeof(WaferMapControl), new PropertyMetadata(null, OnWaferMapChanged));

        private static void OnWaferMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaferMapControl).UpdateDies();
        }

        private List<DiePositionPixels> _dies;

        public List<DiePositionPixels> Dies
        {
            get
            {
                if (_dies is null)
                    _dies = new List<DiePositionPixels>();
                return _dies;
            }

            private set => _dies = value;
        }

        private double _drawingRatio => DiesPositionsDisplay.ActualWidth / (WaferDimentionalCharac?.Diameter.Millimeters ?? 300);

        public double DieWidthPixels => WaferMap.DieDimensions.DieWidth.Millimeters * _drawingRatio;
        public double DieHeightPixels => WaferMap.DieDimensions.DieHeight.Millimeters * _drawingRatio;

        private void UpdateDies()
        {
            if (!IsTextDisplayedOnDies)
                return;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Dies.Clear();

                var newDies = new List<DiePositionPixels>();
                var offsetLeft = WaferMap.DieGridTopLeft.X + WaferDimentionalCharac.Diameter.Millimeters / 2;
                var offsetTop = WaferDimentionalCharac.Diameter.Millimeters / 2 - WaferMap.DieGridTopLeft.Y;

                double dieHorizontalPitch = WaferMap.DieDimensions.DieWidth.Millimeters + WaferMap.DieDimensions.StreetWidth.Millimeters;
                double dieVerticalPitch = WaferMap.DieDimensions.DieHeight.Millimeters + WaferMap.DieDimensions.StreetHeight.Millimeters;

                var drawingRatio = _drawingRatio;
                double dieWidthPixels = WaferMap.DieDimensions.DieWidth.Millimeters * drawingRatio;
                double dieHeightPixels = WaferMap.DieDimensions.DieHeight.Millimeters * drawingRatio;

                double left;
                double top;

                for (int row = 0; row < WaferMap.NbRows; row++)

                {
                    top = (offsetTop + (row * dieVerticalPitch)) * drawingRatio;

                    for (int column = 0; column < WaferMap.NbColumns; column++)

                    {
                        if (WaferMap.DiesPresence.GetValue(row, column))
                        {
                            left = (offsetLeft + (column * dieHorizontalPitch)) * drawingRatio;

                            newDies.Add(new DiePositionPixels() { DieRect = new Rect(left, top, dieWidthPixels, dieHeightPixels), Position = new DieIndex(column, row) });
                        }
                    }
                }

                Dies = newDies;
                BindingOperations.GetBindingExpression(DiesPositionsDisplay, ItemsControl.ItemsSourceProperty).UpdateTarget();

                PositionFontSize = CalculateFontSize(new Rect(0, 0, dieWidthPixels, dieHeightPixels));
            }));
        }

        private double CalculateFontSize(Rect rect, int margin = 0)
        {
            double referenceFontSize = 50;
            var text = new FormattedText("-44 : -44", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), referenceFontSize, new SolidColorBrush(Colors.Black), 1.25);

            var horizontalFontSize = (rect.Width - 2 * margin) * referenceFontSize / text.Width;

            var verticalFontSize = (rect.Height - 2 * margin) * referenceFontSize / text.Height;

            return Math.Min(horizontalFontSize, verticalFontSize);
        }

        public double PositionFontSize { get; set; }

        private double _offsetLeft => WaferMap.DieGridTopLeft.X + WaferDimentionalCharac.Diameter.Millimeters / 2;
        private double _offsetTop => WaferDimentionalCharac.Diameter.Millimeters / 2 - WaferMap.DieGridTopLeft.Y;

        private double _dieHorizontalPitch => WaferMap.DieDimensions.DieWidth.Millimeters + WaferMap.DieDimensions.StreetWidth.Millimeters;
        private double _dieVerticalPitch => WaferMap.DieDimensions.DieHeight.Millimeters + WaferMap.DieDimensions.StreetHeight.Millimeters;

        // Position in milimeters on the wafer with the origin on the Top Left Corner
        private DieIndex GetDieIdFromPos(Point position)
        {
            // the Die 0,0 is at the bottom left corner

            var column = (int)((position.X - _offsetLeft) / _dieHorizontalPitch);
            var row = (int)((position.Y - _offsetTop) / _dieVerticalPitch);

            if ((row < 0) || (column < 0) || (row >= WaferMap.NbRows) || (column >= WaferMap.NbColumns) || (!WaferMap.DiesPresence.GetValue(row, column)))
                return null;

            return new DieIndex(column,  row);
        }

        // Get Die pos in mm
        private Point GetDiePosFromId(DieIndex dieId)
        {
            var posX = dieId.Column * _dieHorizontalPitch + _offsetLeft;
            var posY = dieId.Row * _dieVerticalPitch + _offsetTop;
            return new Point(posX, posY);
        }
    }

    public class PositionToStringConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((values[0] is null) || (values[1] is null) || (values[2] is null))
                return DependencyProperty.UnsetValue;
            if ((values[0] == DependencyProperty.UnsetValue) || (values[1] == DependencyProperty.UnsetValue) || (values[2] == DependencyProperty.UnsetValue))
                return DependencyProperty.UnsetValue;

            int nbRows= (int)values[2];
 
            if ((values[0] is DieIndex diePosition) && (values[1] is DieIndex dieReference))
            {
                return $"{diePosition.Column - dieReference.Column} : {dieReference.Row - diePosition.Row}";
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class PositionRowToStringConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((values[0] is null) || (values[1] is null) )
                return DependencyProperty.UnsetValue;

            if ((values[0] == DependencyProperty.UnsetValue) || (values[1] == DependencyProperty.UnsetValue))
                return DependencyProperty.UnsetValue;


            if ((values[0] is DieIndex diePosition) && (values[1] is DieIndex dieReference))
            {
                return $"{dieReference.Row - diePosition.Row  }";
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class PositionColumnToStringConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((values[0] is null) || (values[1] is null))
                return DependencyProperty.UnsetValue;

            if ((values[0] is DieIndex diePosition) && (values[1] is DieIndex dieReference))
            {
                return $"{diePosition.Column - dieReference.Column}";
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
