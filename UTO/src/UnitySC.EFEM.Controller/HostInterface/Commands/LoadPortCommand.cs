using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

using LoadPortMessages      = UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources.Messages;
using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    public abstract class LoadPortCommand : BaseCommand
    {
        #region Constructors

        protected LoadPortCommand(
            string commandName,
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(commandName, sender, eqFacade, logger, equipmentManager)
        {
            LoadPortId = loadPortId;
            LoadPort   = EquipmentManager.LoadPorts[(int)loadPortId];
        }

        protected LoadPortCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion Constructors

        #region Properties

        public Constants.Port LoadPortId { get; }

        public LoadPort LoadPort { get; }

        public bool WrongLp { get; private set; }

        #endregion Properties

        #region BaseCommand

        protected override bool TreatOrder(Message message)
        {
            // Check parameters to determine if received order applies to the LoadPort of this command
            // Expected message format is: o<CommandName>:<Port>_<arguments depending on command>
            WrongLp = false;

            // Check number of parameters
            if (message.CommandParameters.Count != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter validity
            if (message.CommandParameters[0][0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][0], out Constants.Port port))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check if order applies to this command's load port
            if (port != LoadPortId)
            {
                WrongLp = true;
                return true;
            }

            return false;
        }

        protected override Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            return Constants.Errors[ErrorCode.LoadPortError];
        }

        #endregion BaseCommand

        #region Check Preconditions

        internal bool CancelIfIsInServiceFailed(Message message, string error)
        {
            if (error.Contains(LoadPortMessages.LoadPortOutOfService))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.LoadPortDisable]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsIdleFailed(Message message, string error)
        {
            if (error.Contains(GenericDeviceMessages.NotIdle))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.LoadPortMoving]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsMappingSupportedFailed(Message message, string error)
        {
            if (error.Contains(LoadPortMessages.MappingNotSupported))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.LoadPortError]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsCarrierCorrectlyPlacedFailed(Message message, string error)
        {
            if (error.Contains(LoadPortMessages.CarrierNotCorrectlyPlaced))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.CarrierNotPresent]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsCarrierIdSupportedFailed(Message message, string error)
        {
            if (error.Contains(LoadPortMessages.CarrierIdNotSupported))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.RfidReadFailed]); // TODO verify with rorze.exe
                return true;
            }

            return false;
        }

        #endregion Check Preconditions
    }
}
