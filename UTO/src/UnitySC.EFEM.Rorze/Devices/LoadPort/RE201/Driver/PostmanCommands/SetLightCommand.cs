using Agileo.Common.Communication;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Helpers;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands
{
    public class SetLightCommand : RE201Command
    {
        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            facade.SendEquipmentEvent((int)EFEMEvents.SetLightCompleted, System.EventArgs.Empty);
            CommandComplete();
            return true;
        }

        #endregion RorzeCommand

        #region Constructors

        public static SetLightCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            LoadPortIndicators lpIndicator,
            LightState mode)
        {
            var light = Converters.ToSetLpLightCmdParam(lpIndicator);
            var lightMode = Converters.ToLightStateCmdParam(mode);

            return new SetLightCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                light,
                lightMode);
        }

        private SetLightCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.LoadPort,
                deviceId,
                RorzeConstants.Commands.SetLight,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Constructors
    }
}
