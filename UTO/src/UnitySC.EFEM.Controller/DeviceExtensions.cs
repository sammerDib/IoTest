using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Agileo.EquipmentModeling
{
    public static class DeviceExtensions
    {
        /// <summary>
        /// Determines if a command can be executed on the device.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="commandName">Name of the command as defined in device's interface.</param>
        /// <param name="context">
        /// <see cref="CommandContext"/> created to test command execution,
        /// or <see langword="null"/> if the command was not found on the device.
        /// </param>
        /// <param name="arguments">
        /// Command parameters to test for execution.
        /// Should have same number, same type, same order as defined in command method signature.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the command can be executed (found, valid state, valid parameters...);
        /// Otherwise <see langword="false"/>.
        /// </returns>
        public static bool CanExecute(
            this Device device,
            string commandName,
            out CommandContext context,
            params object[] arguments)
        {
            // Prepare "can execute" check: Get the command metadata (including preconditions, interlocks, etc.) from the device
            var command = device
                .DeviceType
                .AllCommands()
                .SingleOrDefault(c => c.Name.Equals(commandName, StringComparison.InvariantCulture) && c.Parameters.Count == arguments.Length);

            // Build the context for the command: this allows to specify parameter values
            context = command == null
                ? null
                : new CommandContext(
                    device,
                    command,
                    arguments.Select((arg, i) => new Argument(command.Parameters[i].Name, arg)));

            // Actually check if command can be executed
            return context != null && device.CanExecute(context);
        }
    }
}
