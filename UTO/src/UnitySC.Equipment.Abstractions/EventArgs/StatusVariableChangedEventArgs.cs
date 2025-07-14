using System.Collections.Generic;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Equipment.Abstractions
{
    public class StatusVariableChangedEventArgs : System.EventArgs
    {
        #region Properties

        public List<StatusVariable> StatusVariables { get; }

        #endregion

        #region Constructor

        public StatusVariableChangedEventArgs(List<StatusVariable> statusVariables)
        {
            StatusVariables = new List<StatusVariable>(statusVariables);
        }

        #endregion
    }
}
