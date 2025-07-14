using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.ProcessingFramework.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public class DeviceCommandProcessInstruction : ProcessInstruction, IInstructionAbortable
    {
        #region Fields

        private readonly DeviceCommand _command;
        private readonly Device _device;
        private readonly DeviceCommandInstruction _recipeInstruction;

        #endregion Fields

        #region Constructor

        public DeviceCommandProcessInstruction(Device device, DeviceCommandInstruction recipeInstruction)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (recipeInstruction == null)
            {
                throw new ArgumentNullException(nameof(recipeInstruction));
            }

            _recipeInstruction = recipeInstruction;
            _device = device;
            var commands = GetRelatedCommands(device.DeviceType);
            _command = commands.FirstOrDefault(cmd => cmd.Name.Equals(recipeInstruction.CommandName));
        }

        #endregion Constructor

        #region Properties

        public bool IsIntructionAborted { get; private set; }

        public string DeviceName => _device.Name;

        #endregion Properties

        #region Methods

        public void AbortInstruction()
        {
            IsIntructionAborted = true;
        }

        #endregion Methods

        #region Override

        public override void Execute()
        {
            var args = new List<Argument>();
            foreach (var recipeCommandParameter in _recipeInstruction.CommandParameters)
            {
                var deviceCommandParameter =
                    _command.Parameters.FirstOrDefault(p => p.Name.Equals(recipeCommandParameter.Name));

                if (deviceCommandParameter == null)
                {
                    throw new InvalidOperationException(
                        $"Parameter {recipeCommandParameter.Name} cannot be found for command {_command.Name} of device {_device.Name}");
                }

                args.Add(new Argument(recipeCommandParameter.Name, recipeCommandParameter.GetTypedValue()));
            }

            _device.Run(new CommandExecution(_device, _command.Name, args));
        }

        protected override void LocalizeName()
        {

            Name = LocalizationManager.GetString(
                nameof(ProcessInstructionsResources.DEVICE_COMMAND_PROCESS_INSTRUCTION));
        }

        protected override Instruction CloneInstruction()
        {
            return new DeviceCommandProcessInstruction(_device, _recipeInstruction)
            {
                Details = Details,
                ExecutorId = ExecutorId,
                Modifier = Modifier,
                FormattedLabel = FormattedLabel
            };
        }

        #endregion Override

        #region Private Methods

        private ICollection<DeviceCommand> GetRelatedCommands(DeviceType selectedDevice)
        {
            var commands = selectedDevice?.Commands?.ToList();
            if (selectedDevice?.SuperType != null)
            {
                commands?.AddRange(GetRelatedCommands(selectedDevice.SuperType));
            }

            return commands;
        }

        #endregion Private Methods
    }
}
