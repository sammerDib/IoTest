using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the read wafer id only command
    /// 
    /// </summary>
    public class ReadWaferIdOnlyCommand : AlignerCommand
    {
        #region Fields

        private SubstrateSide _substrateSide;
        private SubstrateTypeRDID _type;
        private string _frontSideRecipeName;
        private string _backSideRecipeName;

        #endregion Fields

        #region Constructors

        public ReadWaferIdOnlyCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.ReadWaferIdOnly, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion Constructors

        #region BaseCommand

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            // Check number of parameters
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length < 2
                || message.CommandParameters[0].Length > 5)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter validity
            if (!int.TryParse(message.CommandParameters[0][0], out int aligner)
                || message.CommandParameters[0][0].Length > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (aligner != Constants.Aligner)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check second parameter.
            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][1], out _substrateSide))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check that requested reader(s) exists
            bool result;
            switch (_substrateSide)
            {
                case SubstrateSide.Front:
                    if (message.CommandParameters[0].Length != 4)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                        return true;
                    }

                    _frontSideRecipeName = message.CommandParameters[0][2];

                    // Check last parameter.
                    if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0].Last(), out _type))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    if (EquipmentManager.SubstrateIdReaderFront == null)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    result = EquipmentManager.SubstrateIdReaderFront.CanExecute(
                        nameof(ISubstrateIdReader.Read),
                        out CommandContext _,
                        _frontSideRecipeName);
                    break;

                case SubstrateSide.Both:
                    if (message.CommandParameters[0].Length != 5)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                        return true;
                    }

                    _frontSideRecipeName = message.CommandParameters[0][2];
                    _backSideRecipeName  = message.CommandParameters[0][3];

                    // Check last parameter.
                    if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0].Last(), out _type))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    if (EquipmentManager.SubstrateIdReaderFront == null
                        || EquipmentManager.SubstrateIdReaderBack == null)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    result = EquipmentManager.SubstrateIdReaderFront.CanExecute(
                        nameof(ISubstrateIdReader.Read),
                        out _,
                        _frontSideRecipeName);
                    result &= EquipmentManager.SubstrateIdReaderBack.CanExecute(
                        nameof(ISubstrateIdReader.Read),
                        out _,
                        _backSideRecipeName);
                    break;

                case SubstrateSide.Back:
                    if (message.CommandParameters[0].Length != 4)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                        return true;
                    }

                    _backSideRecipeName = message.CommandParameters[0][2];

                    if (EquipmentManager.SubstrateIdReaderBack == null)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    result = EquipmentManager.SubstrateIdReaderBack.CanExecute(
                        nameof(ISubstrateIdReader.Read),
                        out _,
                        _backSideRecipeName);
                    break;

                default:
                    SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                    return true;
            }

            // Check if command can be executed
            if (!result)
            {
                // Maybe not the correct error, but idk how to simulate this case with Rorze.exe
                SendCancellation(message, Constants.Errors[ErrorCode.RfidReadFailed]);
                return true;
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            Task.Factory.StartNew(delegate
                {
                    switch (_substrateSide)
                    {
                        case SubstrateSide.Front:
                            EquipmentManager.SubstrateIdReaderFront.Read(_frontSideRecipeName);
                            break;

                        case SubstrateSide.Back:
                            EquipmentManager.SubstrateIdReaderBack.Read(_backSideRecipeName);
                            break;

                        case SubstrateSide.Both:
                            EquipmentManager.SubstrateIdReaderFront.Read(_frontSideRecipeName);
                            EquipmentManager.SubstrateIdReaderBack.Read(_backSideRecipeName);
                            break;
                    }
                })
                .ContinueWith(antecedent => SendCommandResult(antecedent));

            return true;
        }

        protected override Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            return Constants.Errors[ErrorCode.RfidReadFailed];
        }

        protected override List<string> GetResultArguments(Error error = null)
        {
            // Build command parameter arguments
            var parameterArgs = new List<string>
            {
                // add ID
                Constants.Aligner.ToString(),

                // Add result
                ((int)(error == null
                    ? Constants.EventResult.Success
                    : Constants.EventResult.Error)).ToString()
            };

            if (error == null)
            {
                switch (_substrateSide)
                {
                    case SubstrateSide.Front:
                        parameterArgs.Add(EquipmentManager.SubstrateIdReaderFront.SubstrateId);
                        break;

                    case SubstrateSide.Back:
                        parameterArgs.Add(EquipmentManager.SubstrateIdReaderBack.SubstrateId);
                        break;

                    case SubstrateSide.Both:
                        parameterArgs.Add(EquipmentManager.SubstrateIdReaderFront.SubstrateId);
                        parameterArgs.Add(EquipmentManager.SubstrateIdReaderBack.SubstrateId);
                        break;

                    case SubstrateSide.Frame: // TODO ???
                        break;
                }

                return parameterArgs;
            }

            // Add error level (if defined)
            if (!string.IsNullOrWhiteSpace(error.Type))
            {
                parameterArgs.Add(error.Type);
            }

            // Add error code (if defined)
            if (!string.IsNullOrWhiteSpace(error.Code))
            {
                parameterArgs.Add(error.Code);
            }

            // Add error message (if defined)
            if (!string.IsNullOrWhiteSpace(error.Description))
            {
                parameterArgs.Add(error.Description);
            }

            return parameterArgs;
        }

        #endregion BaseCommand
    }
}
