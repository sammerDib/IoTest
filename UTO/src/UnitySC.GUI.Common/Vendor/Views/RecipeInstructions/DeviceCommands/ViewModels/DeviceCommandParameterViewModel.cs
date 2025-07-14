using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels
{
    public abstract class DeviceCommandParameterViewModel<T> : Notifier, IDeviceCommandParameterViewModel where T : DeviceCommandParameter
    {
        protected DeviceCommandParameterViewModel(T model)
        {
            Model = model;
        }

        public string Name => Model.Name;

        DeviceCommandParameter IDeviceCommandParameterViewModel.Model => Model;

        public T Model { get; }
    }
}
