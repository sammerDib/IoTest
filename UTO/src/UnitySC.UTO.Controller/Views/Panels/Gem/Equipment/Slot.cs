using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E87;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment
{
    public class Slot : Notifier
    {
        public int Index { get; set; }

        private SlotState _state;

        public SlotState State
        {
            get { return _state; }
            set { SetAndRaiseIfChanged(ref _state, value); }
        }

        private string _substrateId;

        public string SubstrateId
        {
            get { return _substrateId; }
            set { SetAndRaiseIfChanged(ref _substrateId, value); }
        }

        private string _lotId;

        public string LotId
        {
            get { return _lotId; }
            set { SetAndRaiseIfChanged(ref _lotId, value); }
        }
    }
}
