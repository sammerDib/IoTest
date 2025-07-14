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
    /// position 3 - Extend the arm.
    /// </summary>
    /// <remarks>Do not return arm to origin if arm is already at designated position.</remarks>
    public class ExtendRobotArmCommand : RorzeCommand
    {
        #region Constructors

        /// <summary>Creates a new instance of <see cref="ExtendRobotArmCommand" /> class.</summary>
        /// <param name="deviceId">The id of the device.</param>
        /// <param name="extendId">The id to extend the arm</param>
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
        /// <param name="flag">Describe if the motion must be done partially, by steps or completely.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="stg" /> is zero or higher than
        /// 400.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="slt" /> is zero or higher than carrier slots capacity.
        /// </exception>
        public static ExtendRobotArmCommand NewOrder(
            byte deviceId,
            ExtendId extendId,
            ArmOrBoth_Interpolated arm,
            uint stg,
            uint slt,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ExtendFlag flag = ExtendFlag.PerformsArmOrigin)
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

            var parameters = new List<string>(6)
            {
                ((int)extendId).ToString(),
                (int)arm >= 0x10
                    ? ((int)arm).ToString("X2")
                    : ((int)arm).ToString("X1"), // Put the right number of digits
                stg.ToString(),
                slt.ToString(),
                ((int)flag).ToString()
            };

            return new ExtendRobotArmCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private ExtendRobotArmCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.ExtendArm,
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
