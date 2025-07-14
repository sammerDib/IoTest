using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands
{
    public class DeviceViewModel : Notifier
    {
        private readonly Device _device;

        public DeviceViewModel(DeviceCommandInstruction model, Device device, Func<DeviceCommand, bool> commandFilter)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (device.DeviceType == null)
            {
                throw new ArgumentException(@"DeviceType must not be null", nameof(device));
            }

            _device = device;
            Devices = new ObservableCollection<DeviceViewModel>(
                _device.Devices.Select(d => new DeviceViewModel(model, d, commandFilter)));

            var commands = GetCommands(_device.DeviceType, commandFilter);
            CommandGroups = new ObservableCollection<DeviceCommandGroupViewModel>(
                commands.Select(kvp => new DeviceCommandGroupViewModel(model, _device, kvp.Key, kvp.Value)));

            var selectedCommandGroup = CommandGroups.FirstOrDefault(cg => cg.Commands.Any(c => c.Name.Equals(model.CommandName)))
                ?? CommandGroups.FirstOrDefault();

            if (selectedCommandGroup != null)
            {
                _selectedCommand = selectedCommandGroup.Commands.FirstOrDefault(c => c.Name.Equals(model.CommandName, StringComparison.Ordinal))
                                   ?? selectedCommandGroup.Commands.FirstOrDefault();
            }
        }

        public string Name => _device.Name;

        public int InstanceId => _device.InstanceId;

        public ObservableCollection<DeviceViewModel> Devices { get; }

        public ObservableCollection<DeviceCommandGroupViewModel> CommandGroups { get; }

        private DeviceCommandViewModel _selectedCommand;

        public DeviceCommandViewModel SelectedCommand
        {
            get { return _selectedCommand; }
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedCommand, value))
                {
                    _selectedCommand?.UpdateModel();
                    OnPropertyChanged(nameof(UntypedSelectedCommand));
                }
            }
        }

        /// <summary>
        /// Use of an untyped assessor because the linked treeview does not manage only command type elements.
        /// </summary>
        public object UntypedSelectedCommand
        {
            get { return _selectedCommand; }
            set
            {
                if (value is DeviceCommandViewModel command)
                {
                    SelectedCommand = command;
                }
                OnPropertyChanged(nameof(UntypedSelectedCommand));
            }
        }

        private static IDictionary<string, List<DeviceCommand>> GetCommands(DeviceType deviceType, Func<DeviceCommand, bool> commandFilter)
        {
            var commands = new Dictionary<string, List<DeviceCommand>>();

            var deviceCommands = commandFilter == null ? deviceType.Commands : deviceType.Commands.Where(commandFilter);
            foreach (var command in deviceCommands)
            {
                var category = string.IsNullOrEmpty(command.Category) ? "Commands" : command.Category;
                if (!commands.ContainsKey(category))
                {
                    commands.Add(category, new List<DeviceCommand>());
                }

                commands[category].Add(command);
            }

            if (deviceType.SuperType == null)
            {
                return commands;
            }

            var superTypeCommands = GetCommands(deviceType.SuperType, commandFilter);
            foreach (var kvp in superTypeCommands)
            {
                if (commands.ContainsKey(kvp.Key))
                {
                    commands[kvp.Key].AddRange(kvp.Value);
                    continue;
                }

                commands.Add(kvp.Key, kvp.Value);
            }

            return commands;
        }
    }
}
