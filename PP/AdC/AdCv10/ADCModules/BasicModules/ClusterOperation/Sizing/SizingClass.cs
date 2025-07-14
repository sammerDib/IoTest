using System.ComponentModel;

using ADCEngine;


namespace BasicModules.Sizing
{
    ///////////////////////////////////////////////////////////////////////
    // Classe de défauts pour le Sizing
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum eSizingType
    {
        [Description("Length")]
        ByLength,
        [Description("Area")]
        ByArea,
        [Description("Diameter")]
        ByDiameter,
        [Description("PSLLut")]
        ByPSLLut,
        [Description("Height3D")]
        ByHeight3D
    }

    public class SizingClass : IValueComparer
    {
        private SizingParameter Parameter { get; set; }

        public string DefectLabel { get; set; }
        public eSizingType Measure { get; set; }
        public double TuningMultiplier { get; set; }
        public double TuningOffset { get; set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public SizingClass()
        {
            TuningMultiplier = 1;
            TuningOffset = 0;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return DefectLabel + "-" + Measure;
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as SizingClass;
            return @class != null &&
                   ((Parameter == @class.Parameter) || (Parameter != null && Parameter.HasSameValue(@class.Parameter))) &&
                   DefectLabel == @class.DefectLabel &&
                   Measure == @class.Measure &&
                   TuningMultiplier == @class.TuningMultiplier &&
                   TuningOffset == @class.TuningOffset;
        }

    }
}
