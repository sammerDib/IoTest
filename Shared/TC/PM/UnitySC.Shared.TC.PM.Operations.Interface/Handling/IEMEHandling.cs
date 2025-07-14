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
    public interface IEMEHandling : IHandling
    {
        void PMClampMaterial(Material material);
        void PMUnclampMaterial(Material material);

    }
}
