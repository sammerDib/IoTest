using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.PM.ANA.Client.Proxy.Context;

namespace UnitySC.PM.ANA.Client.Proxy
{
    public class CamerasSupervisor : ObservableObject, ICameraServiceEx, ICameraServiceCallback
    {
        public delegate void ObjectiveChangedHandler(string objectiveID);

        public event ObjectiveChangedHandler ObjectiveChangedEvent;

        public bool ApplyObjectiveOffset { get; set; } = true;

        #region Properties

        public List<CameraVM> Cameras { get => _cameras; }

        public CameraVM Camera
        {
            get => _camera;
            set
            {
                if (_camera == value)
                    return;
                if (_camera != null)
                {
                    _camera.IsNormalised = false;
                    _camera.StopStreamingCommand.Execute(null);
                }
                _camera = value;
                OnPropertyChanged();
                UpdateCurrentObjectiveSelector();
                FillObjectivesList();
            }
        }

        public CameraVM GetMainCamera()
        {
            if (Cameras.Count() == 0)
                FillCamerasList(_messenger);

            var camera = Cameras.FirstOrDefault(x => x.IsMainCamera);
            if (camera == null)
            {
                _logger.Error("There is no main camera defined");
                throw new Exception("There is no main camera defined");
            }
            return camera;
        }

        public CameraVM TopCamera => Cameras.FirstOrDefault(x => x.Configuration.ModulePosition == ModulePositions.Up);

        public CameraVM BottomCamera => Cameras.FirstOrDefault(x => x.Configuration.ModulePosition == ModulePositions.Down);
        public ObservableCollection<ObjectiveConfig> Objectives { get => _objectives; }

        public ObjectiveConfig Objective
        {
            get => _objective;
            set
            {
                if (value == null)
                    return;

                if (_objective == value)
                    return;

                if ((_objectiveSelectorCurrent is null) || (_objective is null))
                    return;
                // Is objective managed by the current objective selector ?
                if (_objectiveSelectorCurrent.Objectives.FirstOrDefault(o => o.DeviceID == value.DeviceID) == null)
                    return;
                _objective = value;

                Task.Run(
                    () =>
                    {
                        _probesSupervisor.SetNewObjectiveToUse(_objectiveSelectorCurrent.DeviceID, _objective.DeviceID, ApplyObjectiveOffset);
                        ObjectiveChangedEvent?.Invoke(_objective?.DeviceID);
                    }
                );

                OnPropertyChanged();
                OnPropertyChanged(nameof(PixelSizeXmm));
                OnPropertyChanged(nameof(PixelSizeYmm));
            }
        }

        public ObjectiveConfig MainObjective => ClassLocator.Default.GetInstance<CamerasSupervisor>().Objectives.FirstOrDefault(o => o.IsMainObjective);

        public double PixelSizeXmm => ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(Objective.DeviceID).Image.PixelSizeX.Millimeters;

        public double PixelSizeYmm => ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(Objective.DeviceID).Image.PixelSizeY.Millimeters;

        public bool WebcamEnabled
        {
            get => _webcamEnabled; set { if (_webcamEnabled != value) { _webcamEnabled = value; OnPropertyChanged(); } }
        }

        public string WebcamUrl { get; private set; }

        #endregion Properties

        #region Fields

        private bool _webcamEnabled;
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<ICameraServiceEx> _cameraService;
        private ProbesSupervisor _probesSupervisor;
        private List<CameraVM> _cameras;
        private CameraVM _camera;
        private List<ObjectivesSelectorConfigBase> _objectivesSelectors;
        private ObjectivesSelectorConfigBase _objectiveSelectorCurrent;
        private ObservableCollection<ObjectiveConfig> _objectives;
        private ObjectiveConfig _objective;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CamerasSupervisor(ILogger<CamerasSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            // Camera service
            _cameraService = new DuplexServiceInvoker<ICameraServiceEx>(_instanceContext, "ANALYSECameraService", ClassLocator.Default.GetInstance<SerilogLogger<ICameraServiceEx>>(), messenger, null, ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
            _logger = logger;
            _messenger = messenger;

            _probesSupervisor = ServiceLocator.ProbesSupervisor;
            _probesSupervisor.ObjectiveChangedEvent += ProbesSupervisor_ObjectiveChangedEvent;

            _objectivesSelectors = _probesSupervisor.ObjectivesSelectors;
            FillCamerasList(messenger);

            InitWebCam();
        }

        private void ProbesSupervisor_ObjectiveChangedEvent(ObjectiveResult newObjective)
        {   
            if (Objectives is null)
                return;
            Objective = Objectives.FirstOrDefault(x => x.DeviceID == newObjective.ObjectiveID);
        }

        #endregion Constructor

        #region Public methods

        public void StopAllStreaming()
        {
            _cameras.ForEach(c =>
            {
                if (c != null)
                {
                    c.IsNormalised = false;
                    c.StopStreamingCommand.Execute(null);
                }
            });
        }

        public Response<ServiceImage> GetCameraImage(string cameraId)
        {
            var resp = new Response<ServiceImage>();

            var respOrigin = _cameraService.TryInvokeAndGetMessages(s => s.GetCameraImage(cameraId));
            if (respOrigin != null)
            {
                resp.Messages = respOrigin.Messages;
                resp.Exception = respOrigin.Exception;
                resp.Result = respOrigin.Result;
            }
            return resp;
        }

        public Response<CameraInfo> GetCameraInfo(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.GetCameraInfo(cameraId));
        }

        public Response<ServiceImage> GetSingleGrabImage(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.GetSingleGrabImage(cameraId));
        }

        public void ImageGrabbedCallback(string cameraId, ServiceImage image)
        {
            _logger.Debug("Image grabbed");
            _messenger.Send(new ImageGrabbedMessage() { ServiceImage = image });
        }

        public Response<VoidResult> SetCameraExposureTime(string cameraId, double exposureTimeMs)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.SetCameraExposureTime(cameraId, exposureTimeMs));
        }

        public Response<bool> SetSettings(string cameraId, ICameraInputParams inputParameters)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.SetSettings(cameraId, inputParameters));
        }

        public Response<ICameraInputParams> GetSettings(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.GetSettings(cameraId));
        }

        public Response<VoidResult> StartAcquisition(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.StartAcquisition(cameraId));
        }

        public Response<VoidResult> StopAcquisition(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.StopAcquisition(cameraId));
        }
        public Response<double> GetCameraFrameRate(string cameraId)
        {
            return _cameraService.TryInvokeAndGetMessages(s => s.GetCameraFrameRate(cameraId));
        }

        public Response<VoidResult> Subscribe(Int32Rect acquisitionRoi, double scale)
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _cameraService.TryInvokeAndGetMessages(s => s.Subscribe(acquisitionRoi, scale));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Camera subscribe error");
            }
            return resp;
        }

        public Response<VoidResult> Unsubscribe()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _cameraService.TryInvokeAndGetMessages(s => s.Unsubscribe());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Camera unsubscribe error");
            }

            return resp;
        }

        /// <summary>
        /// Wait until current top objective context matches with the given objectiveId
        /// Or exits if timeout of 3s has been reached before
        /// </summary>
        /// <param name="objectiveId">The objective id to match</param>
        /// <param name="waitMotionEnd">True if we have to wait for motion end after objective selection</param>
        /// <returns>true if current top objective context matches with the given objectiveId in less than 3s / false else</returns>
        public async Task<bool> WaitObjectiveChanged(string objectiveId, bool waitMotionEnd)
        {
            bool hasObjetiveChanged = false;
            await Task<bool>.WhenAny(
                Task.Run(() =>
                {
                    while (!hasObjetiveChanged)
                    {
                        if (ClassLocator.Default.GetInstance<ContextSupervisor>().GetTopObjectiveContext()?.Result.ObjectiveId == objectiveId)
                        {
                            hasObjetiveChanged = true;
                        }

                        // Add a delay before retrieving another context
                        Task.Delay(50);
                    }
                }),
                Task.Delay(3000)
            ).Finally(() =>
            {
                if (waitMotionEnd)
                {
                    ServiceLocator.AxesSupervisor.WaitMotionEnd(20_000);
                }
            });
            
            return hasObjetiveChanged;
        }

        #endregion Public methods

        #region Private methods

        private void FillCamerasList(IMessenger messenger)
        {
            if (_cameras == null)
                _cameras = new List<CameraVM>();

            if (_probesSupervisor.Probes is null)
                return;

            foreach (var probe in _probesSupervisor.Probes)
            {
                if (probe.Configuration is ISingleProbeConfig)
                {
                    foreach (var camera in (probe.Configuration as ISingleProbeConfig).Cameras.Where(x => x.IsEnabled))
                    {
                        _cameras.Add(new CameraVM(this, camera, messenger, _logger));
                    }
                }
            }

            _cameras = _cameras.GroupBy(c => c.Name).Select(grp => grp.First()).ToList();
            if (_cameras.Count > 0)
                Camera = _cameras[0];
        }

        private void FillObjectivesList()
        {
            if (_objectives == null)
                _objectives = new ObservableCollection<ObjectiveConfig>();

            _objectives.Clear();
            if (_objectiveSelectorCurrent is null)
                return;

            var objectiveCurrent = _probesSupervisor.GetObjectiveInUse(_objectiveSelectorCurrent.DeviceID)?.Result;
            int objectiveCurrentIndex = 0;
            int counter = 0;
            foreach (var objective in _objectiveSelectorCurrent.Objectives)
            {
                _objectives.Add(objective);
                if (objective.DeviceID == objectiveCurrent?.DeviceID)
                    objectiveCurrentIndex = counter;
                counter++;
            }
            if (_objectives.Count > 0)
                _objective = _objectives[objectiveCurrentIndex];

            OnPropertyChanged(nameof(Objective));
            OnPropertyChanged(nameof(Objectives));
            ObjectiveChangedEvent?.Invoke(_objective?.DeviceID);
        }

        private void UpdateCurrentObjectiveSelector()
        {
            if (_camera is null)
                return;

            if (_objectiveSelectorCurrent?.DeviceID == _camera.Configuration.ObjectivesSelectorID)
                return;

            _objectiveSelectorCurrent = _objectivesSelectors.Find(objectivesSelectorConfig => objectivesSelectorConfig.DeviceID == _camera.Configuration.ObjectivesSelectorID);
            if (_objectiveSelectorCurrent == null)
            {
                string msg = $"Objectives selector {_camera.Configuration.ObjectivesSelectorID} not found";
                _logger?.Error(msg);
                throw (new Exception(msg));
            }
        }

        private void InitWebCam()
        {
            var urls = ClassLocator.Default.GetInstance<SharedSupervisors>().GetChamberSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE).GetWebcamUrls()?.Result;

            if (urls != null && urls.Any())
                WebcamUrl = urls.First();
            else
                WebcamUrl = "";
        }

        #endregion Private methods

        public static ObjectiveConfig GetObjectiveFromId(string deviceId)
        {
            return ServiceLocator.CamerasSupervisor.Objectives.FirstOrDefault(o => o.DeviceID == deviceId);
        }        
    }
}
