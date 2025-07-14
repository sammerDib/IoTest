using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Equipment.Abstractions.Enums
{
    public enum ProcessModuleState
    {
        Unknown,
        Initializing,
        Offline,
        Idle,
        Active,
        Error,
        ShuttingDown
    }
}
