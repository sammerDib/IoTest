using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get carrier type command defined in
    /// Feature carrier type selection for a TMAP.pdf oGCTY
    /// </summary>
    public class GetCarrierTypeCommand : LoadPortCommand
    {
        #region Constructors

        public static GetCarrierTypeCommand NewEvent(
            Constants.Port loadPortId,
            uint carrierType,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var message = new Message(
                Constants.CommandType.Event,
                Constants.Commands.GetCarrierType,
                new List<string[]> { new[]
                {
                    ((int)loadPortId).ToString(),
                    "1",
                    carrierType.ToString()
                } });

            return new GetCarrierTypeCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        public GetCarrierTypeCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(
                Constants.Commands.GetCarrierType,
                loadPortId,
                sender,
                eqFacade,
                logger,
                equipmentManager)
        {
        }

        public GetCarrierTypeCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion

        #region LoadPortCommand

        protected override bool TreatOrder(Message message)
        {
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
            if (LoadPort.Carrier == null || LoadPort.CarrierPresence != CassettePresence.Correctly )
            {
                SendCancellation(message, Constants.Errors[ErrorCode.CarrierNotPresent]);
                return true;
            }

            SendAcknowledge(message);

            Task.Run(
                () =>
                {
                    NewEvent(
                            LoadPortId,
                            LoadPort.CarrierTypeNumber,
                            commandSender,
                            facade,
                            Logger,
                            EquipmentManager)
                        .SendCommand();
                });

            return true;
        }

        #endregion
    }
}
