using System.Collections.Generic;

using ADCEngine;

namespace BasicModules.Edition.KlarfEditor
{
    public class KlarfCluster : ObjectBase
    {
        public string ClassName { get; set; }
        public List<KlarfDefect> KlarfDefects { get; set; }
        public int RoughBinNum { get; set; }
        public double TotalDefectSize { get; set; }
    }

    public class KlarfDefect
    {
        public int DefectNumber { get; set; }
        public int ClusterNumber { get; set; }
        public double Area { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
    }
}
