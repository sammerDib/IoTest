using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IProbeServiceCallback))]
    [ServiceKnownType(typeof(IProbeConfig))]
    [ServiceKnownType(typeof(ProbeLiseConfig))]
    [ServiceKnownType(typeof(ProbeDualLiseConfig))]
    [ServiceKnownType(typeof(ProbeLiseHFConfig))]
    [ServiceKnownType(typeof(M1280MatroxCameraConfig))]
    [ServiceKnownType(typeof(UI524xCpNirIDSCameraConfig))]
    [ServiceKnownType(typeof(UI324xCpNirIDSCameraConfig))]
    [ServiceKnownType(typeof(SingleLiseInputParams))]
    [ServiceKnownType(typeof(DualLiseInputParams))]
    [ServiceKnownType(typeof(LiseHFInputParams))]
    [ServiceKnownType(typeof(LiseHFCalibParams))]
    [ServiceKnownType(typeof(DualLiseCalibParams))]
    [ServiceKnownType(typeof(LiseResult))]
    [ServiceKnownType(typeof(SingleLiseResult))]
    [ServiceKnownType(typeof(ProbeSample))]
    [ServiceKnownType(typeof(ProbeSampleLayer))]
    [ServiceKnownType(typeof(ObjectiveResult))]
    [ServiceKnownType(typeof(ObjectiveConfig))]
    [ServiceKnownType(typeof(ACSLightConfig))]
    [ServiceKnownType(typeof(SingleObjectiveSelectorConfig))]
    [ServiceKnownType(typeof(LineMotObjectivesSelectorConfig))]
    [ServiceKnownType(typeof(ENTTECLightConfig))]
    public interface IProbeService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToProbeChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToProbeChanges();

        [OperationContract]
        Response<List<IProbeConfig>> GetProbesConfig();

        [OperationContract]
        Response<List<ObjectivesSelectorConfigBase>> GetObjectivesSelectorData();

        [OperationContract]
        Response<List<IProbeResult>> DoMultipleMeasures(string probeID, IProbeInputParams inputParameters, int nbMeasuresWanted);

        [OperationContract]
        Response<IProbeResult> DoMeasure(string probeID, IProbeInputParams inputParameters);

        [OperationContract]
        Response<bool> SingleAcquisition(string probeID, IProbeInputParams inputParameters);

        [OperationContract]
        Response<bool> StartContinuousAcquisition(string probeID, IProbeInputParams inputParameters);

        [OperationContract]
        Response<bool> StopContinuousAcquisition(string probeID);

        [OperationContract]
        Response<bool> StartCalibration(string probeID, IProbeCalibParams probeCalibInputParameters, IProbeInputParams probeInputParameters);

        [OperationContract]
        Response<VoidResult> CancelCalibration(string probeID);

        [OperationContract]
        Response<bool> SetNewObjectiveToUse(string objectiveSelectorID, string newObjectiveToUseID, bool applyObjectiveOffset);

        [OperationContract]
        Response<ObjectiveConfig> GetObjectiveInUse(string objectiveSelectorID);

        [OperationContract]
        Response<bool> ResetObjectivesSelectors();

        [OperationContract]
        Response<bool> IsCalibrated(string probeID, IProbeCalibParams probeCalibInputParameters, IProbeInputParams probeInputParameters);
    }
}
