namespace UnitySC.Shared.ResultUI.Common.Message
{
    public class DisplayErrorMessage
    {
        public string ErrorMessage { get; set; }

        public DisplayErrorMessage(string message)
        {
            ErrorMessage = message;
        }
    }
}
