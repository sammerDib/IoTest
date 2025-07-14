using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get general statuses command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.5.9 oSTAT
    /// </summary>
    public class GetGeneralStatusesCommand : BaseCommand
    {
        #region Constructors

        public static GetGeneralStatusesCommand NewOrder(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            return new GetGeneralStatusesCommand(sender, eqFacade, logger, equipmentManager);
        }

        public static GetGeneralStatusesCommand NewEvent(
            GeneralStatus status,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = new List<string[]>(1) { new[] { status.ToString() } };
            var message    = new Message(Constants.CommandType.Event, Constants.Commands.GetGeneralStatuses, parameters);

            return new GetGeneralStatusesCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        public GetGeneralStatusesCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.GetGeneralStatuses, sender, eqFacade, logger, equipmentManager)
        {
        }

        private GetGeneralStatusesCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion Constructors

        #region BaseCommand

        protected override bool TreatOrder(Message message)
        {
            // Check number of parameters
            if (message.CommandParameters.Count != 0)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            var efem = EquipmentManager.Efem;
            var status = new GeneralStatus(
                efem.OperationMode,
                efem.RobotStatus, efem.RobotSpeed,
                efem.LoadPortStatus1, efem.IsLoadPort1CarrierPresent,
                efem.LoadPortStatus2, efem.IsLoadPort2CarrierPresent,
                efem.LoadPortStatus3, efem.IsLoadPort3CarrierPresent,
                efem.LoadPortStatus4, efem.IsLoadPort4CarrierPresent,
                efem.AlignerStatus, efem.IsAlignerCarrierPresent,
                efem.SafetyDoorSensor,
                efem.VacuumSensor,
                efem.AirSensor);

            var parameters    = new List<string[]>(1) { new[] { status.ToString() } };
            var messageToSend = new Message(Constants.CommandType.Ack, Constants.Commands.GetGeneralStatuses, parameters);

            // Send acknowledge response with results
            SendAcknowledge(messageToSend);

            return true;
        }

        #endregion BaseCommand
    }
}
