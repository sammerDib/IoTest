using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get wafer size in loadport command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.3.10 oLPSZ
    /// </summary>
    public class GetWaferSizeInLoadPortCommand : LoadPortCommand
    {
        public GetWaferSizeInLoadPortCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.GetWaferSizeInLoadPort, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check parameter
            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][0], out Constants.Port port))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            if (!EquipmentManager.LoadPorts.TryGetValue(Constants.ToLoadPortId((Constants.Unit)port), out LoadPort loadPort))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (loadPort.Carrier == null)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.CarrierNotLoad]);
                return true;
            }

            // Send acknowledge response with results
            List<string[]> parameters = CreateCommandParameters(port, loadPort.Carrier.SampleSize);
            SendAcknowledge(message, parameters);

            return true;
        }

        protected void SendAcknowledge(Message receivedMessage, List<string[]> commandResult)
        {
            SendAcknowledge(new Message(Constants.CommandType.Ack, receivedMessage.CommandName, commandResult));
        }

        private static List<string[]> CreateCommandParameters(Constants.Port port, SampleDimension sampleDimension)
        {
            var parameters = new List<string[]>(1)
            {
                new string[3], // prefixe_Port_Wafersize
            };

            parameters[0][0] = "0";
            parameters[0][1] = ((int)port).ToString();
            parameters[0][2] = WaferSizeConverter.ToInch(sampleDimension).ToString();

            return parameters;
        }
    }
}
