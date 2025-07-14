namespace UnitySC.PM.EME.Service.Core.Recipe
{
    public class RunOptions
    {
        public RunOptions(bool runAutoFocus, bool runBwa, bool runStitchFullImages)
        {
            RunAutoFocus = runAutoFocus;
            RunBwa = runBwa;
            RunStitchFullImages = runStitchFullImages;
        }

        public bool RunAutoFocus { get; }
        public bool RunBwa { get; }

        public bool RunStitchFullImages { get; }
    }
}
