using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the carrier presence event defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.3.12 eLPSR
    /// </summary>
    public class CarrierPresenceCommand : LoadPortCommand
    {
        public static CarrierPresenceCommand NewEvent(
            Constants.Port loadPortId,
            CassettePresence carrierPresence,
            bool opPush,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            bool presence;
            bool placement;

            switch (carrierPresence)
            {
                case CassettePresence.Unknown:
                case CassettePresence.Absent:
                    presence  = false;
                    placement = false;
                    break;

                case CassettePresence.PresentNoPlacement:
                    presence  = true;
                    placement = false;
                    break;

                case CassettePresence.NoPresentPlacement:
                    presence  = false;
                    placement = true;
                    break;

                case CassettePresence.Correctly:
                    presence  = true;
                    placement = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(carrierPresence), carrierPresence, null);
            }

            var parameters = new List<string[]>
            {
                new[]
                {
                    ((int)loadPortId).ToString(),
                    presence ? "1" : "0",
                    placement ? "1" : "0",
                    opPush ? "1" : "0"
                }
            };

            var message = new Message(Constants.CommandType.Event, Constants.Commands.CarrierPresence, parameters);

            return new CarrierPresenceCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        private CarrierPresenceCommand(
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
