using System.Windows.Controls;
using System.Windows.Input;

using Agileo.LineCharts.Abstractions.Model;

using UnitySC.GUI.Common.Vendor.UIComponents.Converters;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Popups
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SetupGraphPopupView : UserControl
    {
        public SetupGraphPopupView()
        {
            InitializeComponent();
        }

        private void AxisMinimum_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            UpdateViewModelProperty(sender, nameof(IAxis.Minimum));
        }

        private void AxisMaximum_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            UpdateViewModelProperty(sender, nameof(IAxis.Maximum));
        }

        private void UpdateViewModelProperty(object sender, string propertyName)
        {
            var textBox = (TextBox)sender;
            var inputText = textBox.Text;
            var textBoxDataContext = (IAxis)textBox.DataContext;

            var converter = new NaNDoubleToAutoStringConverter();
            var inputAsDouble = (double)(converter.ConvertBack(inputText, null, null, null) ?? double.NaN);

            switch (propertyName)
            {
                case nameof(IAxis.Minimum):
                    textBoxDataContext.Minimum = inputAsDouble;
                    break;
                case nameof(IAxis.Maximum):
                    textBoxDataContext.Maximum = inputAsDouble;
                    break;
                default:
                    var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                    bindingExpression?.UpdateTarget();
                    break;
            }
        }
    }
}
