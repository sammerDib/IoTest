using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Calibration
{
    public class FakeCalibrationManager : ICalibrationManager
    {
        private readonly DistortionCalibrationData _distortionCalibrationData = new DistortionCalibrationData
        {
            DistortionData = new DistortionData
            {
                CameraMat = new[] { 1.0, 5.0, 2.0, 1.0, 5.0, 1.0, 0.0, 0.0, 1.0 },
                NewOptimalCameraMat = new[] { 1.0, 5.0, 7.0, 1.0, 4.0, 1.0, 0.0, 0.0, 1.0 },
                DistortionMat = new[] { 1.0, 5.0, 2.0, 1.0, 1.0 },
                RotationVec = new[] { 1.0, 5.0, 2.0 },
                TranslationVec = new[] { 4.0, 5.0, 2.0 }
            }
        };

        private readonly FilterData _filterData = new FilterData
        {
            Filters = new List<Filter>
            {
                new Filter("No Filter", EMEFilter.NoFilter, 0),
                new Filter("First filter", EMEFilter.BandPass450nm50, 60),
                new Filter("Second filter", EMEFilter.LowPass650nm, 120)
            }
        };

        public List<Type> GetCalibrationTypes()
        {
            return new List<Type> { typeof(FilterData), typeof(CameraCalibrationData) };
        }

        public List<Filter> GetFilters()
        {
            return _filterData.Filters;
        }

        public DistortionData GetDistortion()
        {
            return _distortionCalibrationData.DistortionData;
        }

        public WaferReferentialSettings GetWaferReferentialSettings(Length waferDiameter)
        {
            throw new NotImplementedException();
        }

        public void UpdateCalibration(ICalibrationData calibrationData)
        {
            throw new NotImplementedException();
        }

        public AxisOrthogonalityCalibrationData GetAxisOrthogonalityCalibrationData()
        {
            return new AxisOrthogonalityCalibrationData()
            {
                AngleX = new Angle(1.235838, AngleUnit.Degree),
                AngleY = new Angle(1.314034, AngleUnit.Degree)
            };
        }

        public IEnumerable<ICalibrationData> Calibrations =>
            new List<ICalibrationData> { _distortionCalibrationData, _filterData };
    }
}
