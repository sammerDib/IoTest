using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.PostmanCommands
{
    public class E84UnloadCommand : RorzeCommand
    {
        #region FIelds

        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>Create a new command for asking the load port to unload through E84.</summary>
        /// <param name="deviceId">The ID of the Load Port.</param>
        /// <param name="sender">The tcp message sender.</param>
        /// <param name="eqFacade">The driver that will manage command result.</param>
        /// <param name="logger">The logger to be used to trace messages, if necessary.</param>
        public static E84UnloadCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new E84UnloadCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                logger);
        }

        private E84UnloadCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.LoadPort,
                deviceId,
                RorzeConstants.Commands.E84Unload,
                sender,
                eqFacade)
        {
            _logger = logger;
        }

        #endregion

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            facade.SendEquipmentEvent((int)EFEMEvents.E84UnloadCompleted, System.EventArgs.Empty);
            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
