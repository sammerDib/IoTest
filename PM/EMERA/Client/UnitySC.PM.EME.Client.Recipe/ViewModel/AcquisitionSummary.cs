using System.Linq;

using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public sealed class AcquisitionSummary
    {
        private readonly LightBench _lightBench;
        public AcquisitionSummary(Acquisition acquisition, LightBench lightBench)
        {
            _lightBench = lightBench;
            Name = acquisition.Name;
            Filter = acquisition.Filter.ToString();
            Light = _lightBench.Lights.FirstOrDefault(x => x.DeviceID == acquisition.LightDeviceId)?.Name;
            ExposureTime = acquisition.ExposureTime;            
        }
        public string Name { get; set; }
        public string Filter { get; set; }
        public string Light { get; set; }
        public double ExposureTime { get; set; }
    }
}
