using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Interlock;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class MatroxCameraManager
    {
        private readonly ILogger _logger;
        private readonly IHardwareLoggerFactory _hardwareLoggerFactory;

        public MatroxCameraManager(ILogger logger, IHardwareLoggerFactory hardwareLoggerFactory)
        {
            _logger = logger;
            _hardwareLoggerFactory = hardwareLoggerFactory;
        }

        public Dictionary<string, CameraBase> Cameras { get; set; } = new Dictionary<string, CameraBase>();

        public bool Init(List<MatroxCameraConfigBase> configs)
        {
            bool initFatalError = false;

            // For now it's mainly to build and initialize Spyder3 that doesn't support command through Matrox API
            initFatalError |= InitCameras(configs.FindAll(c => c is Spyder3MatroxCameraConfig));

            initFatalError |= InitCamerasWithMIL(configs.FindAll(c => !(c is Spyder3MatroxCameraConfig)));

            return initFatalError;
        }

        private bool InitCameras(List<MatroxCameraConfigBase> configs)
        {
            int deviceIndex = 0;
            foreach (var config in configs)
            {
                MatroxCameraBase camera = null;
                try
                {
                    var milSystem = Mil.Instance.GetSystemInstance(config.MilSystemDescriptor);

                    camera = CameraFactory(config);
                    camera.Init(milSystem, deviceIndex);
                    Cameras.Add(config.DeviceID, camera);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"{config.Name} camera initialization error");
                    if (camera != null)
                        camera.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    return true;
                }

                deviceIndex++;
            }

            return false;
        }


        private bool InitCamerasWithMIL(List<MatroxCameraConfigBase> configs)
        {
            _logger.Information("Scanning for cameras...");
            var availableCameras = EnumerateCameras(configs);

            // We create and then initialize the cameras
            foreach (var config in configs)
            {
                MatroxCameraBase cam = null;
                try
                {
                    var camDesc = availableCameras.Find(c => c.SerialNumber == config.SerialNumber);
                    if (camDesc == null)
                        throw new ApplicationException("Can't find camera SN:" + config.SerialNumber);
                    if (camDesc.MilSystemDescriptor != config.MilSystemDescriptor)
                        throw new ApplicationException("camera " + config.SerialNumber +
                                                       " is not of the expected type (" + camDesc.MilSystemDescriptor +
                                                       ")");

                    _logger.Information("Initializing camera " + config.SerialNumber);
                    var milSystem = Mil.Instance.GetSystemInstance(config.MilSystemDescriptor);

                    cam = CameraFactory(config);
                    cam.Init(milSystem, camDesc.DevNumber);
                    Cameras.Add(config.DeviceID, cam);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, string.Format("{0} camera initialization error", config.Name));
                    if (cam != null)
                        cam.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    return true;
                }
            }

            return false;
        }

        private  MatroxCameraBase CameraFactory(MatroxCameraConfigBase config)
        {
            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var logger = _hardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Camera.ToString(), config.Name);

            switch (config)
            {
                case VC155MXMatroxCameraConfig cfg:
                    return new VC155MXCamera(cfg, globalStatusServer, logger);

                case VC151MXMatroxCameraConfig cfg:
                    return new VC151MXCamera(cfg, globalStatusServer, logger);

                case VX29MGMatroxCameraConfig cfg:
                    return new VX29MGCamera(cfg, globalStatusServer, logger);

                case VieworksVH16MG2M4MatroxCameraConfig cfg:
                    return new VieworksVH16MG2M4MatroxCamera(cfg, globalStatusServer, logger);

                case VieworksVC65MXM31CameraConfig cfg:
                    return new VieworksVC65MXM31Camera(cfg, globalStatusServer, logger);

                case AVTGE4900CameraConfig cfg:
                    return new AVTGE4900Camera(cfg, globalStatusServer, logger);

                case AVTGE4400CameraConfig cfg:
                    return new AVTGE4400Camera(cfg, globalStatusServer, logger);

                case BaslerAce2ProCameraConfig cfg:
                    return new BaslerAce2ProCamera(cfg, globalStatusServer, logger);

                case VieworksVtMatroxCameraConfig cfg:
                    return new VieworksVt(cfg, globalStatusServer, logger);

                case M1280MatroxCameraConfig cfg:
                    return new M1280MatroxCamera(cfg, globalStatusServer, logger);

                case ELIIXA16KMatroxCameraConfig cfg:
                    return new ELIIXA16KMatroxCamera(cfg, globalStatusServer, logger);

                case Spyder3MatroxCameraConfig cfg:
                    return new Spyder3Camera(cfg, globalStatusServer, logger);

                default:
                    throw new ApplicationException("Unknown camera class" + config.GetType());
            }
        }

        private List<MatroxCameraDescription> EnumerateCameras(List<MatroxCameraConfigBase> configs)
        {
            var availableCameras = new List<MatroxCameraDescription>();

            foreach (var group in configs.GroupBy(c => c.MilSystemDescriptor))
            {
                string descriptor = group.Key;
                var listSN = group.Select(c => c.SerialNumber).ToList();

                var milSystem = Mil.Instance.GetSystemInstance(descriptor);

                /* Get information on the camera system */
                //MIL_INT systemType = MIL.M_NULL;
                //MIL.MsysInquire(milSystem.MilId, MIL.M_SYSTEM_TYPE, ref systemType);

                long nbDigi = milSystem.DigitizerNum;
                for (int i = 0; i < nbDigi; i++)
                {
                    MIL_ID digID = 0;
                    try
                    {
                        _logger.Debug("Scanning " + descriptor + " / camera " + i);
                        // This prevents the VieworksVC65MXM31 Camera to work so instead of MIL.M_MINIMAL we use MIL.M_DEFAULT
                        //MIL.MdigAlloc(milSystem, i, "M_DEFAULT", MIL.M_MINIMAL, ref digID);
                        MIL.MdigAlloc(milSystem, i, "M_DEFAULT", MIL.M_DEFAULT, ref digID);
                        
                        // We need to check whether the camera is already acquiring in case the previous process was killed
                        // during a continuous grab. Also, it seems necessary, from the tests conducted, that the call to stop
                        // the acquisition be done after the first call to MdigAlloc and before the call to MdigFree happens
                        // otherwise 
                        bool isCameraAlreadyAcquiring = false;
                        MIL.MdigInquireFeature(digID, MIL.M_FEATURE_VALUE, "AcquisitionStatus", MIL.M_TYPE_BOOLEAN,
                                               ref isCameraAlreadyAcquiring);
                        if (isCameraAlreadyAcquiring)
                        {
                            _logger.Warning($"{descriptor} / camera {i} is already acquiring images. Trying to stop the acquisition");
                            try
                            {
                                MIL.MdigControlFeature(digID, MIL.M_FEATURE_EXECUTE, "AcquisitionStop", MIL.M_DEFAULT,
                                                       MIL.M_NULL);
                            }
                            catch (Exception _ignored)
                            {
                                _logger
                                    .Debug($"{descriptor} / camera {i} acquisition couldn't be stopped using AcquisitionStop command. Trying GRAB_ABORT");
                                try
                                {
                                    MIL.MdigControl(digID, MIL.M_GRAB_ABORT, MIL.M_DEFAULT);
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error($"Couldn't stop acquisition. {descriptor} / camera {i} will not be available.");
                                    throw;
                                }
                            }

                            MIL.MdigInquireFeature(digID, MIL.M_FEATURE_VALUE, "AcquisitionStatus", MIL.M_TYPE_BOOLEAN,
                                                   ref isCameraAlreadyAcquiring);
                            if (isCameraAlreadyAcquiring)
                            {
                                throw new Exception($"Couldn't stop acquisition. {descriptor} / camera {i} will not be available.");
                            }

                            _logger.Information($"{descriptor} / camera {i} acquisition successfully stopped.");
                        }
                        // NB: M_SERIAL_NUMBER  et M_SERIAL_NUMBER_SIZE don't work anymore.
                        MIL_INT StringLength = 0;
                        MIL.MdigInquire(digID, MIL.M_GC_SERIAL_NUMBER_SIZE, ref StringLength);
                        var DigName = new StringBuilder((int)StringLength);
                        MIL.MdigInquire(digID, MIL.M_GC_SERIAL_NUMBER, DigName);

                        var availableCamera = new MatroxCameraDescription
                                              {
                                                  SerialNumber = DigName.ToString(),
                                                  MilSystemDescriptor = descriptor,
                                                  DevNumber = i
                                              };
                        availableCameras.Add(availableCamera);
                        if (availableCameras.Select(c => c.SerialNumber).ContainsAll(listSN))
                            break;
                    }
                    catch (Exception Ex)
                    {
                        _logger.Warning("Can't access " + descriptor + " / camera " + i + ": " + Ex.Message);
                    }
                    finally
                    {
                        if (digID != 0)
                        {
                            MIL.MdigFree(digID);
                        }
                    }
                }
            }

            return availableCameras;
        }

        private class MatroxCameraDescription
        {
            /// <summary> Matrox M_DEVx number </summary>
            public int DevNumber;

            /// <summary>
            ///     Descripteur MIL du sytème (ex: M_SYSTEM_GIGE_VISION)
            /// </summary>
            public string MilSystemDescriptor;

            public string SerialNumber;
        }
    }
}
