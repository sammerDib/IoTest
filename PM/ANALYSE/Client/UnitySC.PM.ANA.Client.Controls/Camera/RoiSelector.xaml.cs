using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using UnitySC.Shared.UI.Extensions;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    /// <summary>
    /// Interaction logic for RoiSelector.xaml
    /// </summary>
    public partial class RoiSelector : UserControl
    {
        public RoiSelector()
        {
            InitializeComponent();
        }
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

        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RoiSelector), new FrameworkPropertyMetadata(false));

        public double ContainerWidth
        {
            get { return (double)GetValue(MaxSelectorWidthProperty); }
            set { SetValue(MaxSelectorWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxSelectorWidthProperty =
            DependencyProperty.Register(nameof(ContainerWidth), typeof(double), typeof(RoiSelector), new PropertyMetadata(0d, OnContainerWidthChanged));

        private static void OnContainerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RoiSelector)?.UpdateSelectionRectDisplay();
        }

        public double ContainerHeight
        {
            get { return (double)GetValue(MaxSelectorHeightProperty); }
            set { SetValue(MaxSelectorHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxSelectorHeightProperty =
            DependencyProperty.Register(nameof(ContainerHeight), typeof(double), typeof(RoiSelector), new PropertyMetadata(0d, OnContainerHeightChanged));

        private static void OnContainerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RoiSelector)?.UpdateSelectionRectDisplay();
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(RoiSelector), new PropertyMetadata(0d, OnImageSizeChanged));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(RoiSelector), new PropertyMetadata(0d, OnImageSizeChanged));

        private static void OnImageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RoiSelector)?.UpdateSelectionRectDisplay();
        }

        public double MinSelectorWidth
        {
            get { return (double)GetValue(MinSelectorWidthProperty); }
            set { SetValue(MinSelectorWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinSelectorWidthProperty =
            DependencyProperty.Register(nameof(MinSelectorWidth), typeof(double), typeof(RoiSelector), new PropertyMetadata(0d, OnLimitsChanged));

        public double MinSelectorHeight
        {
            get { return (double)GetValue(MinSelectorHeightProperty); }
            set { SetValue(MinSelectorHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSelectorWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinSelectorHeightProperty =
            DependencyProperty.Register(nameof(MinSelectorHeight), typeof(double), typeof(RoiSelector), new PropertyMetadata(0d, OnLimitsChanged));

        private static void OnLimitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RoiSelector).AdjustSelectorSize(1, 1);
        }
       
        public Rect SelectedRect
        {
            get { return (Rect)GetValue(SelectedRectProperty); }
            set { SetValue(SelectedRectProperty, value); }
        }
        // Using a DependencyProperty as the backing store for SelectedRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRectProperty =
            DependencyProperty.Register(nameof(SelectedRect), typeof(Rect), typeof(RoiSelector), new PropertyMetadata(new Rect(), OnSelectedRectChanged, CoerceSelectedRect));

        private static void OnSelectedRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newRect = (Rect)e.NewValue;
            var oldRect = (Rect)e.OldValue;
            if (oldRect != newRect)
                (d as RoiSelector)?.UpdateSelectionRectDisplay();
        }       
        private static object CoerceSelectedRect(DependencyObject d, object baseValue)
        {
            var rect = (d as RoiSelector)?.GetValidSelectedRect((Rect)baseValue);
            // Méthode pour forcer la valeur de la propriété de dépendance
            var sender = (FrameworkElement)d;
            sender.SetTargetValue(SelectedRectProperty, rect);
            return rect;        
        }
        private Rect GetValidSelectedRect(Rect selectedRect)
        {          
            var mainRect = new Rect(0, 0, ImageWidth, ImageHeight);

            var newX = selectedRect.X;
            var newY = selectedRect.Y;
            //Adjust Width
            if (selectedRect.X < mainRect.X)
            {
                newX = mainRect.X;
                
            }
            if (selectedRect.X > mainRect.X + ImageWidth)
            {
                newX = mainRect.Width - MinSelectorWidth;
                selectedRect.Width= MinSelectorWidth;

            }
            if (ImageWidth!= 0 && selectedRect.X + selectedRect.Width > ImageWidth)
            {
                selectedRect.Width = Math.Abs(ImageWidth - selectedRect.X);

            }
            //Adjust Height
            if (selectedRect.Y < mainRect.Y)
            {
                newY = mainRect.Y;
            }
            if (selectedRect.Y > mainRect.Y + ImageHeight)
            {
                newY = mainRect.Height - MinSelectorHeight;
                selectedRect.Height = MinSelectorHeight;
            }
            if (ImageHeight != 0 && selectedRect.Y + selectedRect.Height > ImageHeight)
            {
                selectedRect.Height = Math.Abs(ImageHeight - selectedRect.Y);
            }

            var newWidth = selectedRect.Width;
            if (newWidth < MinSelectorWidth)
                newWidth = MinSelectorWidth;
            if ((ImageWidth > 0) && (newWidth > ImageWidth))
                newWidth = ImageWidth;

            var newHeight = selectedRect.Height;
            if (newHeight < MinSelectorHeight)
                newHeight = MinSelectorHeight;
            if ((ImageHeight > 0) && (newHeight > ImageHeight))
                newHeight = ImageHeight;
                
            return new Rect(newX, newY, newWidth , newHeight);
        }                                  
        // this function adjusts the size of the selector when the limits change
        private void AdjustSelectorSize(double ratioX, double ratioY)
        {
            Width = SelectedRect.Width * ratioX;

            Height = SelectedRect.Height * ratioY;
        }

        private void UpdateSelectionRectDisplay()
        {
            if (ImageWidth == 0 || ImageHeight == 0)
                return;           

            SetValue(Canvas.LeftProperty, SelectedRect.X * ContainerWidth / ImageWidth);
            SetValue(Canvas.TopProperty, SelectedRect.Y * ContainerHeight / ImageHeight);

            this.Width = SelectedRect.Width * ContainerWidth / ImageWidth;
            this.Height = SelectedRect.Height * ContainerHeight / ImageHeight;

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
            
            if (name.Contains("Top"))
            {
                newHeight = newHeight - e.VerticalChange;
                if (newHeight > maxHeight)
                    newHeight = maxHeight;
                if (newHeight < minHeight)
                    newHeight = minHeight;
                newTop = (newTop + e.VerticalChange);                        
            }
            if (name.Contains("Bottom"))
            {
                newHeight = newHeight + e.VerticalChange;
                if (newHeight > maxHeight)
                    newHeight = maxHeight;
                if (newHeight < minHeight)
                    newHeight = minHeight;                               
            }
            if (name.Contains("Left"))
            {
                newWidth = newWidth - e.HorizontalChange;
                if (newWidth > maxWidth)
                    newWidth = maxWidth;
                if (newWidth < minWidth)
                    newWidth = minWidth;
                newLeft = (newLeft + e.HorizontalChange);                         
            }
            if (name.Contains("Right"))
            {
                newWidth = newWidth + e.HorizontalChange;
                if (newWidth > maxWidth)
                    newWidth = maxWidth;
                if (newWidth < minWidth)
                    newWidth = minWidth;                
            }                       
            SelectedRect = new Rect(newLeft * ImageWidth / ContainerWidth, 
                                    newTop * ImageHeight / ContainerHeight ,
                                    newWidth * ImageWidth / ContainerWidth,
                                    newHeight * ImageHeight / ContainerHeight);
        }       
    }
}
