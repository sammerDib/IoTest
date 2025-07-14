using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels
{
    public interface IDeviceCommandParameterViewModel
    {
        DeviceCommandParameter Model { get; }

        string Name { get; }
    }
}
