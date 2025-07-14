using System.Globalization;

using UnitsNet;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels
{
    public class
        QuantityDeviceCommandParameterViewModel : DeviceCommandParameterViewModel<QuantityDeviceCommandParameter>
    {
        public QuantityDeviceCommandParameterViewModel(QuantityDeviceCommandParameter model) : base(model)
        {
        }

        public IQuantity Quantity
        {
            get
            {
                return Model.GetTypedValue() as IQuantity;
            }
            set
            {
                Model.Value = value?.ToString(CultureInfo.InvariantCulture);
                OnPropertyChanged();
            }
        }
    }
}
