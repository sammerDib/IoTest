using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.PostmanCommands
{
    public class ReadCarrierIdCommand : RorzeCommand
    {
        private readonly ILogger _logger;
        private readonly Tuple<int, int> _carrierIdCharacterInterval;

        #region Constructors

        /// <summary>Create a new command for asking the Carrier ID to the Load Port.</summary>
        /// <param name="deviceId">The ID of the Load Port.</param>
        /// <param name="sender">The tcp message sender.</param>
        /// <param name="eqFacade">The driver that will manage command result.</param>
        /// <param name="logger">The logger to be used to trace messages, if necessary.</param>
        /// <param name="pageIntervalForCarrierId">
        /// The start and stop pages on which to find the Carrier ID (or null if HW parameters are to be used).
        /// </param>
        /// <param name="carrierIdCharacterInterval">
        /// The start and stop index for the string read by the load port. Use it when the carrier ID is
        /// integrated in the pages but is not alone inside it/them. When set to null, the command will return
        /// the entirety of the page(s) content. When at least a value is set to "-1", the corresponding index
        /// would not be taken into account.
        /// </param>
        public static ReadCarrierIdCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            Tuple<uint, uint> pageIntervalForCarrierId,
            Tuple<int, int> carrierIdCharacterInterval = null)
        {
            var parameters = new List<string>(2);

            if (pageIntervalForCarrierId == null)
            {
                return new ReadCarrierIdCommand(
                    RorzeConstants.CommandTypeAbb.Order,
                    deviceId,
                    sender,
                    eqFacade,
                    logger,
                    carrierIdCharacterInterval);
            }

            // Check carrier ID start page and end page compatibility
            if (pageIntervalForCarrierId.Item1 > pageIntervalForCarrierId.Item2)
            {
                logger.Error(
                    $"Page interval for reading carrier ID is not valid: {pageIntervalForCarrierId.Item1} > {pageIntervalForCarrierId.Item2}\n"
                    + "Command would use hardware default values instead.");
            }
            else
            {
                parameters.Add(pageIntervalForCarrierId.Item1.ToString()); // Carrier ID start page
                parameters.Add(pageIntervalForCarrierId.Item2.ToString()); // Carrier ID end page
            }

            return new ReadCarrierIdCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                logger,
                carrierIdCharacterInterval,
                parameters.ToArray());
        }

        private ReadCarrierIdCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            Tuple<int, int> carrierIdCharacterInterval,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.LoadPort,
                deviceId,
                RorzeConstants.Commands.ReadCarrierId,
                sender,
                eqFacade,
                commandParameters)
        {
            _logger = logger;
            _carrierIdCharacterInterval = carrierIdCharacterInterval;
        }

        #endregion

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            var eventArgs = new CarrierIdReceivedEventArgs(DeviceNumber);

            if (message.Data.Contains(RorzeConstants.ErrorCodes.CarrierIdReadFail))
            {
                eventArgs.CarrierId = string.Empty;
                eventArgs.IsSucceed = false;
            }
            else
            {
                // Keep all pages content
                if (_carrierIdCharacterInterval == null)
                {
                    eventArgs.CarrierId = message.Data;
                }

                // Keep only pages content after the start index
                else if (_carrierIdCharacterInterval.Item1 != -1
                         && _carrierIdCharacterInterval.Item1 < message.Data.Length
                         && _carrierIdCharacterInterval.Item2 == -1)
                {
                    eventArgs.CarrierId = message.Data.Substring(_carrierIdCharacterInterval.Item1);
                }

                // Keep only pages content before the end index
                else if (_carrierIdCharacterInterval.Item1 == -1
                         && _carrierIdCharacterInterval.Item2 != -1
                         && _carrierIdCharacterInterval.Item2 < message.Data.Length)
                {
                    eventArgs.CarrierId = message.Data.Substring(
                        0,
                        _carrierIdCharacterInterval.Item2);
                }

                // Keep only pages content before the end index
                else if (_carrierIdCharacterInterval.Item1 != -1
                         && _carrierIdCharacterInterval.Item1 < message.Data.Length
                         && _carrierIdCharacterInterval.Item2 != -1
                         && _carrierIdCharacterInterval.Item2 < message.Data.Length
                         && _carrierIdCharacterInterval.Item1 <= _carrierIdCharacterInterval.Item2)
                {
                    eventArgs.CarrierId = message.Data.Substring(
                        _carrierIdCharacterInterval.Item1,
                        _carrierIdCharacterInterval.Item2 - _carrierIdCharacterInterval.Item1 + 1);
                }

                // Error cases (OR):
                // - Carrier ID start index > Carrier ID stop index
                // - Carrier ID start index > Pages content length
                // - Carrier ID stop index  > Pages content length
                else
                {
                    _logger.Error(
                        "Could not extract carrier ID from carrier tag pages.\n"
                        + "Command would end with the entire pages content.\n"
                        + $"Carrier ID start index = {_carrierIdCharacterInterval.Item1}\n"
                        + $"Carrier ID stop index  = {_carrierIdCharacterInterval.Item2}\n"
                        + $"Pages content length   = {message.Data.Length}\n"
                        + $"Pages content = {message.Data}");
                    eventArgs.CarrierId = message.Data;
                }

                eventArgs.CarrierId = eventArgs.CarrierId.Trim();
                eventArgs.IsSucceed = !string.IsNullOrEmpty(eventArgs.CarrierId);
            }

            facade.SendEquipmentEvent((int)EFEMEvents.ReadCarrierIdCompleted, eventArgs);
            CommandComplete();
            return true;
        }

        protected override bool TreatCancel(RorzeMessage message)
        {
            var eventArgs = new CarrierIdReceivedEventArgs(DeviceNumber);

            if (message.Data.Contains(RorzeConstants.ErrorCodes.CarrierIdReadFail))
            {
                eventArgs.CarrierId = "************";
                eventArgs.IsSucceed = false;
                facade.SendEquipmentEvent((int)EFEMEvents.ReadCarrierIdCompleted, eventArgs);
                CommandComplete();
                return true;
            }

            return base.TreatCancel(message);
        }

        #endregion RorzeCommand
    }
}
