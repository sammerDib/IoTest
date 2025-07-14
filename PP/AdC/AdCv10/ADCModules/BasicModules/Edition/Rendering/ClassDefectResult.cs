namespace BasicModules.Edition.Rendering
{
    public class ClassDefectResult
    {
        public string ClassName { get; private set; }
        public int? RoughBinNum { get; private set; }

        public ClassDefectResult(string className, int? roughBinNum)
        {
            ClassName = className;
            RoughBinNum = roughBinNum;
        }
    }
}
