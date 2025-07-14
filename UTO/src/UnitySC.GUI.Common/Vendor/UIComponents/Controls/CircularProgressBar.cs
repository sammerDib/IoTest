using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// Circular progress bar is use to show that something is loading.
    /// </summary>
    public class CircularProgressBar : ProgressBar
    {
        static CircularProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularProgressBar),
                new FrameworkPropertyMetadata(typeof(CircularProgressBar)));
        }

        /// <summary>
        /// Identifies the <see cref="StrokeThickness" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness), typeof(double), typeof(CircularProgressBar), new PropertyMetadata(default(double)));

        /// <summary>
        /// Gets/Sets the thickness of the <see cref="CircularProgressBar"/>
        /// </summary>
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }
    }
}
