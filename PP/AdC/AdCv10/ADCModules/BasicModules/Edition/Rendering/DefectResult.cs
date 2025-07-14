using System.Drawing;

namespace BasicModules.Edition.Rendering
{
    public class DefectResult
    {
        public RectangleF MicronRect { get; set; }
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int? RoughBinNum { get; set; }
    }
}
