using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.HLS.Client.CommonUI
{
    [Export(typeof(IPmInit))]
    [UCMetadata(ActorType = ActorType.HeLioS)]
    internal class PmInit : IPmInit
    {
        public ActorType ActorType => ActorType.HeLioS;

        public void BootStrap()
        {
            // Todo
            // Bootstrapper.Register();
        }

        public void Init(bool isStandalone)
        {
            // Todo
            //throw new NotImplementedException();
        }
    }
}
