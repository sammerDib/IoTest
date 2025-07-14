using System;

using Agileo.Common.Communication;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands.LoginCommands
{
    public class UserEnteringCommand : Command
    {
        public UserEnteringCommand(IMacroCommandSender sender, IEquipmentFacade eqFacade)
            : base(CognexConstants.User, sender, eqFacade)
        {
            commandTimeout = CognexConstants.CommandTimeout;
        }

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            int indexOfPasswordIndicator = reply.IndexOf(CognexConstants.CognexPasswordIndicator, StringComparison.Ordinal);
            if (indexOfPasswordIndicator != -1
                && indexOfPasswordIndicator == reply.Length - CognexConstants.CognexPasswordIndicator.Length)
            {
                CommandComplete();
                return true;
            }

            return false;
        }
    }
}
