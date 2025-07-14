using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    /// <summary>
    /// Interaction logic for CenteredRoiSelector.xaml
    /// </summary>
    public partial class CenteredRoiSelector : UserControl
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

        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CenteredRoiSelector), new FrameworkPropertyMetadata(false));

        public double ContainerWidth
        {
            get { return (double)GetValue(MaxSelectorWidthProperty); }
            set { SetValue(MaxSelectorWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxSelectorWidthProperty =
            DependencyProperty.Register(nameof(ContainerWidth), typeof(double), typeof(CenteredRoiSelector), new PropertyMetadata(0d, OnContainerWidthChanged));

        private static void OnContainerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CenteredRoiSelector)?.UpdateSelectionDisplay();
        }

        public double ContainerHeight
        {
            get { return (double)GetValue(MaxSelectorHeightProperty); }
            set { SetValue(MaxSelectorHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxSelectorHeightProperty =
            DependencyProperty.Register(nameof(ContainerHeight), typeof(double), typeof(CenteredRoiSelector), new PropertyMetadata(0d, OnContainerHeightChanged));

        private static void OnContainerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CenteredRoiSelector)?.UpdateSelectionDisplay();
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(CenteredRoiSelector), new PropertyMetadata(0d, OnImageSizeChanged));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(CenteredRoiSelector), new PropertyMetadata(0d, OnImageSizeChanged));

        private static void OnImageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CenteredRoiSelector)?.UpdateSelectionDisplay();
        }

        public double MinSelectorWidth
        {
            get { return (double)GetValue(MinSelectorWidthProperty); }
            set { SetValue(MinSelectorWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinSelectorWidthProperty =
            DependencyProperty.Register(nameof(MinSelectorWidth), typeof(double), typeof(CenteredRoiSelector), new PropertyMetadata(0d, OnLimitsChanged));

        public double MinSelectorHeight
        {
            get { return (double)GetValue(MinSelectorHeightProperty); }
            set { SetValue(MinSelectorHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinSelectorHeightProperty =
            DependencyProperty.Register(nameof(MinSelectorHeight), typeof(double), typeof(CenteredRoiSelector), new PropertyMetadata(0d, OnLimitsChanged));

        private static void OnLimitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CenteredRoiSelector).AdjustSelectorSize(1, 1);
        }

        // In pixels on the full image
        public Size SelectedSize
        {
            get { return (Size)GetValue(SelectedSizeProperty); }
            set { SetValue(SelectedSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedSizeProperty =
            DependencyProperty.Register(nameof(SelectedSize), typeof(Size), typeof(CenteredRoiSelector), new PropertyMetadata(new Size(), OnSelectedSizeChanged, CoerceSelectedSize));

        private static object CoerceSelectedSize(DependencyObject d, object baseValue)
        {
            return (d as CenteredRoiSelector)?.GetValidSelectedSize((Size)baseValue);
        }

        // this function adjusts the size of the selector when the limits change
        private void AdjustSelectorSize(double ratioX, double ratioY)
        {
            Width = SelectedSize.Width * ratioX;

            Height = SelectedSize.Height * ratioY;
        }

        // The units are pixels

        private static void OnSelectedSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newSize = (Size)e.NewValue;
            var oldRect = (Size)e.OldValue;
            if (oldRect != newSize)
                (d as CenteredRoiSelector)?.UpdateSelectionDisplay();
        }

        private Size GetValidSelectedSize(Size selectedSize)
        {
            var newWidth = selectedSize.Width;
            if (newWidth < MinSelectorWidth)
                newWidth = MinSelectorWidth;
            if ((ImageWidth > 0) && (newWidth > ImageWidth))
                newWidth = ImageWidth;

            var newHeight = selectedSize.Height;
            if (newHeight < MinSelectorHeight)
                newHeight = MinSelectorHeight;
            if ((ImageHeight > 0) && (newHeight > ImageHeight))
                newHeight = ImageHeight;

            return new Size(newWidth, newHeight);
        }

        private void UpdateSelectionDisplay()
        {
            if (ImageWidth == 0 || ImageHeight == 0)
                return;
            var displaySize = new Size(SelectedSize.Width * ContainerWidth / ImageWidth, SelectedSize.Height * ContainerHeight / ImageHeight);

            SetValue(Canvas.LeftProperty, (ContainerWidth - displaySize.Width) / 2);
            SetValue(Canvas.TopProperty, (ContainerHeight - displaySize.Height) / 2);
            this.Width = displaySize.Width;
            this.Height = displaySize.Height;
        }

        public bool CanVResize { get; private set; }
        public bool CanHResize { get; private set; }

        public CenteredRoiSelector()
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

            var minWidth = this.MinSelectorWidth < 10.0 ? 10.0 : this.MinSelectorWidth;
            var minHeight = this.MinSelectorHeight < 10.0 ? 10.0 : this.MinSelectorHeight;
            var maxWidth = this.ContainerWidth;
            var maxHeight = this.ContainerHeight;

            string name = ((Thumb)sender).Name;
            double newHeight = this.Height;
            double newWidth = this.Width;
            double newLeft = (double)GetValue(Canvas.LeftProperty);
            double newTop = (double)GetValue(Canvas.TopProperty);

            if (name.Contains("Top") || name.Contains("Bottom"))
            {
                if (name.Contains("Top"))
                    newHeight = newHeight - 2 * e.VerticalChange;
                else // Bottom
                    newHeight = newHeight + 2 * e.VerticalChange;
                if (newHeight > maxHeight)
                    newHeight = maxHeight;
                if (newHeight < minHeight)
                    newHeight = minHeight;
                newTop = (maxHeight - newHeight) / 2;
                this.Height = newHeight;
                SetValue(Canvas.TopProperty, newTop);
            }

            if (name.Contains("Left") || name.Contains("Right"))
            {
                if (name.Contains("Left"))
                    newWidth = newWidth - 2 * e.HorizontalChange;
                else  // Right
                    newWidth = newWidth + 2 * e.HorizontalChange;

                if (newWidth > maxWidth)
                    newWidth = maxWidth;
                if (newWidth < minWidth)
                    newWidth = minWidth;
                newLeft = (maxWidth - newWidth) / 2;
                this.Width = newWidth;
                SetValue(Canvas.LeftProperty, newLeft);
            }
            SelectedSize = new Size(newWidth * ImageWidth / ContainerWidth, newHeight * ImageHeight / ContainerHeight);
        }

        private void SelectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
            SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);
        }
    }
}
