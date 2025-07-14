using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    /// <summary>
    /// Aim to ask to the real robot device to: 1 - Return arm to origin 2 - Move to the designated
    /// position 3 - Load the wafer in presence.
    /// </summary>
    /// <remarks>Do not return arm to origin if arm is already at designated position.</remarks>
    public class LoadWaferCommand : RorzeCommand
    {
        private const int MapParameterLength = 5;

        #region Constructors

        /// <summary>Creates a new instance of <see cref="LoadWaferCommand" /> class.</summary>
        /// <param name="deviceId">The id of the device.</param>
        /// <param name="arm">
        /// The arm to be used for loading the wafer. When value is
        /// <see cref="ArmOrBoth_Interpolated.UpperAndLowerArms" />, the slot No. for the upper arm is
        /// designated.
        /// </param>
        /// <param name="stg">
        /// The stopping position associated with the desired stage on which to get the wafer.
        /// </param>
        /// <param name="slt">The slot on which to get the wafer.</param>
        /// <param name="sender">Instance that would send the command.</param>
        /// <param name="eqFacade">Instance that would send event to equipment, if necessary.</param>
        /// <param name="motionType">Describe if the motion must be done partially, by steps or completely.</param>
        /// <param name="map">
        /// Upper arm chucking pattern (Omitting is allowed.). It must have 5 characters. If the upper arm has
        /// five fingers, whether to perform chucking of each finger can be designated by "0" (Not to perform
        /// chucking) or "1" (Perform chucking) using five digits. The head digit signifies the very bottom
        /// finger. If chucking pattern is omitted, the pattern is determined by the setting of the "SVAC"
        /// command.
        /// </param>
        /// <param name="checkOption">Define the checking option to execute while performing the command.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="stg" /> is zero or higher than
        /// 400.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="slt" /> is zero or higher than carrier slots capacity.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="map" /> is given with a length different of 5 characters.
        /// </exception>
        public static LoadWaferCommand NewOrder(
            byte deviceId,
            ArmOrBoth_Interpolated arm,
            uint stg,
            uint slt,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            MotionType motionType = MotionType.NormalMotion,
            string map = null,
            CheckOption checkOption = CheckOption.NoCheck)
        {
            if (stg == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg number can not be zero. Given value is {stg}.");
            }

            if (stg > 400)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg number can not exceed 400. Given value is {stg}.");
            }

            if (slt == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The slt number can not be zero. Given value is {slt}.");
            }

            // TODO: check that slot ID is inside the number of slots.

            var parameters = new List<string>(6)
            {
                (int)arm >= 0x10
                    ? ((int)arm).ToString("X2")
                    : ((int)arm).ToString("X1"), // Put the right number of digits
                stg.ToString(),
                slt.ToString(),
                ((int)motionType).ToString()
            };

            if (map != null)
            {
                if (map.Length != MapParameterLength)
                {
                    throw new InvalidOperationException(
                        $"{nameof(LoadWaferCommand)} - The given map length is not valid.\n"
                        + $"{map} contains {map.Length} characters. {MapParameterLength} were expected.");
                }

                parameters.Add(map);
            }

            if (checkOption != CheckOption.NoCheck)
            {
                // Must give a map parameter in order to give a check option parameter
                if (map == null)
                {
                    parameters.Add("0");
                }

                parameters.Add(((int)checkOption).ToString());
            }

            return new LoadWaferCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        /// <summary>Creates a new instance of <see cref="LoadWaferCommand" /> class.</summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="stg" /> is zero or higher than
        /// 400.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="slt" /> is zero or higher than carrier slots capacity.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="armSpeedAsPercentage" /> is lower than 1% or higher than 100%.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="zAxisSlightSpeedAsPercentage" /> is lower than 1% or higher than 100%.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="armSpeedAsPercentageWhenWaferOnConcaveFinger" /> is lower than 1% or higher than
        /// 100%.
        /// </exception>
        public static LoadWaferCommand NewOrder(
            byte deviceId,
            ArmOrBoth_Interpolated arm,
            uint stg,
            uint slt,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            MotionType motionType,
            uint armSpeedAsPercentage,
            uint zAxisSlightSpeedAsPercentage,
            uint armSpeedAsPercentageWhenWaferOnConcaveFinger)
        {
            if (stg == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg number can not be zero. Given value is {stg}.");
            }

            if (stg > 400)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg number can not exceed 400. Given value is {stg}.");
            }

            if (slt == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg number can not be zero. Given value is {stg}.");
            }

            // TODO: check that slot ID is inside the number of slots.
            if (armSpeedAsPercentage == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "The armSpeedAsPercentage number can not be zero. "
                    + $"Given value is {armSpeedAsPercentage}.");
            }

            if (armSpeedAsPercentage > 100)
            {
                throw new ArgumentOutOfRangeException(
                    "The armSpeedAsPercentage number can not exceed 100. "
                    + $"Given value is {armSpeedAsPercentage}.");
            }

            if (zAxisSlightSpeedAsPercentage == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "The zAxisSlightSpeedAsPercentage number can not be zero. "
                    + $"Given value is {zAxisSlightSpeedAsPercentage}.");
            }

            if (zAxisSlightSpeedAsPercentage > 100)
            {
                throw new ArgumentOutOfRangeException(
                    "The armSpeedAsPercentage number can not exceed 100. "
                    + $"Given value is {zAxisSlightSpeedAsPercentage}.");
            }

            if (armSpeedAsPercentageWhenWaferOnConcaveFinger == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "The zAxisSlightSpeedAsPercentage number can not be zero. "
                    + $"Given value is {zAxisSlightSpeedAsPercentage}.");
            }

            if (armSpeedAsPercentageWhenWaferOnConcaveFinger > 100)
            {
                throw new ArgumentOutOfRangeException(
                    "The armSpeedAsPercentageWhenWaferOnConcaveFinger number can not exceed 100. "
                    + $"Given value is {armSpeedAsPercentageWhenWaferOnConcaveFinger}.");
            }

            var parameters = new List<string>(7)
            {
                (int)arm >= 0x10
                    ? ((int)arm).ToString("X2")
                    : ((int)arm).ToString("X1"), // Put the right number of digits
                stg.ToString(),
                slt.ToString(),
                ((int)motionType).ToString(),
                armSpeedAsPercentage.ToString(),
                zAxisSlightSpeedAsPercentage.ToString(),
                armSpeedAsPercentageWhenWaferOnConcaveFinger.ToString()
            };

            return new LoadWaferCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private LoadWaferCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.LoadWafer,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages and only when command is an order.
            if (message.Name != RorzeConstants.Commands.StatusAcquisition
                || Command.CommandType != RorzeConstants.CommandTypeAbb.Order)
            {
                return false;
            }

            var statuses = new RobotStatus(message.Data);

            // Command is done when hardware has stopped moving 
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete. Otherwise, command was not addressed explicitly to the current object.
            if (!isDone)
            {
                return false;
            }

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
