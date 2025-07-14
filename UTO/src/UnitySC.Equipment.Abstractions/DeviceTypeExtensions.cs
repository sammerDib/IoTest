using System;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions
{
    /// <summary>
    /// Provides extension methods to <see cref="DeviceType"/> class.
    /// </summary>
    public static class DeviceTypeExtensions
    {
        public static DeviceType AddPrecondition(
            this DeviceType deviceType,
            string commandName,
            CommandConditionBehavior precondition,
            ILogger logger = null)
        {
            // Check arguments
            if (commandName == null)
            {
                throw new ArgumentNullException(nameof(commandName));
            }

            if (precondition == null)
            {
                throw new ArgumentNullException(nameof(precondition));
            }

            // Attempt to retrieve the command object
            var command = deviceType
                .AllCommands()
                .FirstOrDefault(cmd => cmd.Name.Equals(commandName, StringComparison.Ordinal));
            if (command == null)
            {
                logger?.Debug($"Add precondition failed: command '{commandName}' not found.");
                return deviceType;
            }

            // Check if precondition already exists.
            // In case of multiple device instances (ex: 3 Load Ports), the same DeviceType is updated for all device instances.
            var preconditionName = precondition.GetType().Name;
            if (command.PreConditions.Any(x => x.Name.Equals(preconditionName)))
            {
                // Do not log here since it is nominal case when there are multiple device instances
                return deviceType;
            }

            // Everything ok, add the precondition
            command.PreConditions.SafeAdd(new CommandCondition(preconditionName, precondition));
            return deviceType;
        }

        public static DeviceType RemovePrecondition(
            this DeviceType deviceType,
            string commandName,
            Type preconditionType,
            ILogger logger = null)
        {
            // Check arguments
            if (commandName == null)
            {
                throw new ArgumentNullException(nameof(commandName));
            }

            if (preconditionType == null)
            {
                throw new ArgumentNullException(nameof(preconditionType));
            }

            // Attempt to retrieve the command object
            var command = deviceType
                .AllCommands()
                .FirstOrDefault(cmd => cmd.Name.Equals(commandName, StringComparison.Ordinal));
            if (command == null)
            {
                logger?.Debug($"Remove precondition failed: command '{commandName}' not found.");
                return deviceType;
            }

            // Check if precondition exists.
            // In case of multiple device instances (ex: 3 Load Ports), the same DeviceType is updated for all device instances.
            var preCondition = command.PreConditions.FirstOrDefault(c => c.Behavior.GetType() == preconditionType);
            if (preCondition != null)
            {
                command.PreConditions.Remove(preCondition);
            }

            return deviceType;
        }
    }
}
