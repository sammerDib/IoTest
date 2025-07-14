using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;

using OperationStatus = UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationStatus;
using RobotStatus    = UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status.RobotStatus;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class OriginSearchCommand : RorzeCommand
    {
        #region Constructors

        public static OriginSearchCommand NewRobotOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            RobotOriginSearchParameterN n     = RobotOriginSearchParameterN.Unconditional,
            RobotOriginSearchParameterFlg flg = RobotOriginSearchParameterFlg.MoveEachAxisToOrigin)
        {
            var parameters = new List<string>(2) { ((int)n).ToString() };

            if (flg != RobotOriginSearchParameterFlg.NotSet)
                parameters.Add(((int)flg).ToString());

            return new OriginSearchCommand(RorzeConstants.CommandTypeAbb.Order, RorzeConstants.DeviceTypeAbb.Robot, deviceId, sender, eqFacade, parameters.ToArray());
        }

        public static RorzeCommand NewAlignerOrder(
            byte deviceId,
            AlignerOriginSearchParameter parameter,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)

        {
            return new OriginSearchCommand(RorzeConstants.CommandTypeAbb.Order, RorzeConstants.DeviceTypeAbb.Aligner, deviceId, sender, eqFacade, ((int)parameter).ToString("X1"));
        }

        private OriginSearchCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] parameters)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.OriginSearch, sender, eqFacade, parameters)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            // Command is done when hardware has stopped moving 
            bool isDone;

            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.Robot:
                    isDone = new RobotStatus(message.Data).OperationStatus == OperationStatus.Stop;
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    isDone = new AlignerStatus(message.Data).OperationStatus == Devices.Aligner.RA420.Driver.Enums.OperationStatus.Stop;
                    break;

                default:
                    return false;
            }

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order
                && isDone)
            {
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
