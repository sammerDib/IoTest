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
    /// Class responsible to handle the command retrieving Substrate Id reader recipes, defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.4.2 oGREC
    /// </summary>
    public class GetOcrRecipesCommand : BaseCommand
    {
        #region Constructors

        public static GetOcrRecipesCommand NewEvent(
            RecipeModel model,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = new List<string[]>
            {
                new[]
                {
                    ((int)Constants.Stage.Aligner).ToString(),
                    model.Number.ToString("D2"),
                    ((int)(model.IsFrame ? SubstrateTypeGREC.Frame : SubstrateTypeGREC.NormalWafer)).ToString(),
                    model.IsStored ? model.Name : "Not Stored"
                }
            };
            var message = new Message(Constants.CommandType.Event, Constants.Commands.GetOcrRecipes, parameters);

            return new GetOcrRecipesCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        public GetOcrRecipesCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.GetOcrRecipes, sender, eqFacade, logger, equipmentManager)
        {
        }

        private GetOcrRecipesCommand(
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

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            // Check number of parameters (sample frame: oGREC:6_2 -> 1 parameter, 2 arguments)
            if (message.CommandParameters.Count <= 0)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            if (message.CommandParameters.Count > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check number of parameter's arguments
            if (message.CommandParameters[0].Length != 2)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first argument (aligner number)
            if (message.CommandParameters[0][0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (!EnumHelpers.TryParseEnumValue(message.CommandParameters[0][0], out Constants.Stage stage)
                || stage != Constants.Stage.Aligner)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check second argument (side filter)
            if (message.CommandParameters[0][1].Length != 1
                || !EnumHelpers.TryParseEnumValue(message.CommandParameters[0][1], out SubstrateSide substrateSide))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check that requested reader(s) exists
            var readers = new List<SubstrateIdReader>();
            switch (substrateSide)
            {
                case SubstrateSide.Front:
                    if (EquipmentManager.SubstrateIdReaderFront == null)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    readers.Add(EquipmentManager.SubstrateIdReaderFront);
                    break;

                case SubstrateSide.Back:
                    if (EquipmentManager.SubstrateIdReaderBack == null)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    readers.Add(EquipmentManager.SubstrateIdReaderBack);
                    break;

                case SubstrateSide.Both:
                    if (EquipmentManager.SubstrateIdReaderFront == null
                        || EquipmentManager.SubstrateIdReaderBack == null)
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                        return true;
                    }

                    readers.Add(EquipmentManager.SubstrateIdReaderFront);
                    readers.Add(EquipmentManager.SubstrateIdReaderBack);
                    break;

                default:
                    SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                    return true;
            }

            // Check if command can be executed
            if (!readers.All(r => r.CanExecute(
                nameof(ISubstrateIdReader.RequestRecipes),
                out CommandContext _)))
            {
                // Maybe not the correct error, but idk how to simulate this case with Rorze.exe
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            Task.Factory.StartNew(delegate
                {
                    readers.ForEach(reader =>
                    {
                        reader.RequestRecipes();
                        reader.Recipes.ForEach(recipe => NewEvent(recipe, commandSender, facade, Logger, EquipmentManager).SendCommand());
                    });
                })
                .ContinueWith(antecedent => SendCommandResult(antecedent));

            return true;
        }

        protected override Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            return Constants.Errors[ErrorCode.AlignerError];
        }

        #endregion BaseCommand
    }
}
