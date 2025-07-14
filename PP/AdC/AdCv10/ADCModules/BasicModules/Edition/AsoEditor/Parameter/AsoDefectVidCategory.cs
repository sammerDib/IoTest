using ADCEngine;

namespace BasicModules.AsoEditor
{
    ///////////////////////////////////////////////////////////////////////
    // Catégories de défauts pour l'Aso
    ///////////////////////////////////////////////////////////////////////
    internal class AsoDefectVidCategory : IValueComparer
    {
        public int VID { get; set; } = -1;
        public string DefectCategory { get; set; }
        public string Color { get; set; } = "red";
        public bool SaveThumbnails { get; set; }

        public bool HasSameValue(object obj)
        {
            var category = obj as AsoDefectVidCategory;
            return category != null &&
                   VID == category.VID &&
                   DefectCategory == category.DefectCategory &&
                   Color == category.Color &&
                   SaveThumbnails == category.SaveThumbnails;
        }

    }
}
