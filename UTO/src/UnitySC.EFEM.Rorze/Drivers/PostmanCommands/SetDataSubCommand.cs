using System;
using System.Collections.Generic;
using System.Text;

using Agileo.Common.Communication;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    /// <summary>
    /// Sub-command allowing to get data from an hardware device data table.
    /// </summary>
    public class SetDataSubCommand : RorzeSubCommand
    {
        private readonly bool _mustSendEquipmentEvent;

        #region Constructor

        public static SetDataSubCommand NewIndividualSettingCommand(
            string deviceType,
            byte deviceId,
            string devicePart,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent,
            string value,
            params uint[] indexes)
        {
            var indexParameters = new string[indexes.Length];

            for (var i = 0; i < indexes.Length; i++)
            {
                indexParameters[i] = indexes[i].ToString();
            }

            return new SetDataSubCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceType,
                deviceId,
                devicePart,
                deviceParts,
                sender,
                eqFacade,
                mustSendEquipmentEvent,
                $"={value}",
                indexParameters);
        }

        public static SetDataSubCommand NewBatchSettingCommand(
            string deviceType,
            byte deviceId,
            string devicePart,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent,
            string[] values,
            params int[] indexes)
        {
            if (values.Length == 0)
                throw new ArgumentException($"There must be at least a value to set. {nameof(values)}={values}");

            var indexParameters = new string[indexes.Length];

            for (var i = 0; i < indexes.Length; i++)
            {
                indexParameters[i] = indexes[i].ToString();
            }

            var sb = new StringBuilder("=" + values[0]);
            for (var i = 1; i < values.Length; ++i)
                sb.Append("," + values[i]);

            return new SetDataSubCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceType,
                deviceId,
                devicePart,
                deviceParts,
                sender,
                eqFacade,
                mustSendEquipmentEvent,
                sb.ToString(),
                indexParameters);
        }

        private SetDataSubCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            string devicePart,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent,
            string suffix,
            params string[] parameters)
            : base(commandType, deviceType, deviceId, devicePart, RorzeConstants.SubCommands.DataSetting, false, deviceParts, sender, eqFacade, suffix, parameters)
        {
            _mustSendEquipmentEvent = mustSendEquipmentEvent;
        }

        #endregion Constructor

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            if (_mustSendEquipmentEvent)
            {
                facade.SendEquipmentEvent((int)EFEMEvents.SetDeviceDataCompleted, System.EventArgs.Empty);
            }

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
