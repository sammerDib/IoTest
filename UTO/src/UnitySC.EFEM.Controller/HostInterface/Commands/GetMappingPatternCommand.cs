using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get mapping command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.3.4 oGMAP
    /// </summary>
    public class GetMappingPatternCommand : LoadPortCommand
    {
        #region Constructors

        /// <summary>
        /// Transmit mapping pattern data to the HOST.
        /// </summary>
        /// <param name="loadPortId">The id of the load port from which the mapping has been done.</param>
        /// <param name="slotStates"></param>
        /// <param name="sender"></param>
        /// <param name="eqFacade"></param>
        /// <param name="logger"></param>
        /// <param name="equipmentManager"></param>
        public static GetMappingPatternCommand NewEvent(
            Constants.Port loadPortId,
            IEnumerable<SlotState> slotStates,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var mapAsString = slotStates.Aggregate(string.Empty, (current, slotState) => current + ((int)slotState));
            var parameters = new List<string>
            {
                ((int)loadPortId).ToString(),
                mapAsString
            };
            var message = new Message(
                Constants.CommandType.Event,
                Constants.Commands.GetMappingPattern,
                new List<string[]> { parameters.ToArray() });
            return new GetMappingPatternCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        public GetMappingPatternCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.GetMappingPattern, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
        }

        private GetMappingPatternCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion Constructors

        #region LoadPortCommand

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            if (base.TreatOrder(message))
            {
                // Message has been treated if treated by base and coming from expected LP
                return !WrongLp;
            }

            // Check number of parameters
            if (message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check carrier state
            if (LoadPort.Carrier == null || LoadPort.PhysicalState != LoadPortState.Open)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.CarrierNotLoad]);
                return true;
            }

            // Send acknowledge response with results
            List<string[]> parameters = CreateCommandParameters(LoadPortId, LoadPort.Carrier.MappingTable);
            SendAcknowledge(message, parameters);
            return true;
        }

        #endregion LoadPortCommand

        #region Other Methods

        protected void SendAcknowledge(Message receivedMessage, List<string[]> commandResult)
        {
            SendAcknowledge(new Message(Constants.CommandType.Ack, receivedMessage.CommandName, commandResult));
        }

        private static List<string[]> CreateCommandParameters(Constants.Port port, IEnumerable<SlotState> slotMap)
        {
            var parameters = new List<string[]>(1)
            {
                new string[2], // Port_SlotMap
            };

            var mapAsString = slotMap.Aggregate(string.Empty, (current, slotState) => current + ((int)slotState));

            parameters[0][0] = ((int)port).ToString();
            parameters[0][1] = mapAsString;

            return parameters;
        }

        #endregion Other Methods
    }
}
