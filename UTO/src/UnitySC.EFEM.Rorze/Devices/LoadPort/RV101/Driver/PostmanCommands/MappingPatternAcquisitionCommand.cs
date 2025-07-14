using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class MappingPatternAcquisitionCommand : RV101Command
    {
        #region Constructors

        public static MappingPatternAcquisitionCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new MappingPatternAcquisitionCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade);
        }

        private MappingPatternAcquisitionCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId, RorzeConstants.Commands.MappingPatternAcquisition, sender, eqFacade)
        {
        }

        #endregion Constructor

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            var mappingPattern = message.Data.Replace(":", string.Empty);

            var slotStates = new List<RV101SlotState>(mappingPattern.Length);
            slotStates.AddRange(mappingPattern.Select(t =>
                (RV101SlotState)Enum.Parse(typeof(RV101SlotState), t.ToString())));

            var mappingArgs = new EventArgs.MappingEventArgs(new Collection<RV101SlotState>(slotStates));

            facade.SendEquipmentEvent((int)EFEMEvents.GetLastMappingCompleted, mappingArgs);

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
