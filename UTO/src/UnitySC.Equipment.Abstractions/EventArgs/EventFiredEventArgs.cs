
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Equipment.Abstractions
{
    public class EventFiredEventArgs : System.EventArgs
    {
        #region Properties

        public CommonEvent CommonEvent { get; }

        #endregion

        #region Constructor

        public EventFiredEventArgs(CommonEvent commonEvent)
        {
            CommonEvent = commonEvent;
        }

        #endregion
    }
}
