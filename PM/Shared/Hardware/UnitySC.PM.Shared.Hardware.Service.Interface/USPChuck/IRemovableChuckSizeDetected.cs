using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    public interface IRemovableChuck
    {
        Length ChuckSizeDetected { get;  }
        void InitChuckSizeDetected(Length size);

    }
}
