using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.Enums;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands
{
    internal class GetImageCommand : CognexBaseCommand
    {
        private readonly List<string> _lines;
        private readonly string _imagePath;

        public static GetImageCommand NewOrder(
            string imagePath,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetImageCommand(imagePath, sender, eqFacade);
        }

        public GetImageCommand(
            string imageImagePath,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params object[] substitutionParam)
            : base(Constants.Commands.GetImage, sender, eqFacade, substitutionParam)
        {
            _imagePath = imageImagePath;
            _lines = new List<string>();

            foreach (string[] strings in CognexConstants.GetImageErrorStrings)
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

            _lines.Add(reply);

            if (reply.Length == 6) //checksum
            {
                facade.SendEquipmentEvent(
                    (int)CommandEvents.GetImageCompleted,
                    new ImageReceivedEventArgs(_imagePath, _lines));

                CommandComplete();
            }

            return true;
        }
    }
}
