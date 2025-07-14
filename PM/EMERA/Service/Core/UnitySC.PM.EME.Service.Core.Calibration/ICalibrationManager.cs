using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Calibration
{
    public interface ICalibrationManager
    {
        List<Type> GetCalibrationTypes();
        List<Filter> GetFilters();
        DistortionData GetDistortion();
        WaferReferentialSettings GetWaferReferentialSettings(Length waferDiameter);
        void UpdateCalibration(ICalibrationData calibrationData);
        AxisOrthogonalityCalibrationData GetAxisOrthogonalityCalibrationData();

        IEnumerable<ICalibrationData> Calibrations { get; }
    }
}
