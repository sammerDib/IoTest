using Agileo.Common.Communication;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands
{
    internal class SwitchOnlineCommand : CognexBaseCommand
    {
        public static SwitchOnlineCommand NewOrder(
            bool goOnline,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new SwitchOnlineCommand(sender, eqFacade, goOnline ? "1" : "0");
        }

        public SwitchOnlineCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params object[] substitutionParam)
            : base(Constants.Commands.SwitchOnline, sender, eqFacade, substitutionParam)
        {
            foreach (string[] strings in CognexConstants.SOErrorStrings)
            {
                MessageCodeContest.Add(strings[0], strings[1]);
            }
        }

        protected override string ParameterSeparator => string.Empty;

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            if (base.TreatReply(reply, ref macroCommandData))
            {
                CommandComplete();
                return true;
            }

            return false;
        }
    }
}
