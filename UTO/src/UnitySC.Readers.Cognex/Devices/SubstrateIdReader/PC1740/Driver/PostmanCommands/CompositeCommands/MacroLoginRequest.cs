using Agileo.Common.Communication;

using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.Enums;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands.CompositeCommands
{
    internal class MacroLoginRequest : CompositeCommand
    {
        public MacroLoginRequest(IEquipmentFacade facade)
            : base(CognexConstants.MacroLoginRequest, facade)
        {
            macroCommandData = false;
        }

        public override bool TreatReply(string reply, ref object nullMacroCommandData)
        {
            bool ret = base.TreatReply(reply, ref nullMacroCommandData);

            if (ret && macroCommandData is true)
            {
                facade.SendEquipmentEvent((int)CommandEvents.CognexLoginRequestReceived, System.EventArgs.Empty);
            }

            return ret;
        }
    }
}
