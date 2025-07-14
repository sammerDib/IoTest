using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.UI;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client
{
    public class MainViewModel : ObservableRecipient
    {
        public PMViewModel PMViewModel { get; private set; }

        public MainViewModel()
        {
            var isWaferLessMode = ClassLocator.Default.GetInstance<IClientConfigurationManager>().IsWaferLessMode;

            PMViewModel = new PMViewModel(ActorType.ANALYSE, isWaferLessMode? ApplicationMode.WaferLess:ApplicationMode.Production, new MainMenuViewModel());

            var clientFDCsSupervisor=ClassLocator.Default.GetInstance<ClientFDCsSupervisor>();
        }
    }
}
