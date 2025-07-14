using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Measure
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class MeasureSupervisor : IMeasureService, IMeasureServiceCallback
    {
        public delegate void MeasureProgressChangedHandler(MeasurePointProgress progress);

        public event MeasureProgressChangedHandler MeasureProgressChangedEvent;

        public delegate void MeasureResultChangedHandler(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex);

        public event MeasureResultChangedHandler MeasureResultChangedEvent;

        public delegate void MeasureStaticResultStartedHandler(int repeatIndex);

        public event MeasureStaticResultStartedHandler StaticMeasureResultStartedEvent;

        public delegate void MeasureStaticResultChangedHandler(MeasurePointResult result, int repeatIndex);

        public event MeasureStaticResultChangedHandler StaticMeasureResultChangedEvent;

        private DuplexServiceInvoker<IMeasureService> _measureService;

        public MeasureSupervisor()
        {
            var instanceContext = new InstanceContext(this);
            _measureService = new DuplexServiceInvoker<IMeasureService>(instanceContext,
               "ANALYSEMeasureService", ClassLocator.Default.GetInstance<ILogger<IMeasureService>>(), ClassLocator.Default.GetInstance<IMessenger>(), x => x.SubscribeToMeasureChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
        }

        public Response<List<MeasureType>> GetAvailableMeasures()
        {
            return _measureService.TryInvokeAndGetMessages(s => s.GetAvailableMeasures());
        }

        public Response<MeasureToolsBase> GetMeasureTools(MeasureSettingsBase measureSettings)
        {
            return _measureService.TryInvokeAndGetMessages(s => s.GetMeasureTools(measureSettings));
        }

        public Response<MeasureConfigurationBase> GetMeasureConfiguration(MeasureType measureType)
        {
            return _measureService.TryInvokeAndGetMessages(s => s.GetMeasureConfiguration(measureType));
        }

        public Response<VoidResult> SubscribeToMeasureChanges()
        {
            return _measureService.TryInvokeAndGetMessages(s => s.SubscribeToMeasureChanges());
        }

        public Response<VoidResult> UnsubscribeToMeasureChanges()
        {
            return _measureService.TryInvokeAndGetMessages(s => s.UnsubscribeToMeasureChanges());
        }

        public void MeasureProgressChanged(MeasurePointProgress measurePoint)
        {
            MeasureProgressChangedEvent?.Invoke(measurePoint);
        }

        public void MeasureResultChanged(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex)
        {
            MeasureResultChangedEvent?.Invoke(res, resultFolderPath, dieIndex);
        }

        public void StaticMeasureResultStarted(int repeatIndex)
        {
            StaticMeasureResultStartedEvent?.Invoke(repeatIndex);
        }

        public void StaticMeasureResultChanged(MeasurePointResult res, int repeatIndex)
        {
            StaticMeasureResultChangedEvent?.Invoke(res, repeatIndex);
        }

        public Response<VoidResult> StartMeasure(MeasureSettingsBase measureSettings, MeasurePoint measurePoint,DieIndex dieIndex)
        {
            return _measureService.TryInvokeAndGetMessages(s => s.StartMeasure(measureSettings, measurePoint, dieIndex));
        }

        public Response<VoidResult> StartMeasureWithSubMeasures(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, List<MeasurePoint> subMeasurePoints, DieIndex dieIndex)
        {
            return _measureService.TryInvokeAndGetMessages(s => s.StartMeasureWithSubMeasures(measureSettings, measurePoint, subMeasurePoints, dieIndex));
        }

        public Response<VoidResult> StartStaticRepetaMeasure(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, int nbOfStaticRepeta)
        {
            return _measureService.TryInvokeAndGetMessages(s => s.StartStaticRepetaMeasure(measureSettings, measurePoint, nbOfStaticRepeta));
        }

        public Response<VoidResult> CancelMeasure()
        {
            return _measureService.TryInvokeAndGetMessages(s => s.CancelMeasure());
        }

        public Response<VoidResult> StopStaticRepetaMeasure()
        {
            return _measureService.TryInvokeAndGetMessages(s => s.StopStaticRepetaMeasure());
        }

        public Response<List<string>> GetMeasureLightIds(MeasureType measureType)
        {
            return _measureService.TryInvokeAndGetMessages(s => s.GetMeasureLightIds(measureType));
        }

    
    }
}
