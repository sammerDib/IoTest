namespace BasicModules.AsoEditor
{
    internal class CategoryStatistic
    {
        public string DefectCategory { get; set; }
        public string Color { get; set; } = "red";
        public bool SaveThumbnails { get; set; }

        public int DefectCount;
        public double TotalDefectSize;

        public override string ToString()
        {
            return "{" + DefectCategory + "->" + Color + " count=" + DefectCount + " size=" + TotalDefectSize + "}";
        }
    }
}
