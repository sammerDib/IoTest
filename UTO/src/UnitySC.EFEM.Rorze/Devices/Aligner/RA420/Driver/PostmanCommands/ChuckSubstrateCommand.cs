using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands
{
    /// <summary>
    /// Chucks the substrate on the spindle axis.
    /// <remarks>
    /// If the Aligner has no chucking mechanism, the logical processing is done.
    /// </remarks>
    /// <remarks>If chucking failed, chucking turns off.</remarks>
    /// </summary>
    public class ChuckSubstrateCommand : RorzeCommand
    {
        #region Constructors

        public static ChuckSubstrateCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ChuckSubstrateZAxisMovement zAxisMovement = ChuckSubstrateZAxisMovement.NoZAxisMove,
            bool raiseErrorOnChuckingFailed = true)
        {
            var parameters = new[]
            {
                ((int)zAxisMovement).ToString(),
                raiseErrorOnChuckingFailed
                    ? "0"
                    : "1"
            };

            return new ChuckSubstrateCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters);
        }

        private ChuckSubstrateCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Aligner,
                deviceId,
                RorzeConstants.Commands.ChuckSubstrate,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Constructors

        #region RorzeCommands

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages and only when command is an order.
            if (message.Name != RorzeConstants.Commands.StatusAcquisition
                || Command.CommandType != RorzeConstants.CommandTypeAbb.Order)
            {
                return false;
            }

            var statuses = new AlignerStatus(message.Data);

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

        #endregion RorzeCommands
    }
}
