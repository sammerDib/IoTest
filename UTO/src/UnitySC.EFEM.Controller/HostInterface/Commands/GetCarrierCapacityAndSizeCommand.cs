using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get carrier capacity and size command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.3.11 oWFMX
    /// </summary>
    public class GetCarrierCapacityAndSizeCommand : LoadPortCommand
    {
        public static GetCarrierCapacityAndSizeCommand NewEvent(
            Constants.Port loadPortId,
            byte carrierCapacity,
            SampleDimension dimension,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = new List<string[]>
            {
                new[]
                {
                    ((int)loadPortId).ToString(),
                    carrierCapacity.ToString(),
                    WaferSizeConverter.ToUint(dimension).ToString()
                }
            };
            var message = new Message(
                Constants.CommandType.Event,
                Constants.Commands.GetCarrierCapacityAndSize,
                parameters);

            return new GetCarrierCapacityAndSizeCommand(
                message,
                sender,
                eqFacade,
                logger,
                equipmentManager);
        }

        public GetCarrierCapacityAndSizeCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }
    }
}
