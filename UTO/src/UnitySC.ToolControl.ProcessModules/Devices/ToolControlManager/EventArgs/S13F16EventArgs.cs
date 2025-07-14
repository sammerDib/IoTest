using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;

namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs
{
    public class S13F16EventArgs : System.EventArgs
    {
        #region Properties

        public ITableData_S13F16 TableData { get; }

        #endregion

        #region Constructor

        public S13F16EventArgs(ITableData_S13F16 tableData)
        {
            TableData = tableData;
        }

        #endregion
    }
}
