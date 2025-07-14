using System.Collections.Generic;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public interface IControllerIO
    {
        Dictionary<string, Input> NameToInput { get; set; }

        Dictionary<string, Output> NameToOutput { get; set; }

        Input GetInput(string name);

        Output GetOutput(string name);

        double AnalogRead(AnalogInput input);

        void AnalogWrite(AnalogOutput output, double value);

        bool DigitalRead(DigitalInput input);

        void DigitalWrite(DigitalOutput output, bool value);

        Task StartRefreshIOStatesTask();

        void StopRefreshIOStatesTask();
    }
}
