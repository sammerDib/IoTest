using System;
using System.Collections.Generic;
using System.Globalization;

using Agileo.Common.Communication;

using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.Enums;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands
{
    internal class ReadCommand : CognexBaseCommand
    {
        public static ReadCommand NewOrder(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new ReadCommand(sender, eqFacade);
        }

        public ReadCommand(IMacroCommandSender sender, IEquipmentFacade eqFacade, params object[] substitutionParam)
            : base(Constants.Commands.ReadSubstrateId, sender, eqFacade, substitutionParam)
        {
            foreach (string[] strings in CognexConstants.ReadErrorStrings)
            {
                MessageCodeContest.Add(strings[0], strings[1]);
            }
        }

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            if (StepCounter == 0)
            {
                return base.TreatReply(reply, ref macroCommandData);
            }

            if (CheckIfFormatIsGoodAndParseParameters(reply, out var readResultString, out var isReadNotFail))
            {
                facade.SendEquipmentEvent(
                    (int)CommandEvents.ReadSubstrateIdCompleted,
                    new SubstrateIdReceivedEventArgs(readResultString, isReadNotFail));

                CommandComplete();
            }

            return true;
        }

        private bool CheckIfFormatIsGoodAndParseParameters(
            string replyToParse,
            out string scribeString,
            out bool isReadNotFail)
        {
            scribeString = string.Empty;
            isReadNotFail = false;

            int lengthOfStringToHave = replyToParse.Length
                                       - CognexConstants.CognexEndReplyIndicatorRead.Length
                                       - CognexConstants.CognexScribeEndingLength
                                       - CognexConstants.CognexScribeStartLength;
            if (lengthOfStringToHave <= 0)
            {
                return false;
            }

            List<string> paramList = new List<string>();
            string replyString     = replyToParse.Substring(CognexConstants.CognexScribeStartLength, lengthOfStringToHave);
            int firstCommaIndex    = replyString.IndexOf(CognexConstants.CognexParametersSeparator);

            while (replyString.Length != 0 && firstCommaIndex != -1)
            {
                paramList.Add(replyString.Substring(0, firstCommaIndex));

                replyString     = replyString.Substring(firstCommaIndex + 1);
                firstCommaIndex = replyString.IndexOf(CognexConstants.CognexParametersSeparator);
            }

            paramList.Add(replyString);
            if (paramList.Count != CognexConstants.CognexReadMacroParamNumber)
            {
                //TODO Add log when error
                return false;
            }

            scribeString = paramList[0];
            var nfi = new NumberFormatInfo { CurrencyDecimalSeparator = "." };

            if (!double.TryParse(paramList[1], NumberStyles.Any, nfi, out var tempParam))
            {
                return false;
            }

            if (tempParam == 0)
            {
                isReadNotFail = false;
                scribeString = "************";
            }
            else if (Math.Abs(tempParam - 1) < double.Epsilon)
            {
                isReadNotFail = true;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
