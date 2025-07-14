using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.FilterCalibration
{
    public class FilterCalibrationFlowDummy : FilterCalibrationFlow
    {
        public FilterCalibrationFlowDummy(FilterCalibrationInput input, IFlowsConfiguration flowsConfiguration,
            IEmeraCamera camera) : base(input, flowsConfiguration, camera)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);

            var calibratedFilters = new List<Filter>();
            if (Input.Filters.Count > 0)
            {
                calibratedFilters.Add(UpdateFilter(Input.Filters[0], 0, 0, 1));
            }

            if (Input.Filters.Count > 1)
            {
                calibratedFilters.Add(UpdateFilter(Input.Filters[1], 0.02, 0, 1.1));
            }

            if (Input.Filters.Count > 2)
            {
                calibratedFilters.Add(UpdateFilter(Input.Filters[2], 0, 0.02, 0.9));
            }

            if (Input.Filters.Count > 3)
            {
                calibratedFilters.Add(UpdateFilter(Input.Filters[3], 0.01, 0.01, 1.2));
            }

            if (Input.Filters.Count > 4)
            {
                calibratedFilters.Add(UpdateFilter(Input.Filters[4], -0.01, 0.01, 0.8));
            }

            if (Input.Filters.Count > 5)
            {
                calibratedFilters.Add(UpdateFilter(Input.Filters[5], 0, 0, 1));
            }

            Result.Filters = calibratedFilters;
            Logger.Information("Filter calibration done");
        }

        private static Filter UpdateFilter(Filter filter, double shiftX, double shiftY, double pixelSize)
        {
            filter.ShiftX = shiftX.Millimeters();
            filter.ShiftY = shiftY.Millimeters();
            filter.PixelSize = pixelSize.Micrometers();
            filter.CalibrationStatus = new FilterCalibrationStatus(FilterCalibrationState.Calibrated);
            return filter;
        }
    }
}
