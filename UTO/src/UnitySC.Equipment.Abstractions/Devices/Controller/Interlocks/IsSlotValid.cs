using System.Collections.Generic;
using System.Globalization;

using Agileo.ModelingFramework;
using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Efem.Resources;
using UnitySC.Equipment.Abstractions.Devices.Robot;

namespace UnitySC.Equipment.Abstractions.Devices.Controller
{
    public class IsSlotValid : CSharpInterlockBehavior
    {
        public override void Check(Device responsible, CommandContext context)
        {
            // This interlock checks that slot selected for command exists in source/destination location
            // Interlocks are checked for any command, so "not a robot" can be nominal case (don't add error)
            if (context.Device is not Robot.Robot)
            {
                return;
            }

            var slotsPerDevice = new Dictionary<object, List<byte>>();
            switch (context.Command.Name)
            {
                // We can check only GoToSpecifiedLocation command
                // Pick and Place commands will be validated with IsLocationReadyForTransfer
                case nameof(IRobot.GoToSpecifiedLocation):
                    slotsPerDevice.Add(
                        context.GetArgument("destinationDevice"),
                        new List<byte> { (byte)context.GetArgument("destinationSlot") });
                    break;

                default:
                    // Interlocks are checked for any command, so "command not supported" can be nominal case (don't add error)
                    return;
            }

            // Check for all selected devices that all selected slots are valid
            // (overkill now, but could be useful in case of Transfer command that takes a source and destination)
            foreach (var kvp in slotsPerDevice)
            {
                var deviceName = (kvp.Key as Device)?.Name ?? kvp.Key.GetType().Name;
                OneToManyComposition<MaterialLocation> materialLocations = null;

                switch (kvp.Key)
                {
                    case Aligner.Aligner aligner:
                        materialLocations = aligner.MaterialLocations;
                        break;

                    case ProcessModule.ProcessModule processModule:
                        materialLocations = processModule.MaterialLocations;
                        break;

                    case LoadPort.LoadPort loadPort:
                        materialLocations = loadPort.Carrier?.MaterialLocations;
                        break;

                    default:
                        context.AddContextError(string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.DeviceNotExpected,
                            deviceName));
                        break;
                }

                // When there is no material locations (e.g. no carrier) we can't check that slot is valid
                // Just return. Should be ok, if slot is really invalid command could fail another way
                if (materialLocations == null || materialLocations.Count < 1)
                {
                    return;
                }

                foreach (var slot in kvp.Value)
                {
                    // Check that slot exists
                    if (1 <= slot && slot <= materialLocations.Count)
                    {
                        continue;
                    }

                    // Special case when only one slot
                    // (would be weird to say "not in range 1..1")
                    if (materialLocations.Count == 1)
                    {
                        context.AddContextError(string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.DeviceHaveOnlyOneSlot,
                            deviceName));
                    }
                    else
                    {
                        context.AddContextError(string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.SlotNotInRange,
                            slot,
                            materialLocations.Count));
                    }
                }
            }
        }
    }
}
