using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands
{
    public class DeviceCommandGroupViewModel : Notifier
    {
        public DeviceCommandGroupViewModel(
            DeviceCommandInstruction model,
            Device device,
            string name,
            IEnumerable<DeviceCommand> commands)
        {
            Name = name;
            Commands = new ObservableCollection<DeviceCommandViewModel>(commands.Select(c =>
                new DeviceCommandViewModel(model, device, c)));
        }

        public string Name { get; }

        public ObservableCollection<DeviceCommandViewModel> Commands { get; }
    }
}
