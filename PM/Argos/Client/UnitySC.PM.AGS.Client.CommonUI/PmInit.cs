using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.AGS.Client.CommonUI
{
    [Export(typeof(IPmInit))]
    [UCMetadata(ActorType = ActorType.Argos)]
    internal class PmInit : IPmInit
    {
        public ActorType ActorType => ActorType.Argos;

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
