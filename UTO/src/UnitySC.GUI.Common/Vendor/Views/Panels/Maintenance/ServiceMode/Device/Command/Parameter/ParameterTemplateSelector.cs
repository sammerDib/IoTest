using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate NumericTemplate { get; set; }
        public DataTemplate UnitNetTemplate { get; set; }
        public DataTemplate MaterialLocationTemplate { get; set; }
        public DataTemplate MaterialLocationContainerTemplate { get; set; }
        public DataTemplate EnumerableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var viewModel = item as ParameterViewModel;
            switch (viewModel)
            {
                case null:
                    return null;
                case EnumerableParameterViewModel:
                    return EnumerableTemplate;
                case StringParameterViewModel:
                    return StringTemplate;
                case MaterialLocationChoiceViewModel:
                    return MaterialLocationTemplate;
                case MaterialLocationContainerChoiceViewModel:
                    return MaterialLocationContainerTemplate;
                case QuantityParameterViewModel:
                    return UnitNetTemplate;
                default:
                    var numericType = viewModel.Type;
                    return numericType == typeof(bool)
                        ? BoolTemplate
                        : NumericTemplate;
            }
        }
    }
}
