using System;
using System.Collections.Generic;

using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EquipmentController.Simulator.Driver.EventArgs
{
    /// <summary>
    /// Contain all information about OCR (substrate id reader) recipes.
    /// </summary>
    internal class OcrRecipesReceivedEventArgs : System.EventArgs
    {
        public OcrRecipesReceivedEventArgs(
            SubstrateSide requestedSide,
            SortedList<int, string> ocrRecipesFront,
            SortedList<int, string> ocrRecipesBack)
        {
            RequestedSide   = requestedSide;
            OcrRecipesFront = ocrRecipesFront ?? throw new ArgumentNullException(nameof(ocrRecipesFront));
            OcrRecipesBack  = ocrRecipesBack ?? throw new ArgumentNullException(nameof(ocrRecipesBack));
        }

        public SubstrateSide RequestedSide { get; }

        public SortedList<int, string> OcrRecipesFront { get; }

        public SortedList<int, string> OcrRecipesBack { get; }
    }
}
