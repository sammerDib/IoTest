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
    /// Performs the mapping operation on the designated stage.
    /// This command performs the mapping operation only. To acquire the mapping pattern, use the "GMAP" command after the mapping operation is completed.
    /// (Refer to "c) Mapping pattern acquisition in "3.1.3 Other commands".)
    /// </summary>
    public class PerformWaferMappingCommand : RorzeCommand
    {
        #region Constructors

        public static PerformWaferMappingCommand NewOrder(
            byte deviceId,
            uint stg,
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

            var parameters = new List<string> { stg.ToString() };

            return new PerformWaferMappingCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private PerformWaferMappingCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.PerformWaferMapping,
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
