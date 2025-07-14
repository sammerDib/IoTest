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
    public class VerticalLineMoveControl : ContentControl
    {
        public const double Cst_SideThumb_px = 4.0;

        public VerticalLineMoveControl()
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(VerticalLineMoveControl));
            descriptor.AddValueChanged(this, OnCanvasLeftChanged);

            descriptor = DependencyPropertyDescriptor.FromProperty(WidthProperty, typeof(VerticalLineMoveControl));
            descriptor.AddValueChanged(this, OnWidthChanged);

        }

        private void OnWidthChanged(object sender, EventArgs e)
        {
            UpdateCanvasLeftPosition();
        }

        private void UpdateCanvasLeftPosition()
        {
            Canvas.SetLeft(this, Position - ActualWidth / 2); 
        }

        private void OnCanvasLeftChanged(object sender, EventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                Position = Canvas.GetLeft(frameworkElement) + frameworkElement.ActualWidth / 2;
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
            DependencyProperty.Register("SideThumbThickness", typeof(double), typeof(VerticalLineMoveControl), new PropertyMetadata(1.0));



        /// <summary>
        /// Couleur de dessin de la sélection
        /// </summary>
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(Brush), typeof(VerticalLineMoveControl), new PropertyMetadata(Brushes.LightGreen));




        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(VerticalLineMoveControl), new PropertyMetadata(20d));



        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(double), typeof(VerticalLineMoveControl), new PropertyMetadata(20d,OnPositionChanged));

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is  double value) 
            {
                if (double.IsNaN(value))
                    return;
                if (d is VerticalLineMoveControl verticalLineMoveControl)
                {
                    verticalLineMoveControl.UpdateCanvasLeftPosition();
                }
            }
        }



    }
}
