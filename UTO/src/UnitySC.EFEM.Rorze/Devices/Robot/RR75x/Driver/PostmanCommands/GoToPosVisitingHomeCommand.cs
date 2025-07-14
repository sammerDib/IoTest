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
    /// Aim to ask to the real robot device to move to the designated position after returning the arm to
    /// origin.
    /// </summary>
    public class GoToPosVisitingHomeCommand : RorzeCommand
    {
        #region Constructors

        /// <summary>Create a command that only move all robot to the mechanical origin.</summary>
        public static GoToPosVisitingHomeCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GoToPosVisitingHomeCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade);
        }

        public static GoToPosVisitingHomeCommand NewOrder(
            byte deviceId,
            MoveId moveId,
            Arm_Interpolated arm,
            uint stg,
            uint slt,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
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

            var parameters = new List<string>(4)
            {
                ((int)moveId).ToString(),
                (int)arm >= 0x10
                    ? ((int)arm).ToString("X2")
                    : ((int)arm).ToString("X1"), // Put the right number of digits
                stg.ToString(),
                slt.ToString()
            };

            return new GoToPosVisitingHomeCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private GoToPosVisitingHomeCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.GoToPosVisitingHome,
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
