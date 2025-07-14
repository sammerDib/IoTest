namespace ADCConfiguration.ViewModel
{
    public interface INavigationViewModel
    {
        void Refresh();
        bool MustBeSave { get; }
    }
}
