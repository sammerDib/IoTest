using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;
using AlignerMessages       = UnitySC.Equipment.Abstractions.Devices.Aligner.Resources.Messages;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    public abstract class AlignerCommand : BaseCommand
    {
        #region Constructors

        protected AlignerCommand(
            string commandName,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(commandName, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected AlignerCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion Constructors

        #region BaseCommand

        protected override Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            return Constants.Errors[ErrorCode.AlignerError];
        }

        #endregion BaseCommand

        #region Check Preconditions

        protected bool CancelIfIsIdleFailed(Message message, string error)
        {
            if (error.Contains(GenericDeviceMessages.NotIdle))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AlignerMoving]);
                return true;
            }

            return false;
        }

        protected bool CancelIfIsAngleValidFailed(Message message, string error)
        {
            if (error.Contains(AlignerMessages.ArgumentNotAnAngle)
                || error.Contains(AlignerMessages.AngleNotInRange))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            return false;
        }

        #endregion Check Preconditions
    }
}
