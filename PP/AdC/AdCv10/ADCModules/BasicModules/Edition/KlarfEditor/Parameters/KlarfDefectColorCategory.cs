using ADCEngine;

namespace BasicModules.KlarfEditor
{
    ///////////////////////////////////////////////////////////////////////
    // Catégories de défauts pour l'Aso
    ///////////////////////////////////////////////////////////////////////
    internal class KlarfDefectColorCategory : IValueComparer
    {
        public string DefectLabel { get; set; }
        public string Color { get; set; } = "white";

        //public KlarfDefectColorCategory(string label)
        //{
        //    DefectLabel = label;
        //    Color = "white";
        //}

        public bool HasSameValue(object obj)
        {
            var category = obj as KlarfDefectColorCategory;
            return category != null &&
                   DefectLabel == category.DefectLabel &&
                   Color == category.Color;
        }

    }
}
