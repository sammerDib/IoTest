using UnitySC.Shared.Data.SecsGem;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs
{
    public class CollectionEventEventArgs : System.EventArgs
    {
        #region Properties

        public string CollectionEventName { get; }

        public ISecsVariableList DataVariables { get; }

        #endregion

        #region Constructor

        public CollectionEventEventArgs(string collectionEventName, ISecsVariableList dataVariables)
        {
            CollectionEventName = collectionEventName;
            DataVariables = dataVariables;
        }

        #endregion
    }
}
