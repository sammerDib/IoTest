using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Controller
{
    public interface ISlitDoor
    {
        SlitDoorPosition SlitDoorState { get;}

        void OpenSlitDoor();

        void CloseSlitDoor();
    }
}
