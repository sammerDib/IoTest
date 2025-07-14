using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [ServiceContract]
    [ServiceKnownType(typeof(CameraCalibrationData))]
    [ServiceKnownType(typeof(WaferReferentialCalibrationData))]
    [ServiceKnownType(typeof(FilterData))]
    [ServiceKnownType(typeof(DistortionCalibrationData))]
    [ServiceKnownType(typeof(DistanceSensorCalibrationData))]
    [ServiceKnownType(typeof(AxisOrthogonalityCalibrationData))]
    public interface ICalibrationService
    {
        [OperationContract]
        Response<CameraCalibrationData> GetCameraCalibrationData();

        [OperationContract]
        Response<string> GetCalibrationPath();

        [OperationContract]
        Response<IEnumerable<ICalibrationData>> GetCalibrations();
        
        [OperationContract]
        Response<WaferReferentialSettings> GetWaferReferentialSettings(Length waferDiameter);

        [OperationContract]
        Response<List<Filter>> GetFilters();

        [OperationContract]
        Response<VoidResult> SaveCalibration(ICalibrationData calibrationData);

        [OperationContract] 
        Response<int> GetNeededCalibrationCount();
    }
}
