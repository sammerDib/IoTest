using Agileo.Common.Communication;

namespace UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands
{
    public class BaseMacroCommand : CompositeCommand
    {
        private readonly int _eventToFacade;

        public BaseMacroCommand(string commandName, IEquipmentFacade eqFacade, int eventToFacade = -1)
            : base(commandName, eqFacade)
        {
            _eventToFacade = eventToFacade;
        }

        public BaseMacroCommand(IEquipmentFacade eqFacade, int eventToFacade = -1)
            : this(string.Empty, eqFacade, eventToFacade)
        {
        }

        protected override void ReportWholeMacroComplete()
        {
            if (_eventToFacade != -1)
            {
                facade.SendEquipmentEvent(_eventToFacade, System.EventArgs.Empty);
            }
        }
    }
}
