using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Jobs
{
    public class CorrespondingCarrierSlotMapViewModel
    {
        public CorrespondingCarrierSlotMapViewModel(MaterialNameListElement materialNameListElement, Carrier carrier)
        {
            Carrier = carrier;
            MaterialNameListElement = materialNameListElement;
            SlotMap = new SlotMapViewModel();
            SlotMap.UpdateSlotMap(carrier);
        }

        public MaterialNameListElement MaterialNameListElement { get; }

        public Carrier Carrier { get; }

        public SlotMapViewModel SlotMap { get; }
    }
}
