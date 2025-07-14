using Agileo.Common.Communication;
using System.Collections.Generic;

using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class SetMappingSensorCommand : RorzeCommand
    {
        #region Constructors

        public static SetMappingSensorCommand NewOrder(
            byte deviceId,
            bool isEnabled,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            var state = isEnabled
                ? "0"
                : "1";

            var parameters = new List<string> { $"D08{state}B","1"};

            return new SetMappingSensorCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private SetMappingSensorCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                $"ARM1.{RorzeConstants.SubCommands.DirectCommand}",
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            CommandComplete();
            return true;
        }

        protected override bool CheckReplyParsing(string reply)
        {
            return reply.Contains(Name);
        }

        #endregion
    }
}
