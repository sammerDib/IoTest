using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.Enums;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands
{
    internal class GetFileListCommand : CognexBaseCommand
    {
        private readonly List<string> _fileList;
        private int _fileNumber;

        public static GetFileListCommand NewOrder(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetFileListCommand(sender, eqFacade);
        }

        public GetFileListCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params object[] substitutionParam)
            : base(Constants.Commands.GetFileList, sender, eqFacade, substitutionParam)
        {
            foreach (string[] strings in CognexConstants.GetFileListErrorStrings)
            {
                MessageCodeContest.Add(strings[0], strings[1]);
            }

            _fileList = new List<string>();
        }

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            if (StepCounter == 0)
            {
                return base.TreatReply(reply, ref macroCommandData);
            }

            if (StepCounter == 1)
            {
                int.TryParse(reply, out _fileNumber);
                StepCounter++;
            }
            else if (_fileList.Count < _fileNumber)
            {
                _fileList.Add(reply.Replace(CognexConstants.CognexEndReplyIndicatorRead, string.Empty));
            }

            if (_fileList.Count == _fileNumber)
            {
                facade.SendEquipmentEvent((int)CommandEvents.GetFileListCompleted,
                    new FileNamesReceivedEventArgs(_fileList));
                CommandComplete();
            }

            return true;
        }
    }
}
