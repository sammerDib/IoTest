using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels
{
    public class
        MaterialLocationDeviceCommandParameterViewModel : DeviceCommandParameterViewModel<MaterialLocationDeviceCommandParameter>
    {
        public MaterialLocationDeviceCommandParameterViewModel(
            MaterialLocationDeviceCommandParameter model,
            Device device,
            DeviceCommand command,
            Parameter parameter) : base(model)
        {
            var mm = MaterialManager.GetMaterialManagerFrom(device);
            if (mm == null)
            {
                throw new InvalidOperationException("There is no material manager in the model");
            }

            // PickSafeLocations can filter location if material is present.
            // In this case we want to retrieve available material location for selected device
            Locations = new ObservableCollection<MaterialLocation>(
                mm.LocationPicker.PickSafeLocations(device, command, parameter));
        }

        public ObservableCollection<MaterialLocation> Locations { get; }

        public MaterialLocation TypedValue
        {
            get
            {
                return Locations.FirstOrDefault(l => l.Name.Equals(Model.Value, StringComparison.Ordinal));
            }
            set
            {
                if (Model.Value.Equals(value?.Name, StringComparison.Ordinal))
                {
                    return;
                }

                Model.Value = value?.Name ?? string.Empty;
                OnPropertyChanged();
            }
        }
    }
}
