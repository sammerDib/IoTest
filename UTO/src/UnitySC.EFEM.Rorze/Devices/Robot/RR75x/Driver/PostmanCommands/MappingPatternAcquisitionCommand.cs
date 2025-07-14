using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class MappingPatternAcquisitionCommand : RorzeCommand
    {
        #region Constructors

        public static MappingPatternAcquisitionCommand NewOrder(
            byte deviceId,
            uint stg,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            if (stg == 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg number can not be zero. Given value is {stg}.");
            }

            if (stg > 400)
            {
                throw new ArgumentOutOfRangeException(
                    $"The stg number can not exceed 400. Given value is {stg}.");
            }

            var parameters = new List<string> { stg.ToString() };

            return new MappingPatternAcquisitionCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private MappingPatternAcquisitionCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.MappingPatternAcquisition,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Constructor

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            var mappingPattern = message.Data.Replace(":", string.Empty);

            var slotStates = new List<RR75xSlotState>(mappingPattern.Length);
            slotStates.AddRange(
                mappingPattern.Select(
                    t => (RR75xSlotState)Enum.Parse(typeof(RR75xSlotState), t.ToString())));

            var mappingArgs =
                new MappingEventArgs(new Collection<RR75xSlotState>(slotStates));

            facade.SendEquipmentEvent((int)EFEMEvents.GetLastMappingCompleted, mappingArgs);

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }

}
