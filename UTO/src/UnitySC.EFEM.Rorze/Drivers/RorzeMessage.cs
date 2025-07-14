using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UnitySC.EFEM.Rorze.Drivers
{
    public class RorzeMessage
    {
        #region Fields

        protected readonly IReadOnlyDictionary<string, DevicePart> _deviceParts;

        #endregion

        public RorzeMessage(string frame, IReadOnlyDictionary<string, DevicePart> deviceParts)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            // Allowed because virtual method is an initializer method, which call will not be overriden by the derived constructor.
            _deviceParts = deviceParts;
            Parse(frame);
        }

        public RorzeMessage(
            char commandType,
            string deviceType,
            byte deviceId,
            string name,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            IReadOnlyList<string> parameters,
            bool useParentheses = true)
        {
            CommandType = commandType;
            DeviceType  = deviceType;
            DeviceId    = deviceId;
            Name        = name;
            Frame       = string.Empty;
            _deviceParts = deviceParts;
            
            Frame += commandType;
            Frame += deviceType;
            Frame += deviceId.ToString("X1");
            Frame += '.';
            Frame += name;

            Data = string.Empty;
            if (parameters != null && parameters.Count > 0)
            {
                CommandParameters = new List<string>(parameters.Count);

                if (useParentheses)
                {
                    Data += '(';

                    for (var paramIndex = 0; paramIndex < parameters.Count; ++paramIndex)
                    {
                        var parameter = parameters[paramIndex];
                        CommandParameters.Add(parameter);
                        Data += parameter;

                        if (paramIndex < parameters.Count - 1)
                            Data += ',';
                    }

                    Data += ')';
                }
                else
                {
                    foreach (var parameter in parameters)
                    {
                        CommandParameters.Add(parameter);
                        Data += '[' + parameter + ']';
                    }
                }
            }

            Frame += Data;
        }

        public char CommandType { get; protected set; }

        public string DeviceType { get; protected set; }

        public byte DeviceId { get; protected set; }
        
        public string Name { get; protected set; }

        public string Data { get; protected set; }

        /// <summary>
        /// Has a value only when the <see cref="CommandType"/> is <see cref="RorzeConstants.CommandTypeAbb.Order"/>.
        /// </summary>
        public List<string> CommandParameters { get; }

        public string Frame { get; protected set; }

        protected virtual void Parse(string frame)
        {
            // Indicative frame used in other comments to explain the parse:
            // "aSTG1.STAT:s1/s2"
            // formatted as: <CommandType><DeviceType><DeviceId>.<Name>:<Data>

            // Messages not always carry data but there is at least the "aSTG1.STAT" part
            if (frame.Length < 10)
            {
                throw new ArgumentOutOfRangeException(nameof(frame),
                    $@"Length ({frame.Length}) should be greater or equal to 10.");
            }

            // Remove last CR
            frame = frame.Replace("\r", string.Empty);

            // Get Command Type
            // 'a' from "aSTG1.STAT:s1/s2"
            var validCommandType = new List<char>
            {
                RorzeConstants.CommandTypeAbb.Nak,
                RorzeConstants.CommandTypeAbb.Cancel,
                RorzeConstants.CommandTypeAbb.Event,
                RorzeConstants.CommandTypeAbb.Order,
                RorzeConstants.CommandTypeAbb.Ack,
            };
            var commandType = frame[0];
            if (!validCommandType.Contains(commandType))
            {
                throw new ArgumentException($@"Unexpected command type: {commandType}.", nameof(frame));
            }

            CommandType = commandType;

            // Get Device Type
            // "STG" from "aSTG1.STAT:s1/s2"
            var validDeviceType = new List<string>
            {
                RorzeConstants.DeviceTypeAbb.LoadPort,
                RorzeConstants.DeviceTypeAbb.Robot,
                RorzeConstants.DeviceTypeAbb.Aligner,
                RorzeConstants.DeviceTypeAbb.IO
            };
            var deviceType = frame.Remove(4).Remove(0, 1);
            if (!validDeviceType.Contains(deviceType))
            {
                throw new ArgumentException($@"Unexpected device type: {deviceType}.", nameof(frame));
            }

            DeviceType = deviceType;

            // Get Device Id
            // 1 from "aSTG1.STAT:s1/s2"
            string idString = frame.Substring(4, 1);
            if (!byte.TryParse(idString, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var deviceId))
            {
                throw new ArgumentException($@"Unexpected device Id: {idString}.", nameof(frame));
            }

            DeviceId = deviceId;

            // Get message Name
            // "STAT" from "aSTG1.STAT:s1/s2"
            var items = frame.Substring(6).Split(':');
            Name = items[0];

            // Get Data
            // "s1/s2" from "aSTG1.STAT:s1/s2"
            Data = items.ElementAtOrDefault(1) ?? string.Empty;
        }
    }
}
