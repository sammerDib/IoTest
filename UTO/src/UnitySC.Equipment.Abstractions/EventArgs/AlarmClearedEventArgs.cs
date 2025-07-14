using System.Collections.Generic;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Equipment.Abstractions
{
    public class AlarmClearedEventArgs : System.EventArgs
    {
        #region Properties

        public List<Alarm> Alarms { get; }

        #endregion

        #region Constructor

        public AlarmClearedEventArgs(List<Alarm> alarms)
        {
            Alarms = new List<Alarm>(alarms);
        }

        #endregion
    }
}
