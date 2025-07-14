using System;
using System.Globalization;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Enums;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;
using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;
using RobotMessages         = UnitySC.Equipment.Abstractions.Devices.Robot.Resources.Messages;
using AlignerMessages       = UnitySC.Equipment.Abstractions.Devices.Aligner.Resources.Messages;
using LoadPortMessages      = UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources.Messages;
using ProcessModuleMessages = UnitySC.Equipment.Abstractions.Devices.ProcessModule.Resources.Messages;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    public abstract class RobotCommand : BaseCommand
    {
        protected RobotCommand(
            string commandName,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(commandName, sender, eqFacade, logger, equipmentManager)
        {
        }

        #region BaseCommand

        protected override Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            return Constants.Errors[ErrorCode.RobotError];
        }

        #endregion BaseCommand

        #region Check Preconditions

        internal bool CancelIfIsArmEffectorValidFailed(
            Message message,
            string error,
            RobotArm arm,
            EffectorType effectorType,
            SampleDimension size)
        {
            if (error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.ArmIsInvalid, arm.ToString()))
                || error.Contains(RobotMessages.ArgumentNotAnArm)
                || error.Contains(RobotMessages.ArgumentNotAnIExtendedMaterialLocationContainer)
                || error.Contains(RobotMessages.ArgumentNotASlot))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            if (error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.EffectorNotSupported, effectorType))
                || error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.SizeNotSupported, size, effectorType)))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.ArmStateError]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsArmEnabledFailed(Message message, string error, RobotArm arm)
        {
            if (error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.ArmIsInvalid, arm.ToString()))
                || error.Contains(RobotMessages.ArgumentNotAnArm))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            if (error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.ArmIsDisabled, arm.ToString())))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.ArmStateError]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsArmReadyFailed(Message message, string error, RobotArm arm, bool isPick)
        {
            if (error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.ArmIsInvalid, arm.ToString()))
                || error.Contains(RobotMessages.ArgumentNotAnArm))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            if (isPick)
            {
                if (error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.ArmAlreadyHaveMaterial, arm.ToString())))
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.ArmStateError]);
                    return true;
                }
            }
            else
            {
                if (error.Contains(string.Format(CultureInfo.InvariantCulture, RobotMessages.ArmHaveNoMaterial, arm.ToString())))
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.ArmStateError]);
                    return true;
                }
            }

            return false;
        }

        internal bool CancelIfIsSpeedValidFailed(Message message, string error)
        {
            if (error.Contains(RobotMessages.ArgumentNotARatio)
                || error.Contains(RobotMessages.SpeedNotInRange))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsLocationReadyFailed(
            Message message,
            string error,
            byte slot)
        {
            if (error.Contains(LoadPortMessages.CarrierNotCorrectlyPlaced)
                || error.Contains(LoadPortMessages.CarrierNotDocked)
                || error.Contains(LoadPortMessages.CarrierNotMapped)
                || error.Contains(LoadPortMessages.DoorNotOpened))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.CarrierNotLoad]);
                return true;
            }

            if (error.Contains(AlignerMessages.AlignerHaveOnlyOneSlot)
                || error.Contains(ProcessModuleMessages.ProcessModuleHaveOnlyOneSlot)
                || error.Contains(LoadPortMessages.CarrierHaveOnlyOneSlot)
                || error.Contains(string.Format(
                    CultureInfo.InvariantCulture,
                    // We don't know the range, so we remove the second argument from format string
                    LoadPortMessages.SlotNotInRange.Substring(
                        0,
                        LoadPortMessages.SlotNotInRange.IndexOf("{1}", StringComparison.Ordinal)),
                    slot))
                || error.Contains(string.Format(
                    CultureInfo.InvariantCulture,
                    // We don't know the state, so we remove the second argument from format string
                    LoadPortMessages.SlotHasInvalidState.Substring(
                        0,
                        LoadPortMessages.SlotHasInvalidState.IndexOf("{1}", StringComparison.Ordinal)),
                    slot)))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.WaferStateError]);
                return true;
            }

            if (error.Contains(AlignerMessages.AlignerAlreadyHaveMaterial)
                || error.Contains(AlignerMessages.AlignerHaveNoMaterial)
                || error.Contains(LoadPortMessages.MaterialNotAllowed.Substring(4))
                || error.Contains(LoadPortMessages.SubstrateSizeNotAllowed.Substring(4))
                || error.Contains(string.Format(CultureInfo.InvariantCulture, LoadPortMessages.SlotAlreadyHaveMaterial, slot))
                || error.Contains(string.Format(CultureInfo.InvariantCulture, LoadPortMessages.SlotHaveNoMaterial, slot)))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.ArmStateError]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsIdleFailed(Message message, string error)
        {
            if (error.Contains(GenericDeviceMessages.NotIdle))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.RobotError]);
                return true;
            }

            return false;
        }

        internal bool CancelIfIsNotBusyFailed(Message message, string error)
        {
            if (error.Contains(GenericDeviceMessages.AlreadyBusy))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.RobotMoving]);
                return true;
            }

            return false;
        }

        #endregion Check Preconditions
    }
}
