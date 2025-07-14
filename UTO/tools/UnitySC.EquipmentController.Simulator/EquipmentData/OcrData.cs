using System.Collections.Generic;

using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EquipmentController.Simulator.EquipmentData
{
    internal class OcrData
    {
        public SubstrateSide? ReceivedSide { get; set; }

        public SortedList<int, string> OcrRecipesFront { get; } = new(30);

        public SortedList<int, string> OcrRecipesBack { get; } = new(30);
    }
}
