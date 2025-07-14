using ADCEngine;


namespace BasicModules.AsoEditor
{
    ///////////////////////////////////////////////////////////////////////
    // Classes de défauts pour l'Aso
    ///////////////////////////////////////////////////////////////////////
    internal class AsoDefectClass : IValueComparer
    {
        public string DefectLabel { get; set; }
        public string DefectCategory { get; set; }
        public string Color { get; set; } = "red";
        public bool SaveThumbnails { get; set; }

        public bool HasSameValue(object obj)
        {
            var @class = obj as AsoDefectClass;
            return @class != null &&
                   DefectLabel == @class.DefectLabel &&
                   DefectCategory == @class.DefectCategory &&
                   Color == @class.Color &&
                   SaveThumbnails == @class.SaveThumbnails;
        }

    }
}
