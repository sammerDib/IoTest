using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.HLS.Client
{
    public class MainViewModel : ViewModelBase
    {
        public PMViewModel PMViewModel { get; private set; }

        public MainViewModel()
        {
#if DEBUG
            PMViewModel = new PMViewModel(ActorType.HeLioS, ApplicationMode.Production);
#else
            PMViewModel = new PMViewModel(ActorType.HeLioS, ApplicationMode.Production);
#endif
        }
    }
}
