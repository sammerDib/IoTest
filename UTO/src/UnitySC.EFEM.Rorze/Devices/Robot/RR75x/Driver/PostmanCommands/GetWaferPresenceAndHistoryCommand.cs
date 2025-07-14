using System;

using Agileo.Common.Communication;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Devices.Robot.Converters;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    /// <summary>
    /// Wafer ID acquisition [GWID] Acquires the ID of the wafer on the arm. The wafer ID is six-digit
    /// decimal numeral. The top three digits are stage No. and the bottom three digits are slot No. In the
    /// case of "999999", the wafer ID is undefined.
    /// </summary>
    public class GetWaferPresenceAndHistoryCommand : RorzeCommand
    {
        private readonly GetWaferPresenceArmParameter _askedArm = GetWaferPresenceArmParameter.Both;
        private readonly IStoppingPositionConverter _stoppingPositionConverter;

        #region Constructors

        public static GetWaferPresenceAndHistoryCommand NewOrder(
            byte deviceId,
            IStoppingPositionConverter stoppingPositionConverter,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            GetWaferPresenceArmParameter arm = GetWaferPresenceArmParameter.Both)
        {
            return new GetWaferPresenceAndHistoryCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                arm,
                stoppingPositionConverter,
                sender,
                eqFacade);
        }

        public static GetWaferPresenceAndHistoryCommand NewEvent(
            byte deviceId,
            IStoppingPositionConverter stoppingPositionConverter,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetWaferPresenceAndHistoryCommand(
                RorzeConstants.CommandTypeAbb.Event,
                deviceId,
                stoppingPositionConverter,
                sender,
                eqFacade);
        }

        private GetWaferPresenceAndHistoryCommand(
            char commandType,
            byte deviceId,
            GetWaferPresenceArmParameter arm,
            IStoppingPositionConverter stoppingPositionConverter,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.GetWaferPresenceAndHistory,
                sender,
                eqFacade,
                ((int)arm).ToString())
        {
            _askedArm = arm;
            _stoppingPositionConverter = stoppingPositionConverter;
        }

        private GetWaferPresenceAndHistoryCommand(
            char commandType,
            byte deviceId,
            IStoppingPositionConverter stoppingPositionConverter,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.GetWaferPresenceAndHistory,
                sender,
                eqFacade)
        {
            _stoppingPositionConverter = stoppingPositionConverter;
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            SubstratePresenceEventArgs args;
            var statuses = message.Data.Replace(":", string.Empty).Split(',');

            // A received robot wafer presence status could represent lower, upper or both arm(s).
            switch (_askedArm)
            {
                case GetWaferPresenceArmParameter.Lower:
                case GetWaferPresenceArmParameter.Upper:
                    args = ToSubstratePresenceAndHistoryEventArgs(
                        statuses[0],
                        message.DeviceId,
                        _askedArm);
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.WaferPresenceAndHistoryReceived,
                        args);
                    break;

                case GetWaferPresenceArmParameter.Both:
                    args = ToSubstratePresenceAndHistoryEventArgs(
                        statuses[0],
                        message.DeviceId,
                        GetWaferPresenceArmParameter.Upper);
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.WaferPresenceAndHistoryReceived,
                        args);

                    args = ToSubstratePresenceAndHistoryEventArgs(
                        statuses[1],
                        message.DeviceId,
                        GetWaferPresenceArmParameter.Lower);
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.WaferPresenceAndHistoryReceived,
                        args);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            facade.SendEquipmentEvent((int)EFEMEvents.WaferPresenceAndHistoryCompleted, null);

            CommandComplete();

            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            if (message.Name != RorzeConstants.Commands.GetSubstratePresence)
            {
                return false;
            }

            var statuses = message.Data.Replace(":", string.Empty).Split(',');

            var args = ToSubstratePresenceAndHistoryEventArgs(
                statuses[0],
                message.DeviceId,
                GetWaferPresenceArmParameter.Upper);
            facade.SendEquipmentEvent((int)EFEMEvents.WaferPresenceAndHistoryReceived, args);

            args = ToSubstratePresenceAndHistoryEventArgs(
                statuses[1],
                message.DeviceId,
                GetWaferPresenceArmParameter.Lower);
            facade.SendEquipmentEvent((int)EFEMEvents.WaferPresenceAndHistoryReceived, args);

            return true;
        }

        #endregion RorzeCommand

        #region Private Methods

        private SubstratePresenceAndHistoryEventArgs ToSubstratePresenceAndHistoryEventArgs(
            string status,
            byte port,
            GetWaferPresenceArmParameter arm)
        {
            Parser(status, out var undefined, out var presence, out var stageId, out var slotId);

            SubstratePresenceAndHistoryEventArgs args;
            if (undefined)
            {
                args = new SubstratePresenceAndHistoryEventArgs(
                    SlotState.Correct,
                    TransferLocation.Robot,
                    port,
                    (byte)arm,
                    TransferLocation.DummyPortA,
                    0);
            }
            else if (!presence)
            {
                args = new SubstratePresenceAndHistoryEventArgs(
                    SlotState.Empty,
                    TransferLocation.Robot,
                    port,
                    (byte)arm,
                    TransferLocation.DummyPortA,
                    0);
            }
            else
            {
                args = new SubstratePresenceAndHistoryEventArgs(
                    SlotState.Correct,
                    TransferLocation.Robot,
                    port,
                    (byte)arm,
                    _stoppingPositionConverter.ToTransferLocation(stageId, false),
                    (byte)slotId);
            }

            return args;
        }

        /// <summary>
        /// Extract all possible information from the given <paramref name="waferId" />.
        /// </summary>
        /// <param name="waferId">The received wafer ID as a single 6-characters string.</param>
        /// <param name="undefined">
        /// <value>true</value>
        /// if the <paramref name="waferId" /> is "999999".
        /// <value>false</value>
        /// otherwise.
        /// </param>
        /// <param name="presence">
        /// <value>false</value>
        /// if the <paramref name="waferId" /> is "000000".
        /// <value>true</value>
        /// otherwise.
        /// </param>
        /// <param name="stageId">
        /// The previous stage ID of the wafer present on arm. Value is valid only when
        /// <paramref name="presence" /> is
        /// <value>true</value>
        /// .
        /// </param>
        /// <param name="slotId">
        /// The previous slot ID of the wafer present on arm. Value is valid only when
        /// <paramref name="presence" /> is
        /// <value>true</value>
        /// .
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// if the provided <paramref name="waferId" /> has not the expected length.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// if the provided <paramref name="waferId" /> has not a valid content (must be convertible into a
        /// uint).
        /// </exception>
        private static void Parser(
            string waferId,
            out bool undefined,
            out bool presence,
            out uint stageId,
            out uint slotId)
        {
            if (waferId.Length != 6)
            {
                throw new InvalidOperationException(
                    $"The provided {nameof(waferId)} must have 6 characters. Value = {waferId}");
            }

            undefined = waferId.Equals("999999");
            presence = !waferId.Equals("000000");

            if (!uint.TryParse(waferId.Substring(0, 3), out stageId))
            {
                throw new InvalidOperationException(
                    $"The provided {nameof(waferId)} first 3 characters must be a uint. Value = {waferId.Substring(0, 3)}");
            }

            if (!uint.TryParse(waferId.Substring(3, 3), out slotId))
            {
                throw new InvalidOperationException(
                    $"The provided {nameof(waferId)} last 3 characters must be a uint. Value = {waferId.Substring(3, 3)}");
            }
        }

        #endregion Private Methods
    }
}
