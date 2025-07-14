using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeCalibrationSupervisor : ICalibrationService
    {
        public Length PixelSize { get; set; } = 1.Micrometers();

        public Response<List<Filter>> GetFilters()
        {
            return new Response<List<Filter>>
            {
                Result = new List<Filter>
                {
                    new Filter("No Filter", EMEFilter.NoFilter, 20.0, 0.Millimeters(), 0.Millimeters(),
                        2.Micrometers()),
                    new Filter("Filter", EMEFilter.BandPass450nm50, 60.0, 0.Millimeters(), 0.Millimeters(),
                        2.Micrometers())
                }
            };
        }

        public Response<string> GetCalibrationPath()
        {
            throw new NotImplementedException();
        }

        public Response<IEnumerable<ICalibrationData>> GetCalibrations()
        {
            return new Response<IEnumerable<ICalibrationData>>
            {
                Result = new List<ICalibrationData>
                {
                    new CameraCalibrationData()
                    {
                        PixelSize = PixelSize
                    }
                }
            };
        }

        public Response<int> GetNeededCalibrationCount()
        {
            return new Response<int>
            {
                Result = 5
            };
        }

        public Response<CameraCalibrationData> GetCameraCalibrationData()
        {
            return new Response<CameraCalibrationData>
            {
                Result = new CameraCalibrationData { PixelSize = PixelSize }
            };
        }

        public Response<WaferReferentialSettings> GetWaferReferentialSettings(Length waferDiameter)
        {
            return new Response<WaferReferentialSettings>
            {
                Result = new WaferReferentialSettings { }
            };
        }
        public Response<VoidResult> SaveCalibration(ICalibrationData calibrationData)
        {
            return new Response<VoidResult>();
        }       
    }
}
