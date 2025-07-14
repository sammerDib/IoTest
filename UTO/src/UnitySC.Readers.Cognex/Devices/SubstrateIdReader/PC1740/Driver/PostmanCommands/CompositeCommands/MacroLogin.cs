using Agileo.Common.Communication;

using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.Enums;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands.CompositeCommands
{
    internal class MacroLogin : CompositeCommand
    {
        public MacroLogin(IEquipmentFacade facade)
            : base(CognexConstants.MacroLogin, facade)
        {
        }

        protected override void ReportWholeMacroComplete()
        {
            base.ReportWholeMacroComplete();

            facade.SendEquipmentEvent((int)CommandEvents.CognexLoginCompleted, System.EventArgs.Empty);
        }
    }
}
