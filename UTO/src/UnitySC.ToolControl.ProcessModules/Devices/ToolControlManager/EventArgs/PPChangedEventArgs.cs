namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs
{
    public class PPChangedEventArgs : System.EventArgs
    {
        #region Properties

        public string ProcessProgramId { get; }

        #endregion

        #region Constructor

        public PPChangedEventArgs(string processProgramId)
        {
            ProcessProgramId = processProgramId;
        }

        #endregion
    }
}
