using System;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Drivers.EventArgs;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class VersionAcquisitionCommand : RorzeCommand
    {
        #region fields

        private const string _version = "Ver ";

        #endregion

        #region Constructors

        public static VersionAcquisitionCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new VersionAcquisitionCommand(RorzeConstants.CommandTypeAbb.Order, deviceType, deviceId, sender, eqFacade);
        }

        private VersionAcquisitionCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.VersionAcquisition, sender, eqFacade)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool CheckReplyParsing(string reply)
        {
            return true;
        }

        protected override bool TreatAck(RorzeMessage message)
        {
            facade.SendEquipmentEvent(
                (int)EFEMEvents.GetVersionCompleted,
                new VersionAcquisitionEventArgs(message.Data));
            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
