using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class TransferWaferCommand : RorzeCommand
    {
        #region Constructors

        public static TransferWaferCommand NewOrder(
            byte deviceId,
            ArmOrBoth_Interpolated pickArm,
            uint pickStage,
            uint pickSlot,
            ArmOrBoth_Interpolated placeArm,
            uint placeStage,
            uint placeSlot,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            if (pickStage == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg1 number can not be zero. Given value is {pickStage}.");
            }

            if (pickStage > 400)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg1 number can not exceed 400. Given value is {pickStage}.");
            }

            if (pickSlot == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The slt1 number can not be zero. Given value is {pickSlot}.");
            }

            if (placeStage == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg1 number can not be zero. Given value is {placeStage}.");
            }

            if (placeStage > 400)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg1 number can not exceed 400. Given value is {placeStage}.");
            }

            if (placeSlot == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The slt1 number can not be zero. Given value is {placeSlot}.");
            }

            var parameters = new List<string>(6)
            {
                (int)pickArm >= 0x10
                    ? ((int)pickArm).ToString("X2")
                    : ((int)pickArm).ToString("X1"), // Put the right number of digits
                pickStage.ToString(),
                pickSlot.ToString(),
                (int)placeArm >= 0x10
                    ? ((int)placeArm).ToString("X2")
                    : ((int)placeArm).ToString("X1"), // Put the right number of digits
                placeStage.ToString(),
                placeSlot.ToString()
            };

            return new TransferWaferCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private TransferWaferCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.TransferWafer,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion

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

        #endregion
    }
}
