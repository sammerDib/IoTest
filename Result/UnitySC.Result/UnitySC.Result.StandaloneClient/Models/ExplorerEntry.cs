using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Result.StandaloneClient.Models
{
    public class ExplorerEntry : ObservableRecipient
    {
        public string Path { get; }

        protected ExplorerEntry(string path)
        {
            Path = path;
        }
    }
}
