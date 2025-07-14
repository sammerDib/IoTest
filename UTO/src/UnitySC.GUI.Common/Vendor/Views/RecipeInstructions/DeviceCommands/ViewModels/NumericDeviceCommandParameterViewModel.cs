using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels
{
    public class NumericDeviceCommandParameterViewModel : DeviceCommandParameterViewModel<NumericDeviceCommandParameter>
    {
        public NumericDeviceCommandParameterViewModel(NumericDeviceCommandParameter model) : base(model)
        {
        }

        public double Value
        {
            get { return Model.Value; }
            set
            {
                if (Model.Value.Equals(value))
                {
                    return;
                }

                Model.Value = value;
                OnPropertyChanged();
            }
        }
    }
}
