using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitsNet.Units;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get pressure command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.5.8 oGPRS
    /// </summary>
    public class GetPressureCommand : BaseCommand
    {
        public GetPressureCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.GetPressure, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            // Check device owning the data exists
            if (EquipmentManager.Ffu == null)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.FfuAbnormal]);
                return true;
            }

            // Check number of parameters
            if (message.CommandParameters.Count != 0)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // For this command, acknowledge contains the resulting pressure, there is no event
            // Send acknowledge response with result
            message.CommandParameters.Clear();
            message.CommandParameters.Add(new[]
            {
                $"{EquipmentManager.Ffu.DifferentialPressure.As(PressureUnit.Millipascal)} mPa"
            });
            SendAcknowledge(message);
            return true;
        }
    }
}
