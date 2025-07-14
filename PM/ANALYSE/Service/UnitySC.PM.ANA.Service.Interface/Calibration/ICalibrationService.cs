using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [ServiceContract(CallbackContract = typeof(ICalibrationServiceCallback))]
    [ServiceKnownType(typeof(LiseHFCalibrationData))]
    [ServiceKnownType(typeof(ObjectivesCalibrationData))]
    [ServiceKnownType(typeof(XYCalibrationData))]
    public interface ICalibrationService
    {
        [OperationContract]
        Response<IEnumerable<ICalibrationData>> GetCalibrations();

        [OperationContract]
        Response<VoidResult> SaveCalibration(ICalibrationData calibrationData);

        [OperationContract]
        Response<List<OpticalReferenceDefinition>> GetReferences();

   
        [OperationContract]
        Response<VoidResult> SubscribeToCalibrationChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToCalibrationChanges();
        
        #region Objective Calibration
        [OperationContract]
        Response<VoidResult> StartObjectiveCalibration(ObjectiveCalibrationInput input);

        [OperationContract]
        Response<VoidResult> CancelObjectiveCalibration();

        [OperationContract]
        Response<List<ObjectiveToCalibrate>> GetObjectivesToCalibrate();
        #endregion 

        #region XY Calibration
        [OperationContract]
        Response<VoidResult> StartXYCalibration(XYCalibrationInput input, string userName);

        [OperationContract]
        Response<VoidResult> StopXYCalibration();

        [OperationContract]
        Response<VoidResult> StartXYCalibrationTest(XYCalibrationData xyCalibration);

        [OperationContract]
        Response<List<XYCalibrationRecipe>> GetXYCalibrationRecipes();
        #endregion

        #region LiseHF Calibration
        [OperationContract]
        Response<VoidResult> StartLiseHFIntegrationTimeCalibration(LiseHFIntegrationTimeCalibrationInput input);

        [OperationContract]
        Response<VoidResult> StopLiseHFIntegrationTimeCalibration();

        [OperationContract]
        Response<VoidResult> StartLiseHFSpotCalibration(LiseHFSpotCalibrationInput input);

        [OperationContract]
        Response<VoidResult> StopLiseHFSpotCalibration();
        #endregion 
    }
}
