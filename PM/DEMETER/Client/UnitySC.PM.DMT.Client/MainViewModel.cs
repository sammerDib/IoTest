using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Client
{
    public class MainViewModel : ObservableRecipient
    {
        public PMViewModel PMViewModel { get; private set; }

        public MainViewModel()
        {
            PMViewModel = new PMViewModel(ActorType.DEMETER, ApplicationMode.Production, new MainMenuViewModel());
        }
    }
}
