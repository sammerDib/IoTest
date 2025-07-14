using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    /// <summary>
    /// Interaction logic for HorizontalLineSelector.xaml
    /// </summary>
    public partial class HorizontalLineSelector : UserControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                if (value)
                    Canvas.SetZIndex(this, 100);
                else
                    Canvas.SetZIndex(this, 0);
            }
        }

        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(HorizontalLineSelector), new FrameworkPropertyMetadata(false));

        public double ContainerWidth
        {
            get { return (double)GetValue(MaxSelectorWidthProperty); }
            set { SetValue(MaxSelectorWidthProperty, value); }
        }

        public static readonly DependencyProperty MaxSelectorWidthProperty =
            DependencyProperty.Register(nameof(ContainerWidth), typeof(double), typeof(HorizontalLineSelector), new PropertyMetadata(0d, OnContainerWidthChanged));

        private static void OnContainerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HorizontalLineSelector)?.UpdateSelectionDisplay();
        }

        public double ContainerHeight
        {
            get { return (double)GetValue(MaxSelectorHeightProperty); }
            set { SetValue(MaxSelectorHeightProperty, value); }
        }

        public static readonly DependencyProperty MaxSelectorHeightProperty =
            DependencyProperty.Register(nameof(ContainerHeight), typeof(double), typeof(HorizontalLineSelector), new PropertyMetadata(0d, OnContainerHeightChanged));

        private static void OnContainerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HorizontalLineSelector)?.UpdateSelectionDisplay();
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(HorizontalLineSelector), new PropertyMetadata(0d, OnImageSizeChanged));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(HorizontalLineSelector), new PropertyMetadata(0d, OnImageSizeChanged));

        private static void OnImageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d is HorizontalLineSelector horizontalLineSelector) && (!(horizontalLineSelector is null)))
            {
                horizontalLineSelector.SelectedVerticalPos = horizontalLineSelector.ImageHeight / 2 + 100;
                horizontalLineSelector.UpdateSelectionDisplay();
            }
        }

        // In pixels on the full image
        public double SelectedVerticalPos
        {
            get { return (double)GetValue(SelectedVerticalPosProperty); }
            set { SetValue(SelectedVerticalPosProperty, value); }
        }

        public static readonly DependencyProperty SelectedVerticalPosProperty =
            DependencyProperty.Register(nameof(SelectedVerticalPos), typeof(double), typeof(HorizontalLineSelector), new PropertyMetadata(0d, OnSelectedVerticalPosChanged, CoerceSelectedVerticalPos));

        private static object CoerceSelectedVerticalPos(DependencyObject d, object baseValue)
        {
            return (d as HorizontalLineSelector)?.GetValidSelectedVerticalPos((double)baseValue);
        }

        // The units are pixels

        private static void OnSelectedVerticalPosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newPos = (double)e.NewValue;
            var oldPos = (double)e.OldValue;
            if (oldPos != newPos)
                (d as HorizontalLineSelector)?.UpdateSelectionDisplay();
        }

        private double GetValidSelectedVerticalPos(double selectedVerticalPos)
        {
            if (ContainerHeight == 0)
                return selectedVerticalPos;

            if (selectedVerticalPos < 0)
                selectedVerticalPos = 0;
            if (selectedVerticalPos > ImageHeight)
                selectedVerticalPos = ImageHeight;
            return selectedVerticalPos;
        }

        private void UpdateSelectionDisplay()
        {
            if (ImageWidth == 0 || ImageHeight == 0)
                return;
            SetValue(Canvas.TopProperty, (SelectedVerticalPos * ContainerHeight / ImageHeight) - ActualHeight / 2);
            this.Width = ContainerWidth;
        }

        public HorizontalLineSelector()
        {
            this.InitializeComponent();
        }

        private void DesignerComponent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsSelected = true;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if ((ContainerWidth == 0) || (ContainerHeight == 0))
                return;

            string name = ((Thumb)sender).Name;
            double newTop = (double)GetValue(Canvas.TopProperty);

            if (name.Contains("Left") || name.Contains("Right"))
            {
                newTop = newTop + e.VerticalChange;
                SelectedVerticalPos = (newTop + ActualHeight / 2) * ImageHeight / ContainerHeight;
            }
        }

        private void SelectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
            SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);
        }
    }
}
