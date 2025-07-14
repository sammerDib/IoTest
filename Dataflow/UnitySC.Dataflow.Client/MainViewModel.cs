using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Dataflow.Client
{
    public class MainViewModel : ObservableRecipient
    {
        public PMViewModel PMViewModel { get; private set; }

        public MainViewModel()
        {
#if DEBUG
            PMViewModel = new PMViewModel(ActorType.DataflowManager, ApplicationMode.Production);
#else
            PMViewModel = new PMViewModel(ActorType.DataflowManager, ApplicationMode.Production);
#endif

        }
    }
}
