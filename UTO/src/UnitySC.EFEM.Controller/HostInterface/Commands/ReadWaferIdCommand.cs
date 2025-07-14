using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the read wafer id command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.4.3 oRDID
    /// </summary>
    public class ReadWaferIdCommand : AlignerCommand
    {
        #region Fields

        private SubstrateSide _substrateSide;
        private SubstrateTypeRDID _type;
        private string _frontSideRecipeName;
        private string _backSideRecipeName;
        private readonly HostDriver _hostDriver;

        #endregion Fields

        #region Constructors

        public ReadWaferIdCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager,
            HostDriver hostDriver)
            : base(Constants.Commands.ReadWaferId, sender, eqFacade, logger, equipmentManager)
        {
            _hostDriver = hostDriver;
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
                    if (message.CommandParameters[0].Length != 4
                        && message.CommandParameters[0].Length != 2)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                        return true;
                    }

                    _frontSideRecipeName = message.CommandParameters[0].Length == 4
                        ? message.CommandParameters[0][2]
                        : "T7";

                    // Check last parameter.
                    if (message.CommandParameters[0].Length == 4)
                    {
                        if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0].Last(), out _type))
                        {
                            SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                            return true;
                        }
                    }
                    else
                    {
                        _type = SubstrateTypeRDID.NormalWafer;
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

                    EquipmentManager.SubstrateIdReaderFront.RequestRecipes();

                    if (!EquipmentManager.SubstrateIdReaderFront.Recipes.Exists(
                            r => r.Name == _frontSideRecipeName))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.OcrRecipeNotSet]);
                        return true;
                    }

                    // Check if aligner is ready to align before reading the ID
                    if (!EquipmentManager.Aligner.CanExecute(
                            nameof(IAligner.Align),
                            out _,
                            Angle.FromDegrees(EquipmentManager.SubstrateIdReaderFront.Recipes.First(r => r.Name == _frontSideRecipeName).Angle),
                            AlignType.AlignWaferWithoutCheckingSubO_FlatLocation))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AlignerError]);
                        return true;
                    }
                    break;

                case SubstrateSide.Both:
                    if (message.CommandParameters[0].Length != 5)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                        return true;
                    }

                    _frontSideRecipeName = message.CommandParameters[0][2];
                    _backSideRecipeName = message.CommandParameters[0][3];

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

                    EquipmentManager.SubstrateIdReaderFront.RequestRecipes();
                    EquipmentManager.SubstrateIdReaderBack.RequestRecipes();

                    if (!EquipmentManager.SubstrateIdReaderFront.Recipes.Exists(
                            r => r.Name == _frontSideRecipeName))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.OcrRecipeNotSet]);
                        return true;
                    }

                    if (!EquipmentManager.SubstrateIdReaderBack.Recipes.Exists(
                            r => r.Name == _backSideRecipeName))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.OcrRecipeNotSet]);
                        return true;
                    }

                    // Check if aligner is ready to align before reading the ID
                    if (!EquipmentManager.Aligner.CanExecute(
                            nameof(IAligner.Align),
                            out _,
                            Angle.FromDegrees(EquipmentManager.SubstrateIdReaderFront.Recipes.First(r => r.Name == _frontSideRecipeName).Angle),
                            AlignType.AlignWaferWithoutCheckingSubO_FlatLocation))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AlignerError]);
                        return true;
                    }

                    if (!EquipmentManager.Aligner.CanExecute(
                            nameof(IAligner.Align),
                            out _,
                            Angle.FromDegrees(EquipmentManager.SubstrateIdReaderBack.Recipes.First(r => r.Name == _backSideRecipeName).Angle),
                            AlignType.AlignWaferWithoutCheckingSubO_FlatLocation))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AlignerError]);
                        return true;
                    }
                    break;

                case SubstrateSide.Back:
                    if (message.CommandParameters[0].Length != 4
                        && message.CommandParameters[0].Length != 2)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                        return true;
                    }

                    _backSideRecipeName = message.CommandParameters[0].Length == 4
                        ? message.CommandParameters[0][2]
                        : "T7";

                    // Check last parameter.
                    if (message.CommandParameters[0].Length == 4)
                    {
                        if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0].Last(), out _type))
                        {
                            SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                            return true;
                        }
                    }
                    else
                    {
                        _type = SubstrateTypeRDID.NormalWafer;
                    }

                    if (EquipmentManager.SubstrateIdReaderBack == null)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    result = EquipmentManager.SubstrateIdReaderBack.CanExecute(
                        nameof(ISubstrateIdReader.Read),
                        out _,
                        _backSideRecipeName);

                    EquipmentManager.SubstrateIdReaderBack.RequestRecipes();

                    if (!EquipmentManager.SubstrateIdReaderBack.Recipes.Exists(
                            r => r.Name == _backSideRecipeName))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.OcrRecipeNotSet]);
                        return true;
                    }

                    // Check if aligner is ready to align before reading the ID
                    if (!EquipmentManager.Aligner.CanExecute(
                            nameof(IAligner.Align),
                            out _,
                            Angle.FromDegrees(EquipmentManager.SubstrateIdReaderBack.Recipes.First(r => r.Name == _backSideRecipeName).Angle),
                            AlignType.AlignWaferWithoutCheckingSubO_FlatLocation))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AlignerError]);
                        return true;
                    }
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
                            ReadWaferId(EquipmentManager.SubstrateIdReaderFront, _frontSideRecipeName);
                            break;

                        case SubstrateSide.Back:
                            ReadWaferId(EquipmentManager.SubstrateIdReaderBack, _backSideRecipeName);
                            break;

                        case SubstrateSide.Both:
                            ReadWaferId(
                                EquipmentManager.SubstrateIdReaderFront,
                                _frontSideRecipeName,
                                EquipmentManager.SubstrateIdReaderBack,
                                _backSideRecipeName);

                            var frontAngle = EquipmentManager.SubstrateIdReaderFront.Recipes
                                .First(r => r.Name == _frontSideRecipeName)
                                .Angle;
                            var backAngle = EquipmentManager.SubstrateIdReaderBack.Recipes
                                .First(r => r.Name == _backSideRecipeName)
                                .Angle;

                            EquipmentManager.Aligner.Align(
                                Angle.FromDegrees(frontAngle),
                                AlignType.AlignWaferWithoutCheckingSubO_FlatLocation);
                            EquipmentManager.SubstrateIdReaderFront.Read(_frontSideRecipeName);

                            //Align before reading with back reader only if angles are different
                            if (frontAngle != backAngle)
                            {
                                EquipmentManager.Aligner.Align(
                                    Angle.FromDegrees(backAngle),
                                    AlignType.AlignWaferWithoutCheckingSubO_FlatLocation);
                            }
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

        #region Private Methods

        private void ReadWaferId(SubstrateIdReader reader, string recipeName, SubstrateIdReader secondReader = null, string secondRecipeName = "")
        {
            var waferSize = EquipmentManager.Aligner.GetMaterialDimension();
            var angle = Angle.FromDegrees(reader.Recipes.First(r => r.Name == recipeName).Angle);
            var secondAngle = Angle.MaxValue;
            if (secondReader != null)
            {
                secondAngle = Angle.FromDegrees(secondReader.Recipes.First(r => r.Name == secondRecipeName).Angle);
            }

            EquipmentManager.Aligner.Align(
                angle,
                AlignType.AlignWaferWithoutCheckingSubO_FlatLocation);

            if (waferSize == SampleDimension.S150mm)
            {
                //In case of 6" wafers :


                lock (_hostDriver.LockRobotCommand)
                {

                    //First select robot arm to use
                    var robotArm = RobotArm.Arm1;
                    if (EquipmentManager.Robot.UpperArmLocation.Wafer != null)
                    {
                        robotArm = RobotArm.Arm2;
                    }

                    //Unsubscribe events so we do not send event to the host
                    _hostDriver.IsRobotSequenceForOcrReadingInProgress = true;
                    _hostDriver.UnsubscribeToEquipmentEvents();

                    try
                    {

                        //Then prepare aligner for pick
                        var pickAlignerTasks = new List<Task>();
                        pickAlignerTasks.Add(
                            EquipmentManager.Robot.GoToSpecifiedLocationAsync(
                                EquipmentManager.Aligner,
                                1,
                                robotArm,
                                true));
                        pickAlignerTasks.Add(
                            EquipmentManager.Aligner.PrepareTransferAsync(
                                robotArm == RobotArm.Arm1
                                    ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                                    : EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                                EquipmentManager.Aligner.GetMaterialDimension(),
                                EquipmentManager.Aligner.GetMaterialType()));
                        Task.WaitAll(pickAlignerTasks.ToArray());

                        //Then pick the wafer in the aligner with robot selected arm
                        EquipmentManager.Robot.Pick(robotArm, EquipmentManager.Aligner, 1);

                        //Then move down the aligner lift pins
                        EquipmentManager.Aligner.MoveZAxis(true);

                        //Then go to the substrate id reader position
                        EquipmentManager.Robot.ExtendArm(robotArm, TransferLocation.PreAlignerD, 1);
                    }
                    catch
                    {
                        //Subscribe to events to send substrate id information to the host
                        _hostDriver.IsRobotSequenceForOcrReadingInProgress = false;
                        _hostDriver.SubscribeToEquipmentEvents();
                        throw;
                    }

                    //Subscribe to events to send substrate id information to the host
                    _hostDriver.SubscribeToEquipmentEvents();

                    try
                    {
                        //Read wafer id
                        reader.Read(recipeName);
                        if (secondReader != null && angle == secondAngle)
                        {
                            secondReader.Read(secondRecipeName);
                        }
                    }
                    catch
                    {
                        _hostDriver.IsRobotSequenceForOcrReadingInProgress = false;
                        throw;
                    }

                    //Unsubscribe events so we do not send event to the host
                    _hostDriver.UnsubscribeToEquipmentEvents();

                    try
                    {
                        //Then prepare aligner for place
                        EquipmentManager.Robot.GoToSpecifiedLocation(
                            EquipmentManager.Aligner,
                            1,
                            robotArm,
                            false);

                        EquipmentManager.Aligner.PrepareTransfer(
                            robotArm == RobotArm.Arm1
                                ? EquipmentManager.Robot.Configuration.UpperArm.EffectorType
                                : EquipmentManager.Robot.Configuration.LowerArm.EffectorType,
                            robotArm == RobotArm.Arm1
                                ? EquipmentManager.Robot.UpperArmLocation.Wafer.MaterialDimension
                                : EquipmentManager.Robot.LowerArmLocation.Wafer.MaterialDimension,
                            robotArm == RobotArm.Arm1
                                ? EquipmentManager.Robot.UpperArmLocation.Wafer.MaterialType
                                : EquipmentManager.Robot.LowerArmLocation.Wafer.MaterialType);

                        //Then place the wafer in the aligner with robot upper arm
                        EquipmentManager.Robot.Place(robotArm, EquipmentManager.Aligner, 1);
                    }
                    catch
                    {
                        //Subscribe to events to send substrate id information to the host
                        _hostDriver.IsRobotSequenceForOcrReadingInProgress = false;
                        _hostDriver.SubscribeToEquipmentEvents();
                        throw;
                    }

                    //Subscribe to events to send substrate id information to the host
                    _hostDriver.IsRobotSequenceForOcrReadingInProgress = false;
                    _hostDriver.SubscribeToEquipmentEvents();
                }
                
            }
            else
            {
                //Read wafer id
                reader.Read(recipeName);
                if (secondReader != null && angle == secondAngle)
                {
                    secondReader.Read(secondRecipeName);
                }
            }

            if (secondReader != null && angle != secondAngle)
            {
                ReadWaferId(secondReader, secondRecipeName);
            }
        }


        #endregion
    }
}
