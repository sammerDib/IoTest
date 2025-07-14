using System;

using Agileo.Common.Communication;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands.LoginCommands
{
    public class PasswordEnteringCommand : Command
    {
        public PasswordEnteringCommand(IMacroCommandSender sender, IEquipmentFacade eqFacade)
            : base(CognexConstants.Password, sender, eqFacade)
        {
            commandTimeout = CognexConstants.CommandTimeout;
        }

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            int indexOfLoginCompletedIndicator = reply.IndexOf(CognexConstants.CognexLoginCompletedIndicator, StringComparison.Ordinal);
            if (indexOfLoginCompletedIndicator != -1
                && indexOfLoginCompletedIndicator == reply.Length - CognexConstants.CognexLoginCompletedIndicator.Length)
            {
                CommandComplete();
                return true;
            }

            return false;
        }
    }
}
