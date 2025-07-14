namespace ADCConfiguration.Messages
{
    public class UserChangedMessage
    {
        public ViewModel.UserViewModel OldUser { get; set; }
        public ViewModel.UserViewModel NewUser { get; set; }

    }
}
