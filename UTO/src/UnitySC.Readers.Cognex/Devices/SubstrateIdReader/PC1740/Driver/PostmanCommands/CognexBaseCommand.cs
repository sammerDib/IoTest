using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Tracing;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands
{
    internal abstract class CognexBaseCommand : Command
    {
        protected byte StepCounter;

        protected Dictionary<string, string> MessageCodeContest = new();

        protected CognexBaseCommand(
            string abbreviation,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params object[] substitutionParam)
            : base(abbreviation, sender, eqFacade, substitutionParam)
        {
            commandTimeout = CognexConstants.CommandTimeout;
        }

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            var acknowledge = reply.Substring(
                0,
                reply.Length - CognexConstants.CognexEndReplyIndicatorRead.Length);
            switch (acknowledge)
            {
                case CognexConstants.CognexInvalidCommandIndicator:
                    TraceManager.Instance()
                        .Trace(
                            CognexConstants.CognexEquipmentAlias,
                            TraceLevelType.Warning,
                            $"[Invalid Command] answer was received for command \"{Name}\"");
                    return true;

                case CognexConstants.CognexGoodAcknowledge:
                    StepCounter++;
                    return true;

                default:
                    var traceKeyOfCondition = GetSpecificForCommandErrorDescription(acknowledge);
                    facade.SendEquipmentAlarm(
                        DeviceNumber,
                        false,
                        "A_COGNEX_ERROR_ACKNOWLEDGE",
                        Name,
                        traceKeyOfCondition);
                    return true;
            }
        }

        protected string GetSpecificForCommandErrorDescription(string acknowledge)
        {
            return MessageCodeContest.ContainsKey(acknowledge)
                ? MessageCodeContest[acknowledge]
                : $"Unknown acknowledge received {acknowledge}. There is no description for it.";
        }
    }
}
