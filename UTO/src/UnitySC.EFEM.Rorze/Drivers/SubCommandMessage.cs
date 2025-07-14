using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UnitySC.EFEM.Rorze.Drivers
{
    /// <summary>
    /// Aims to parse and create rorze sub command messages for getting or setting device data or controlling each axis individually.
    /// Format is:
    ///     - For order : "oDDDD.TTTT.CCCC{{[I]}[J]}{=NNNN}" or "oDDDD.TTTT.CCCC{(A)}"
    ///     - For ack   : "aDDDD.TTTT.CCCC:RES"
    /// Where:
    ///     - DDDD is the device ID (e.g. STG1)
    ///     - TTTT is the device data table where the information is stored
    ///     - CCCC is the command (what is to be done)
    ///     - I is the row index of the data table
    ///     - J is the column index of the data table
    ///     - NNNN is the data to be affected in the data table (could be a single value or a list)
    ///     - A is the parameter to be given to the sub-command when it is not used as an array.
    ///     - RES is the result of the data acquisition (if any).
    ///     - String insides "{}" are optional.
    /// </summary>
    public class SubCommandMessage : RorzeMessage
    {
        #region Constructors

        public SubCommandMessage(string frame, IReadOnlyDictionary<string, DevicePart> deviceParts)
            : base(frame, deviceParts)
        {
            // Do nothing here. All is done in the protected "Parse" method.
            // Affectation here would undo property initialization done in "Parse".
        }

        public SubCommandMessage(
            char commandType,
            string deviceType,
            byte deviceId,
            string devicePart,
            string name,
            IReadOnlyList<string> parameters,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            bool useParentheses,
            string suffix = "")
            : base(commandType, deviceType, deviceId, name, deviceParts, parameters, useParentheses)
        {
            DevicePart = devicePart;

            // Insert device part in frame
            var decomposedFrame = Frame.Split('.');
            var newFrame        = decomposedFrame[0] + '.' + devicePart;

            for (var i = 1; i < decomposedFrame.Length; ++i)
            {
                newFrame += '.' + decomposedFrame[i];
            }

            newFrame += suffix;
            Frame = newFrame;
        }

        #endregion Constructors

        #region Properties

        public string DevicePart { get; protected set; }

        #endregion Properties

        #region Override

        protected override void Parse(string frame)
        {
            // Indicative frame used in other comments to explain the parse:
            // "aSTG1.DSTG.GTDT:res"
            // formatted as: <CommandType><DeviceType><DeviceId>.<DevicePart>.<Name>:<Data>

            // Sub commands not always carry data but there is at least the "aSTG1.DSTG.GTDT" part
            if (frame.Length < 15)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(frame),
                    $@"Length ({frame.Length}) should be greater or equal to 15.");
            }

            // Get Command Type
            // 'a' from "aSTG1.DSTG.GTDT:res"
            var validCommandType = new List<char>
            {
                RorzeConstants.CommandTypeAbb.Nak,
                RorzeConstants.CommandTypeAbb.Cancel,
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
            // "STG" from "aSTG1.DSTG.GTDT:res"
            var validDeviceType = new List<string>
            {
                RorzeConstants.DeviceTypeAbb.LoadPort,
                RorzeConstants.DeviceTypeAbb.Robot,
                RorzeConstants.DeviceTypeAbb.Aligner
            };
            var deviceType = frame.Remove(4).Remove(0, 1);
            if (!validDeviceType.Contains(deviceType))
            {
                throw new ArgumentException($@"Unexpected device type: {deviceType}.", nameof(frame));
            }

            DeviceType = deviceType;

            // Get Device Id
            // 1 from "aSTG1.DSTG.GTDT:res"
            string idString = frame.Substring(4, 1);
            if (!byte.TryParse(idString, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture,
                out var deviceId))
            {
                throw new ArgumentException($@"Unexpected device Id: {idString}.", nameof(frame));
            }

            DeviceId = deviceId;

            // Get Device Part
            // "DSTG" from "aSTG1.DSTG.GTDT:res"
            string devicePart = frame.Substring(6, 4);
            if (!CheckDevicePartValidity(deviceType, devicePart))
            {
                throw new ArgumentException(
                    $@"Unexpected sub device for device {deviceType}: {devicePart}.",
                    nameof(frame));
            }

            DevicePart = devicePart;

            // Get message Name
            // "GTDT" from "aSTG1.DSTG.GTDT:res"
            var items = frame.Substring(11).Split(':');
            if (!CheckCommandValidityForDevicePart(deviceType, devicePart, items[0]))
            {
                throw new ArgumentException(
                    $@"Unexpected command for device part {devicePart} of device {deviceType}: {items[0]}.",
                    nameof(frame));
            }

            Name = items[0];

            // Get Data
            // "res" from "aSTG1.DSTG.GTDT:res"
            Data = items.ElementAtOrDefault(1)?.Replace("\r", string.Empty) ?? string.Empty; // Remove last CR
        }

        #endregion Override

        #region Helpers

        public bool CheckDevicePartValidity(string deviceType, string devicePart)
        {
            switch (deviceType)
            {
                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    return _deviceParts.ContainsKey(devicePart);

                case RorzeConstants.DeviceTypeAbb.Robot:
                    // TODO
                    return false;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    // TODO
                    return false;

                default:
                    throw new ArgumentException($"Device \"{deviceType}\" does not support sub-commands for now.");
            }
        }

        public bool CheckCommandValidityForDevicePart(string deviceType, string devicePart, string commandName)
        {
            switch (deviceType)
            {
                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    return _deviceParts[devicePart].ApplicableSubCommands.Contains(commandName);

                case RorzeConstants.DeviceTypeAbb.Robot:
                    // TODO
                    return false;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    // TODO
                    return false;

                default:
                    throw new ArgumentException($"Device \"{deviceType}\" does not support sub-commands for now.");
            }
        }

        #endregion Helpers
    }
}
