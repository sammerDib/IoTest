using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.EquipmentController.Simulator.Driver.EventArgs;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    /// <summary>
    /// Class responsible to handle the get mapping command defined in
    ///  EFEM Controller Comm Specs 211006.pdf §4.3.4 oGMAP
    /// </summary>
    public class GetMappingPatternCommand : LoadPortCommand
    {
        #region Constructors
        
        public static GetMappingPatternCommand NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new GetMappingPatternCommand(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        public static GetMappingPatternCommand NewEvent(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new GetMappingPatternCommand(
                Constants.CommandType.Event,
                loadPort,
                null,
                sender,
                eqFacade,
                logger);
        }

        private GetMappingPatternCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetMappingPattern, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand
        
        protected override bool TreatAck(Message message)
        {
            // Check parameters to ensure that received ack applies to the LoadPort of this command
            if (message.CommandParameters.Count != 1
                  || message.CommandParameters[0].Length != 2
                  || !int.TryParse(message.CommandParameters[0][0], out int port)
                  && port != (int)LoadPort)
                return false;

            var mappingPatternAsString = message.CommandParameters[0][1];
            var formattedMappingPattern = ReadMappingPattern(mappingPatternAsString);

            facade.SendEquipmentEvent((int)EventsToFacade.MappingReceived, new MappingPatternEventArgs(LoadPort, formattedMappingPattern));

            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(Message message)
        {
            // Check parameters to ensure that received event applies to the LoadPort of this command
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 2
                || !int.TryParse(message.CommandParameters[0][0], out int port)
                || port != (int)LoadPort)
                return false;

            var mappingPatternAsString = message.CommandParameters[0][1];
            var formattedMappingPattern = ReadMappingPattern(mappingPatternAsString);

            facade.SendEquipmentEvent((int)EventsToFacade.MappingReceived, new MappingPatternEventArgs(LoadPort, formattedMappingPattern));
            
            return true;
        }

        #endregion RorzeCommand

        #region Private Methods

        private List<SlotState> ReadMappingPattern(string slotMap)
        {
            // Decompose 2nd parameter to extract mapping data and send it to equipment representation.
            var formattedMappingPattern = new List<SlotState>(slotMap.Length);

            foreach (var c in slotMap)
            {
                if (Enum.TryParse(c.ToString(), out SlotState slotState))
                {
                    formattedMappingPattern.Add(slotState);
                }
                else
                {
                    Logger.Warning($"Character {c} is not recognized as {nameof(SlotState)}");
                }
            }

            return formattedMappingPattern;
        }

        #endregion Private Methods
    }
}
