using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;

namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs
{
    public class S13F13EventArgs : System.EventArgs
    {
        #region Properties

        public ITableData_S13F13 TableData { get; }

        #endregion

        #region Constructor

        public S13F13EventArgs(ITableData_S13F13 tableData)
        {
            TableData = tableData;
        }

        #endregion
    }
}
