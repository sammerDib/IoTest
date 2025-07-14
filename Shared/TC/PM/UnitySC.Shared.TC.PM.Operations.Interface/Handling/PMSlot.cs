using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public class PMSlot
    {
        public MaterialPresence MaterialPresenceState { get; set;}
        public MaterialPresence LastMaterialPresenceState { get; set;}
        public MaterialClamp MaterialClampState { get; set ;}
        public Length Size { get; set; }

        public PMSlot(Length size, MaterialPresence materialPresence)
        {
            Size = size;
            MaterialPresenceState = materialPresence;
        }
    }
}
