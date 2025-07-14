using Agileo.Common.Communication;
using Agileo.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class RobotMacroCommand : CompositeCommand
    {
        private readonly int _eventToFacade;
        private readonly RobotAction _action;

        public RobotMacroCommand(string commandName, IEquipmentFacade eqFacade, int eventToFacade, RobotAction action)
            : base(commandName, eqFacade)
        {
            _eventToFacade = eventToFacade;
            _action        = action;
        }

        public RobotMacroCommand(IEquipmentFacade eqFacade, int eventToFacade, RobotAction action)
            : this(string.Empty, eqFacade, eventToFacade, action)
        {
        }

        protected override void ReportWholeMacroComplete()
        {
            if (_eventToFacade != -1)
            {
                facade.SendEquipmentEvent(_eventToFacade, new RobotMovementEventArgs(_action));
            }
        }
    }
}
