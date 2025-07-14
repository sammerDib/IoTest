using System;
using System.Diagnostics;

using Agileo.Common.Communication;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands.LoginCommands
{
    public class LoginRequestCommand : Command
    {
        private const string CommandLoginRequest = ""; // This command doesn't send anything, just waits for login

        public LoginRequestCommand(IMacroCommandSender sender, IEquipmentFacade eqFacade)
            : base(CommandLoginRequest, sender, eqFacade)
        {
        }

        public override void SendCommand()
        {
            // This command doesn't send anything but listens for data from the equipment
            // So send is overriden to be empty
        }

        public override bool TreatReply(string reply, ref object macroCommandData)
        {
            if (macroCommandData is bool)
            {
                int indexOfLoginStartIndicator = reply.IndexOf(CognexConstants.CognexLoginStartIndicator, StringComparison.Ordinal);
                if (indexOfLoginStartIndicator != -1
                    && indexOfLoginStartIndicator == reply.Length - CognexConstants.CognexLoginStartIndicator.Length)
                {
                    macroCommandData = true;
                    return true;
                }

                indexOfLoginStartIndicator = reply.IndexOf(CognexConstants.CognexLoginWelcome, StringComparison.Ordinal);
                if (indexOfLoginStartIndicator != -1)
                {
                    macroCommandData = false;
                    return true;
                }
            }
            else
            {
                Debug.Assert(false, "Null in macro data in CommandLoginRequest");
            }

            return false;
        }
    }
}
