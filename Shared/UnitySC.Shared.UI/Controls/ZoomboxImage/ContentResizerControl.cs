using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// Un ContentControl qui permet à l'utilisateur de déplacer/redimensioner son contenu
    /// </summary>
    public class ContentResizerControl : ContentControl
    {
        public const double Cst_SideThumb_px = 4.0;
        public const double Cst_CornerThumb_px = 12.0;

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ContentResizerControl), new PropertyMetadata(1.0, new PropertyChangedCallback(ScalePropertyChanged)));

        private static void ScalePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var myThis = dependencyObject as ContentResizerControl;
            myThis.SideThumbThickness = Cst_SideThumb_px / myThis.Scale;
            myThis.CornerThumbThickness = Cst_CornerThumb_px / myThis.Scale;
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
            DependencyProperty.Register("SideThumbThickness", typeof(double), typeof(ContentResizerControl), new PropertyMetadata(1.0));

        /// <summary>
        /// Epaisseur des poignées dans les coins
        /// </summary>
        public double CornerThumbThickness
        {
            get { return (double)GetValue(CornerThumbThicknessProperty); }
            set { SetValue(CornerThumbThicknessProperty, value); }
        }

        public static readonly DependencyProperty CornerThumbThicknessProperty =
            DependencyProperty.Register("CornerThumbThickness", typeof(double), typeof(ContentResizerControl), new PropertyMetadata(1.0));

        /// <summary>
        /// Couleur de dessin de la sélection
        /// </summary>
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(Brush), typeof(ContentResizerControl), new PropertyMetadata(Brushes.LightGreen));

        /// <summary>
        /// Hilite Aim Cross
        /// </summary>
        public double HiliteCrossThumbSize
        {
            get { return (double)GetValue(HiliteCrossThumbSizeProperty); }
            set { SetValue(HiliteCrossThumbSizeProperty, value); }
        }

        public static readonly DependencyProperty HiliteCrossThumbSizeProperty =
            DependencyProperty.Register("HiliteCrossThumbSizeProperty", typeof(double), typeof(ContentResizerControl), new PropertyMetadata(128.0));
    }
}
