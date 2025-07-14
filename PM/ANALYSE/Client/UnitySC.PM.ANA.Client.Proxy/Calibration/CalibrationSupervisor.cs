using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Proxy.Exceptions;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Calibration
{
    public class CalibrationSupervisor : ICalibrationService, ICalibrationServiceCallback
    {
        #region Event

        public delegate void ObjectiveCalibrationChangedHandler(ObjectiveCalibrationResult objectiveCalibrationResult);

        public event ObjectiveCalibrationChangedHandler ObjectiveCalibrationEvent;

        public delegate void XYCalibrationChangedHandler(XYCalibrationData xYCalibrationData);

        public event XYCalibrationChangedHandler XYCalibrationChangedEvent;

        public delegate void XYCalibrationTestChangedHandler(XYCalibrationTest xYCalibrationTest);

        public event XYCalibrationTestChangedHandler XYCalibrationTestChangedEvent;

        public delegate void XYCalibrationProgressHandler(string progress, ProgressType progressType);

        public event XYCalibrationProgressHandler XYCalibrationProgressEvent;

        public delegate void XYCalibrationTestProgressHandler(string progress, ProgressType progressType);

        public event XYCalibrationTestProgressHandler XYCalibrationTestProgressEvent;

        public delegate void LiseHFSpotCalibationChangedHandler(LiseHFSpotCalibrationResults liseHFSpotCalibrationResult);

        public event LiseHFSpotCalibationChangedHandler LiseHFSpotCalibrationEvent;
        
        public delegate void LiseHFRefCalibationChangedHandler(LiseHFIntegrationTimeCalibrationResults liseHFRefCalibrationResult);
        
        public event LiseHFRefCalibationChangedHandler LiseHFRefCalibrationEvent;


        #endregion Event

        private DuplexServiceInvoker<ICalibrationService> _calibrationService;

        private List<ICalibrationData> _calibrationDatasCache;

        public CalibrationSupervisor()
        {
            var instanceContext = new InstanceContext(this);
            _calibrationService = new DuplexServiceInvoker<ICalibrationService>(instanceContext,
                "ANALYSECalibrationService", ClassLocator.Default.GetInstance<SerilogLogger<ICalibrationService>>(), ClassLocator.Default.GetInstance<IMessenger>(), x => x.SubscribeToCalibrationChanges(), ClientConfiguration.GetServiceAddress(ActorType.ANALYSE));
        }

        public Response<IEnumerable<ICalibrationData>> GetCalibrations()
        {
            var res = _calibrationService.TryInvokeAndGetMessages(x => x.GetCalibrations());
            if (res?.Result != null)
                _calibrationDatasCache = res.Result.ToList();
            return res;
        }

        public Response<List<OpticalReferenceDefinition>> GetReferences()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.GetReferences());
        }


        public Response<VoidResult> SaveCalibration(ICalibrationData calibrationData)
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.SaveCalibration(calibrationData));
        }

        public void UpdateCalibrationCache()
        {
            GetCalibrations();
        }

        #region ObjectivesCalibration

        public Response<List<ObjectiveToCalibrate>> GetObjectivesToCalibrate()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.GetObjectivesToCalibrate());
        }

        public Response<VoidResult> SubscribeToCalibrationChanges()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.SubscribeToCalibrationChanges());
        }

        public Response<VoidResult> UnsubscribeToCalibrationChanges()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.UnsubscribeToCalibrationChanges());
        }

        public Response<VoidResult> StartObjectiveCalibration(ObjectiveCalibrationInput input)
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StartObjectiveCalibration(input));
        }

        public Response<VoidResult> CancelObjectiveCalibration()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.CancelObjectiveCalibration());
        }
        public void ObjectiveCalibrationChanged(ObjectiveCalibrationResult objCalibResult)
        {
            ObjectiveCalibrationEvent?.Invoke(objCalibResult);
        }


        public ObjectiveCalibration GetObjectiveCalibration(string objectiveID)
        {
            if (_calibrationDatasCache == null)
                UpdateCalibrationCache();
            var res = _calibrationDatasCache.OfType<ObjectivesCalibrationData>().FirstOrDefault()?.Calibrations.FirstOrDefault(x => x.DeviceId == objectiveID);
            if (res is null)
                throw new NullObjectiveCalibrationException($"Objective calibration is missing for {objectiveID}");
            return res;
        }

        public bool ObjectiveIsCalibrated(string objectiveID)
        {
            if (_calibrationDatasCache == null)
                UpdateCalibrationCache();
            return _calibrationDatasCache.OfType<ObjectivesCalibrationData>().FirstOrDefault()?.Calibrations.FirstOrDefault(x => x.DeviceId == objectiveID) != null;
        } 
        #endregion ObjectivesCalibration

        #region XYCalibration
        public Response<VoidResult> StartXYCalibration(XYCalibrationInput input, string userName)
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StartXYCalibration(input, userName));
        }

        public Response<VoidResult> StartXYCalibrationTest(XYCalibrationData xyCalibration)
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StartXYCalibrationTest(xyCalibration));
        }

        public Response<VoidResult> StopXYCalibration()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StopXYCalibration());
        }

        public void XYCalibrationChanged(XYCalibrationData xyCalibrationData)
        {
            XYCalibrationChangedEvent?.Invoke(xyCalibrationData);
        }

        public void XYCalibrationTestChanged(XYCalibrationTest xyCalibrationTest)
        {
            XYCalibrationTestChangedEvent?.Invoke(xyCalibrationTest);
        }

        public void XYCalibrationProgress(string progress, ProgressType progressType)
        {
            XYCalibrationProgressEvent?.Invoke(progress, progressType);
        }

        public void XYCalibrationTestProgress(string progress, ProgressType progressType)
        {
            XYCalibrationTestProgressEvent?.Invoke(progress, progressType);
        }

        public Response<List<XYCalibrationRecipe>> GetXYCalibrationRecipes()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.GetXYCalibrationRecipes());
        }

        #endregion XYCalibration

        #region LiseHFCalibration
        public Response<VoidResult> StartLiseHFSpotCalibration(LiseHFSpotCalibrationInput inputs)
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StartLiseHFSpotCalibration(inputs));
        }

        public Response<VoidResult> StopLiseHFSpotCalibration()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StopLiseHFSpotCalibration());
        }

        public void LiseHFSpotCalibrationChanged(LiseHFSpotCalibrationResults liseHFSpotCalibResults)
        {
            LiseHFSpotCalibrationEvent?.Invoke(liseHFSpotCalibResults);
        }

        public Response<VoidResult> StartLiseHFIntegrationTimeCalibration(LiseHFIntegrationTimeCalibrationInput input)
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StartLiseHFIntegrationTimeCalibration(input));
        }

        public Response<VoidResult> StopLiseHFIntegrationTimeCalibration()
        {
            return _calibrationService.TryInvokeAndGetMessages(x => x.StopLiseHFIntegrationTimeCalibration());
        }

        public void LiseHFRefCalibrationChanged(LiseHFIntegrationTimeCalibrationResults liseHFRefCalibResults)
        {
            LiseHFRefCalibrationEvent?.Invoke(liseHFRefCalibResults);
        }

        #endregion LiseHFCalibration
    }
}
