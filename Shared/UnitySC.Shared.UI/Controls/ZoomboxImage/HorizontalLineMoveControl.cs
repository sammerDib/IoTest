using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// Un ContentControl qui permet à l'utilisateur de déplacer horizontalement son contenu
    /// </summary>
    public class HorizontalLineMoveControl : ContentControl
    {
        public const double Cst_SideThumb_px = 4.0;

        public HorizontalLineMoveControl()
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(HorizontalLineMoveControl));
            descriptor.AddValueChanged(this, OnCanvasTopChanged);

            descriptor = DependencyPropertyDescriptor.FromProperty(HeightProperty, typeof(HorizontalLineMoveControl));
            descriptor.AddValueChanged(this, OnHeightChanged);

        }

        private void OnHeightChanged(object sender, EventArgs e)
        {
            UpdateCanvasTopPosition();
        }

        private void UpdateCanvasTopPosition()
        {
            Canvas.SetTop(this, Position - ActualHeight / 2);
        }

        private void OnCanvasTopChanged(object sender, EventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                Position = Canvas.GetTop(frameworkElement) + frameworkElement.ActualHeight / 2;
            }
        }


        /// <summary>
        /// Epaisseur des cotés
        /// </summary>
        public double SideThumbThickness
        {
            get { return (double)GetValue(SideThumbThicknessProperty); }
            set { SetValue(SideThumbThicknessProperty, value); }
        }

        public static readonly DependencyProperty SideThumbThicknessProperty =
            DependencyProperty.Register("SideThumbThickness", typeof(double), typeof(HorizontalLineMoveControl), new PropertyMetadata(1.0));



        /// <summary>
        /// Couleur de dessin de la sélection
        /// </summary>
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(Brush), typeof(HorizontalLineMoveControl), new PropertyMetadata(Brushes.LightGreen));




        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(HorizontalLineMoveControl), new PropertyMetadata(20d));


        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(double), typeof(HorizontalLineMoveControl), new PropertyMetadata(20d, OnPositionChanged));

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double value)
            {
                if (double.IsNaN(value))
                    return;
                if (d is HorizontalLineMoveControl horizontalLineMoveControl)
                {
                    horizontalLineMoveControl.UpdateCanvasTopPosition();
                }
            }
        }



    }
}
