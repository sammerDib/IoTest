using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.UI.Controls
{
    /// <summary>
    /// Interaction logic for TwoChoiceDisplay.xaml
    /// </summary>
    public partial class TwoChoiceDisplay : UserControl
    {
        public TwoChoiceDisplay()
        {
            InitializeComponent();
        }

        public string Label
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string FirstChoice
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string SecondChoice
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TwoChoiceDisplay), new FrameworkPropertyMetadata(string.Empty) { BindsTwoWayByDefault = true });

    }
}
