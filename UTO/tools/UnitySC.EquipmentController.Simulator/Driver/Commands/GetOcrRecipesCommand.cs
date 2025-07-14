using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EquipmentController.Simulator.Driver.EventArgs;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetOcrRecipesCommand : RorzeCommand
    {
        #region Fields

        private readonly SortedList<int, string> _ocrRecipesBack = new SortedList<int, string>(30);
        private readonly SortedList<int, string> _ocrRecipesFront = new SortedList<int, string>(30);
        private readonly SubstrateSide _requestedSide;

        #endregion Fields

        #region Constructors

        public static GetOcrRecipesCommand NewOrder(
            SubstrateSide side,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>
            {
                new[] { ((int)Constants.Stage.Aligner).ToString(), ((int)side).ToString() }
            };

            return new GetOcrRecipesCommand(side, parameters, sender, eqFacade, logger);
        }

        private GetOcrRecipesCommand(
            SubstrateSide side,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(Constants.CommandType.Order, Constants.Commands.GetOcrRecipes, parameters, sender, eqFacade, logger)
        {
            _requestedSide = side;
        }

        #endregion Constructors

        #region BaseCommand overrides

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least one argument
            // Data received: eGREC:<Aligner>_<OCR Recipe No.>_<Wafer Type>_<OCR Recipe Name>
            // Success: eGREC:1
            // Error:   eGREC:2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length < 1)
            {
                return false;
            }

            // Store received data until end of command
            if (message.CommandParameters[0].Length == 4)
            {
                if (!int.TryParse(message.CommandParameters[0][1], out int recipeNumber))
                {
                    return false;
                }

                switch (_requestedSide)
                {
                    case SubstrateSide.Front:
                        _ocrRecipesFront.Add(recipeNumber, message.CommandParameters[0][3]);
                        break;

                    case SubstrateSide.Back:
                        _ocrRecipesBack.Add(recipeNumber, message.CommandParameters[0][3]);
                        break;

                    case SubstrateSide.Both:
                        // In this case it seems Rorze.exe is first sending all front recipes then all back recipes
                        if (_ocrRecipesFront.Count < _ocrRecipesFront.Capacity)
                        {
                            _ocrRecipesFront.Add(recipeNumber, message.CommandParameters[0][3]);
                        }
                        else
                        {
                            _ocrRecipesBack.Add(recipeNumber, message.CommandParameters[0][3]);
                        }

                        break;

                    default:
                        return false;
                }

                return true;
            }

            // Parse argument to get command status
            if (!int.TryParse(message.CommandParameters[0][0], out int result))
            {
                return false;
            }

            switch (result)
            {
                case (int)Constants.EventResult.Success:
                    facade.SendEquipmentEvent(
                        (int)EventsToFacade.OcrRecipesReceived,
                        new OcrRecipesReceivedEventArgs(_requestedSide, _ocrRecipesFront, _ocrRecipesBack));
                    break;

                case (int)Constants.EventResult.Error:
                    // TODO Notify driver about command failure
                    break;

                // Unexpected result
                default:
                    return false;
            }

            CommandComplete();
            return true;
        }

        #endregion BaseCommand overrides
    }
}
