using System.Windows;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.UserControls
{
    /// <summary>
    /// Interaction logic for Arrow.xaml
    /// </summary>
    public partial class Arrow
    {
        public Arrow()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle), typeof(double), typeof(Arrow), new PropertyMetadata(default(double)));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
    }
}
