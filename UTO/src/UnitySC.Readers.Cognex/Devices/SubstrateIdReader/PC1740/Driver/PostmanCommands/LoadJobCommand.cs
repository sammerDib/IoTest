using Agileo.Common.Communication;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands
{
    internal class LoadJobCommand : CognexBaseCommand
    {
        public static LoadJobCommand NewOrder(
            string jobName,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new LoadJobCommand(sender, eqFacade, jobName);
        }

        public LoadJobCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params object[] substitutionParam)
            : base(Constants.Commands.LoadJob, sender, eqFacade, substitutionParam)
        {
            foreach (string[] strings in CognexConstants.LFErrorStrings)
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
