using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.UI.Controls
{
    /// <summary>
    /// Interaction logic for TextBoxWithPen.xaml
    /// </summary>
    public partial class TextBoxWithPen : UserControl
    {
        public TextBoxWithPen()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBoxWithPen), new FrameworkPropertyMetadata(string.Empty) { BindsTwoWayByDefault = true });

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(TextBoxWithPen), new PropertyMetadata(false));

        // Used to place the pen next to the text or far away on the right
        public HorizontalAlignment PenAlignment
        {
            get { return (HorizontalAlignment)GetValue(PenAlignmentProperty); }
            set { SetValue(PenAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PenAlignmentProperty =
            DependencyProperty.Register("PenAlignment", typeof(HorizontalAlignment), typeof(TextBoxWithPen), new PropertyMetadata(HorizontalAlignment.Left));

        private void TexBoxEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
        }
    }
}
