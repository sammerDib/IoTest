using System.Collections.Generic;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public delegate void ChuckStateChangedDelegate(ChuckState chuckState);

    public interface IChuckController : IDevice
    {
        void Init(List<Message> initErrors);

        ILogger Logger { get; }

        ChuckState GetState();

        event ChuckStateChangedDelegate ChuckStateChangedEvent; 
        
        bool IsChuckStateChangedEventSet { get; }

    }
}
