using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{
    public interface IChuckClamp
    {
        void ClampWafer(Length wafer);

        void ReleaseWafer(Length wafer);
    }
}
