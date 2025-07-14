using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.PostmanCommands
{
    public class ReleaseCarrierCommand : RV201Command
    {
        #region Constructors

        #region Public

        public static ReleaseCarrierCommand NewOrder(
            byte deviceId,
            ReleaseCarrierOperationMode selectedMode,
            ReleaseCarrierUnclampOrMoveToYPosition unclampOrMoveToYPos,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ReleaseCarrierEnableMapping enableMapping = ReleaseCarrierEnableMapping.NotSet,
            ReleaseCarrierRotateAtYPos1 carrierRotateAtYPos1 = ReleaseCarrierRotateAtYPos1.NotSet)
        {
            var parameters = new List<string>(4) { ((int)selectedMode).ToString("X1") };

            // "NotSet" parameter means that the parameter has not to be sent to the real device (not constant number of parameters).
            if (unclampOrMoveToYPos == ReleaseCarrierUnclampOrMoveToYPosition.NotSet)
            {
                return new ReleaseCarrierCommand(
                    RorzeConstants.CommandTypeAbb.Order,
                    deviceId,
                    sender,
                    eqFacade,
                    parameters.ToArray());
            }

            parameters.Add(((int)unclampOrMoveToYPos).ToString("X1"));

            // "NotSet" parameter means that the parameter has not to be sent to the real device (see ReleaseCarrierEnableMapping.NotSet documentation).
            if (enableMapping == ReleaseCarrierEnableMapping.NotSet)
            {
                return new ReleaseCarrierCommand(
                    RorzeConstants.CommandTypeAbb.Order,
                    deviceId,
                    sender,
                    eqFacade,
                    parameters.ToArray());
            }

            parameters.Add(((int)enableMapping).ToString("X1"));

            // "NotSet" parameter means that the parameter has not to be sent to the real device (see ReleaseCarrierRotateAtYPos1.NotSet documentation).
            if (carrierRotateAtYPos1 == ReleaseCarrierRotateAtYPos1.NotSet)
            {
                return new ReleaseCarrierCommand(
                    RorzeConstants.CommandTypeAbb.Order,
                    deviceId,
                    sender,
                    eqFacade,
                    parameters.ToArray());
            }

            parameters.Add(((int)carrierRotateAtYPos1).ToString("X1"));

            return new ReleaseCarrierCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        #endregion Public

        #region Private

        private ReleaseCarrierCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.LoadPort,
                deviceId,
                RorzeConstants.Commands.ReleaseCarrier,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Private

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            var statuses = new RV201LoadPortStatus(message.Data);

            // Command is done when hardware has stopped moving
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order && isDone)
            {
                facade.SendEquipmentEvent(
                    (int)EFEMEvents.ReleaseCarrierCompleted,
                    System.EventArgs.Empty);
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
