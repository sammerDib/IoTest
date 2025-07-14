using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EFEM.Controller.HostInterface
{
    public class Message
    {
        public Message(string frame)
        {
            // Check we have something to parse
            if (frame.Length <= Constants.EndOfFrame.Length)
            {
                throw new MessageParseException(
                    Constants.Errors[ErrorCode.AbnormalFormatOfCommand],
                    message: "Empty string was received.");
            }

            if (!frame.Contains(Constants.EndOfFrame))
            {
                throw new MessageParseException(Constants.Errors[ErrorCode.HaveNoEndCharacter]);
            }

            // Sample frame: oLOAD:2_2_04
            // Frame format: <CommandType><CommandName><CommandSeparator><CommandParameters>
            // Type and Name are fixed size and always present, separate them from optional parameters
            Frame = frame;
            var frameParts = frame.Split(
                new[] { Constants.CommandSeparator, Constants.EndOfFrame },
                StringSplitOptions.RemoveEmptyEntries);

            // Get command type and name: 'o' and "LOAD" from sample frame oLOAD:2_2_04
            try
            {
                CommandType = frameParts[0][0];
                CommandName = frameParts[0].Remove(0, 1);
            }
            catch (Exception e)
            {
                throw new MessageParseException(Constants.Errors[ErrorCode.AbnormalFormatOfCommand], innerException: e);
            }

            // Command type should be known
            if (typeof(Constants.CommandType)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .All(fi => !fi.GetValue(null).Equals(CommandType)))
            {
                throw new MessageParseException(
                    Constants.Errors[ErrorCode.AbnormalFormatOfCommand],
                    message: $"Unknown command type '{CommandType}'.");
            }

            // Command name should be 4 characters wide (check this before checking that name is known)
            if (CommandName.Length != 4)
            {
                throw new MessageParseException(
                    Constants.Errors[ErrorCode.AbnormalFormatOfCommand],
                    message: $"Command name '{CommandName}' does not have 4 characters.");
            }

            // Command name should be known (case-sensitive)
            if (typeof(Constants.Commands)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .All(fi => !fi.GetValue(null).ToString().Equals(CommandName, StringComparison.InvariantCulture)))
            {
                throw new MessageParseException(
                    Constants.Errors[ErrorCode.NoThisCommand],
                    new Message(CommandType, CommandName, new List<string[]>()),
                    $"Unknown command name '{CommandName}'.");
            }

            // Command separator should be present
            if (!frame.Contains(Constants.CommandSeparator))
            {
                throw new MessageParseException(
                    Constants.Errors[ErrorCode.AbnormalFormatOfCommand],
                    message: $"Missing command separator ('{Constants.CommandSeparator}').");
            }

            // No parameters detected, ok to return
            if (frameParts.Length < 2)
                return;

            // Command parameters also has some kind of format
            // Sample frame: eWARM:6/1_1_01/1_1_02
            // First split 'parameters': 6, 1_1_01 and 1_1_02 from sample frame above
            var commandParameters = frameParts[1]
                .Split(new[] { Constants.ParameterSeparator }, StringSplitOptions.RemoveEmptyEntries);
            CommandParameters = new List<string[]>(commandParameters.Length);

            // Then split 'arguments': 1, 1 and 02 from sample parameter "1_1_02"
            foreach (var commandParameter in commandParameters)
                CommandParameters.Add(commandParameter.Split(
                    new[] { Constants.ArgumentSeparator },
                    StringSplitOptions.RemoveEmptyEntries));
        }

        // TODO Maybe try to improve commandParameters to make more obvious the choices one have:
        // - List of 2 items being an array of one string
        // - List of 1 item being an array of two strings
        public Message(char commandType, string commandName, List<string[]> commandParameters)
        {
            CommandType = commandType;
            CommandName = commandName;
            CommandParameters = commandParameters;

            Frame = string.Empty;
            Frame += commandType;
            Frame += commandName;
            Frame += Constants.CommandSeparator;

            if (commandParameters != null)
            {
                for (var parameterIndex = 0; parameterIndex < commandParameters.Count; ++parameterIndex)
                {
                    var commandParameter = commandParameters[parameterIndex];

                    for (var argumentIndex = 0; argumentIndex < commandParameter.Length; ++argumentIndex)
                    {
                        var parameterArgument = commandParameter[argumentIndex];
                        Frame += parameterArgument;

                        if (argumentIndex < commandParameter.Length - 1)
                            Frame += Constants.ArgumentSeparator;
                    }

                    if (parameterIndex < commandParameters.Count - 1)
                        Frame += Constants.ParameterSeparator;
                }
            }
        }

        public char CommandType { get; }

        public string CommandName { get; }

        public List<string[]> CommandParameters { get; } = new List<string[]>();

        public string Frame { get; }
    }
}
