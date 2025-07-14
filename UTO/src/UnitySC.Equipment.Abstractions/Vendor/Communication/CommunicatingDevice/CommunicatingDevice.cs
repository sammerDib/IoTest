using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

using UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice
{
    public partial class CommunicatingDevice
    {
        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    // When the Initialize command is used through a CommunicatingDevice,
                    // it is needed to add the CheckDriverConnected preconditions on this command.
                    // The only way to do that is by using the code below to add the preconditions dynamically.
                    // When multiple instance of the same DeviceType (ex: 3 Load Ports) the DeviceType is updated
                    // for all the instances.
                    // That is why we have to check if the preconditions has already been added to the command.
                    var initCommand = DeviceType.AllCommands()
                        .FirstOrDefault(x => x.Name == nameof(IGenericDevice.Initialize));
                    if (initCommand != null
                        && initCommand.PreConditions.All(
                            x => x.Name != nameof(CheckDriverConnected)))
                    {
                        initCommand.PreConditions.SafeAdd(
                            new CommandCondition(
                                nameof(CheckDriverConnected),
                                new CheckDriverConnected()));
                    }

                    break;
                case SetupPhase.SettingUp:
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        protected virtual void InternalConnect()
        {
            throw new NotImplementedException("Command Connect has not been implemented");
        }

        protected virtual void InternalDisconnect()
        {
            throw new NotImplementedException("Command Disconnect has not been implemented");
        }

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        protected override void HandleCommandExecutionStateChanged(
            Agileo.EquipmentModeling.CommandExecutionEventArgs e)
        {
            //Connect command does not interact with the device State so it does not need to call the base
            if (e.Execution.Context.Command.Name == nameof(Connect))
            {
                return;
            }

            base.HandleCommandExecutionStateChanged(e);
        }
    }
}
