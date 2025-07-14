using System;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

namespace UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice
{
    public partial class UnityCommunicatingDevice
    {
        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    // When the Initialize command is used through a CommunicatingDevice,
                    // it is needed to add preconditions on this command.
                    // The only way to do that is by using the code below to add the preconditions dynamically.
                    DeviceType.AddPrecondition(
                        nameof(IGenericDevice.Initialize),
                        new IsCommunicating(),
                        Logger);
                    break;
                case SetupPhase.SettingUp:
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        protected abstract void InternalStartCommunication();

        protected abstract void InternalStopCommunication();

        protected override void HandleCommandExecutionStateChanged(
            CommandExecutionEventArgs e)
        {
            //StartCommunication and StopCommunication commands does not interact
            //with the device State so it does not need to call the base
            if (e.Execution.Context.Command.Name == nameof(StartCommunication)
                || e.Execution.Context.Command.Name == nameof(StopCommunication))
            {
                return;
            }

            base.HandleCommandExecutionStateChanged(e);
        }
    }
}
