using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.EME.Service.Core.Recipe
{
    public class AcquisitionSettings
    {
        public AcquisitionSettings(Acquisition acquisition, ICalibrationManager calibrationManager,
            EmeHardwareManager hardware)
        {
            Name = acquisition.Name;
            Filter = GetFilterFromType(calibrationManager.GetFilters(), acquisition.Filter);
            Light = hardware.EMELights[acquisition.LightDeviceId].Config;
            ExposureTime = acquisition.ExposureTime;
        }

        public AcquisitionSettings()
        {
            Filter = new Filter();
            Light = new EMELightConfig();
        }

        private static Filter GetFilterFromType(List<Filter> filters, EMEFilter acquisitionFilter)
        {
            var filter = filters.FirstOrDefault(x => x.Type == acquisitionFilter);
            if (filter == null)
                throw new Exception($"Filter {acquisitionFilter} is not found.");
            return filter;
        }

        public string Name { get; set; }

        public Filter Filter { get; set; }

        public EMELightConfig Light { get; set; }

        public double ExposureTime { get; set; }
    }
}
