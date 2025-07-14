namespace UnitySC.Equipment.Abstractions
{
    public class UserInformationEventArgs : System.EventArgs
    {
        #region Properties

        public string Message { get; }

        #endregion

        #region Constructor

        public UserInformationEventArgs(string message)
        {
            Message = message;
        }

        #endregion
    }
}
