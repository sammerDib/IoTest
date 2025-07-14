using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class ExchangeWaferCommand : RorzeCommand
    {
        #region Constructors

        public static ExchangeWaferCommand NewOrder(
            byte deviceId,
            ArmOrBoth_Interpolated pickArm,
            uint stg,
            uint slt,
            ExchangeMotionType id,
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
                    $"The slt number can not be zero. Given value is {slt}.");
            }

            if (pickArm > ArmOrBoth_Interpolated.LowerArm)
            {
                throw new ArgumentOutOfRangeException(
                    $"The arm must be lower or upper. Given value is {pickArm}.");
            }

            var parameters = new List<string>()
            {
                ((int)pickArm).ToString("X1"), // Put the right number of digits
                stg.ToString(),
                slt.ToString(),
                ((int)id).ToString()
            };

            return new ExchangeWaferCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private ExchangeWaferCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.ExchangeWafer,
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
