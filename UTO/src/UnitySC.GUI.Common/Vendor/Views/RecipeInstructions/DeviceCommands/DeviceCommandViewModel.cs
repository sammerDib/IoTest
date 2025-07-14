using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using Humanizer;

using UnitsNet;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;
using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands.ViewModels;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.DeviceCommands
{
    public class DeviceCommandViewModel : Notifier
    {
        private readonly DeviceCommandInstruction _model;
        private readonly DeviceCommand _command;

        public DeviceCommandViewModel(DeviceCommandInstruction model, Device device, DeviceCommand command)
        {
            _model = model;
            _command = command ?? throw new ArgumentNullException(nameof(command));
            Parameters = new ObservableCollection<IDeviceCommandParameterViewModel>(GetParameters(model, device, command));
        }

        public string Name => _command.Name;
        public string DisplayName => string.IsNullOrEmpty(_command.Name) ? string.Empty : _command.Name.Humanize();

        public ObservableCollection<IDeviceCommandParameterViewModel> Parameters { get; }

        public void UpdateModel()
        {
            _model.CommandName = _command.Name;
            _model.CommandParameters = Parameters.Select(p => p.Model).ToList();
        }

        private static IEnumerable<IDeviceCommandParameterViewModel> GetParameters(
            DeviceCommandInstruction instruction,
            Device device,
            DeviceCommand command)
        {
            foreach (var parameter in command.Parameters)
            {
                DeviceCommandParameter model = null;
                if (command.Name.Equals(instruction.CommandName, StringComparison.Ordinal))
                {
                    model = instruction.CommandParameters.FirstOrDefault(cp =>
                        cp.Name.Equals(parameter.Name, StringComparison.Ordinal));
                }

                yield return NewParameterViewModel(model, device, command, parameter);
            }
        }

        private static IDeviceCommandParameterViewModel NewParameterViewModel(
            DeviceCommandParameter model,
            Device device,
            DeviceCommand deviceCommand,
            Parameter parameter)
        {
            var type = parameter.Type as CSharpType;
            if (type == null)
            {
                return null;
            }

            var platformType = type.PlatformType;
            if (platformType.IsEnum)
            {
                var enumParameter = model as EnumerableDeviceCommandParameter
                                    ?? new EnumerableDeviceCommandParameter(parameter);

                return new EnumerableDeviceCommandParameterViewModel(enumParameter);
            }

            if (platformType == typeof(string))
            {
                var stringParameter =
                    model as StringDeviceCommandParameter ?? new StringDeviceCommandParameter(parameter);

                return new StringDeviceCommandParameterViewModel(stringParameter);
            }

            if (platformType.Assembly == typeof(Acceleration).Assembly || parameter.Unit != null)
            {
                var quantityParameter = model as QuantityDeviceCommandParameter
                                        ?? new QuantityDeviceCommandParameter(parameter);

                return new QuantityDeviceCommandParameterViewModel(quantityParameter);
            }

            if (platformType == typeof(IMaterialLocationContainer))
            {
                var containerParameter = model as MaterialLocationContainerDeviceCommandParameter
                                         ?? new MaterialLocationContainerDeviceCommandParameter(parameter);

                return new MaterialLocationContainerDeviceCommandParameterViewModel(containerParameter);
            }

            if (type.Name.Equals(nameof(MaterialLocationType)))
            {
                var locationParameter = model as MaterialLocationDeviceCommandParameter
                                        ?? new MaterialLocationDeviceCommandParameter(parameter);

                return new MaterialLocationDeviceCommandParameterViewModel(
                    locationParameter, device, deviceCommand, parameter);
            }

            if (platformType.IsPrimitive && platformType == typeof(bool))
            {
                var boolParameter = model as BoolDeviceCommandParameter ?? new BoolDeviceCommandParameter(parameter);
                return new BoolDeviceCommandParameterViewModel(boolParameter);
            }

            if (platformType.IsPrimitive && platformType != typeof(char))
            {
                var numericParameter = model as NumericDeviceCommandParameter
                                       ?? new NumericDeviceCommandParameter(parameter);

                if (numericParameter.PlatformType == null)
                {
                    // Initialize PlatformType for existing recipe where this value does not exist (null)
                    numericParameter.PlatformType = ((CSharpType)parameter.Type).PlatformType.FullName;
                }

                return new NumericDeviceCommandParameterViewModel(numericParameter);
            }

            return null;
        }
    }
}
