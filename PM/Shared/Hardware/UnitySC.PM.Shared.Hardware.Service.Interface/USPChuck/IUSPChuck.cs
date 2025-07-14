using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public interface IUSPChuck : IDevice
    {
        event ChuckStateChangedDelegate ChuckStateChangedEvent;

        ChuckBaseConfig Configuration { get; set; }
        IChuckController ChuckController { get; set; }

        void Init();

        ChuckState GetState();

        bool IsSensorPresenceEnable(Length size);

        List<Length> GetMaterialDiametersSupported();


    }
}
