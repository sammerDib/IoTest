using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.Equipment.Devices.Controller.Resources;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.Equipment.Devices.Controller
{
    public class CheckJob : CSharpCommandConditionBehavior
    {
        public override void Check(CommandContext context)
        {
            if (context.Device is not Controller controller)
            {
                context.AddContextError(string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.InvalidDeviceType,
                    nameof(Controller),
                    context.Device?.GetType().Name ?? "null"));
                return;
            }

            if (context.GetArgument("job") is not Job job)
            {
                context.AddContextError(Messages.JobNotSpecified);
                return;
            }

            var efem = controller.TryGetDevice<Abstractions.Devices.Efem.Efem>();
            if (efem == null)
            {
                context.AddContextError(Messages.EfemNotAvailable);
                return;
            }

            if (job.Wafers.Count <= 0)
            {
                context.AddContextError(Messages.JobHaveNoSubstrate);
                return;
            }

            //Check substrates
            foreach (var substrate in job.Wafers)
            {
                if (substrate == null)
                {
                    context.AddContextError(Messages.JobHaveEmptySubstrate);
                    return;
                }
            }

            //Check load ports
            var expectedLoadPorts = job.Wafers.Select(x => x.SourcePort).Distinct();
            foreach (var expectedLoadPort in expectedLoadPorts)
            {
                var loadPort = efem.TryGetDevice<Abstractions.Devices.LoadPort.LoadPort>(expectedLoadPort);
                if (loadPort == null)
                {
                    context.AddContextError(string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.ExpectedLoadPortNotAvailable,
                        expectedLoadPort));
                    return;
                }

                if (loadPort.Configuration.CloseDoorAfterRobotAction)
                {
                    if (loadPort.PhysicalState != LoadPortState.Docked
                        && loadPort.PhysicalState != LoadPortState.Undocked
                        && loadPort.PhysicalState != LoadPortState.Open
                        && loadPort.PhysicalState != LoadPortState.Closed)
                    {
                        context.AddContextError(string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.InvalidCarrierState,
                            expectedLoadPort));
                        return;
                    }
                }
                else
                {
                    if (loadPort.PhysicalState != LoadPortState.Open)
                    {
                        context.AddContextError(string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.ExpectedCarrierNotOpen,
                            expectedLoadPort));
                        return;
                    }
                }

            }

            //Check slots
            foreach (var jobData in job.Wafers)
            {
                var loadPort = efem.TryGetDevice<Abstractions.Devices.LoadPort.LoadPort>(jobData.SourcePort);

                if (loadPort.Carrier.OriginalMappingTable.Count < jobData.SourceSlot)
                {
                    context.AddContextError(string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.SlotCanNotBeUsed,
                        jobData.SourceSlot,
                        jobData.SourcePort));
                }

                if (loadPort.Carrier.OriginalMappingTable[jobData.SourceSlot - 1] != SlotState.HasWafer)
                {
                    context.AddContextError(string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.SlotCanNotBeUsed,
                        jobData.SourceSlot,
                        jobData.SourcePort));
                }
            }
        }
    }
}
