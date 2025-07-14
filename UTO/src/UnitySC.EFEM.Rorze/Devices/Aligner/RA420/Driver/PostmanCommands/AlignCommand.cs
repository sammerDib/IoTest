using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands
{
    /// <summary>Performs the alignment of a substrate.</summary>
    public class AlignCommand : RorzeCommand
    {
        #region Constructors

        public static AlignCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            AlignmentMode alignmentMode,
            AlignmentValue alignmentValue,
            AlignPostOperationActions postOperationActions = AlignPostOperationActions.NoExtraMove,
            byte alignAsForSpecificConfiguration = 0)
        {
            var parameters = new List<string>
            {
                ((int)alignmentMode).ToString(),
                alignmentValue.StringRepresentation,
                ((int)postOperationActions).ToString(),
                alignAsForSpecificConfiguration.ToString()
            };

            return new AlignCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private AlignCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Aligner,
                deviceId,
                RorzeConstants.Commands.Align,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Constructors

        #region Rorze Command

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

        #endregion Rorze Command
    }
}
