using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CameraServiceEx : CameraService, ICameraServiceEx
    {
        protected new AnaHardwareManager HardwareManager => (AnaHardwareManager)base.HardwareManager;

        private const string DeviceName = "Camera";

        public CameraServiceEx(ILogger logger) : base(logger)
        {
        }

        public override void Init()
        {
            base.Init();
        }

        Response<bool> ICameraServiceEx.SetSettings(string cameraId, ICameraInputParams inputParameters)
        {
            return InvokeDataResponse(messageContainer =>
            {
                bool success = false;
                bool wasAcquiring = false;
                CameraBase camera = null;
                try
                {
                    camera = HardwareManager.Cameras[cameraId];

                    if (camera.IsAcquiring)
                    {
                        camera.StopContinuousGrab();
                        wasAcquiring = true;
                    }

                    if (inputParameters.ColorMode != string.Empty && inputParameters.ColorMode != camera.GetColorMode())
                        camera.SetColorMode(inputParameters.ColorMode);

                    if (!double.IsNaN(inputParameters.Gain) && inputParameters.Gain != camera.GetGain())
                        camera.SetGain(inputParameters.Gain);

                    if (!double.IsNaN(inputParameters.ExposureTimeMs) && !inputParameters.ExposureTimeMs.Near(camera.GetExposureTimeMs(), 0.001))
                        camera.SetExposureTimeMs(inputParameters.ExposureTimeMs);

                    if (!double.IsNaN(inputParameters.FrameRate) && !inputParameters.FrameRate.Near(camera.GetFrameRate(), 0.001))
                        camera.SetFrameRate(inputParameters.FrameRate);
                    success = true;
                }
                catch (Exception e)
                {
                    ReformulationMessage(messageContainer, e.Message);
                    success = false;
                }
                finally
                {
                    if (wasAcquiring && camera != null)
                        camera.StartContinuousGrab();
                }

                return success;
            });
        }

        Response<ICameraInputParams> ICameraServiceEx.GetSettings(string cameraId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    var camera = HardwareManager.Cameras[cameraId];
                    return (ICameraInputParams)new CameraInputParams()
                    {
                        Gain = camera.GetGain(),
                        ExposureTimeMs = camera.GetExposureTimeMs(),
                        FrameRate = camera.GetFrameRate(),
                        ColorMode = camera.GetColorMode(),
                    };
                }
                catch (Exception e)
                {
                    ReformulationMessage(messageContainer, e.Message);
                    return null;
                }
            });
        }

        Response<ServiceImage> ICameraServiceEx.GetSingleGrabImage(string cameraId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    var camera = (USPCameraBase)HardwareManager.Cameras[cameraId];
                    USPImage procimg = camera.SingleGrab();
                    if (procimg != null)
                    {
                        return procimg.ToServiceImage();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    ReformulationMessage(messageContainer, e.Message);
                    return null;
                }
            });
        }

        //TODO : Methode dupliquer de AxesService : a deplacer dans DuplexServiceBase ?
        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            //if (!string.IsNullOrEmpty(userContent))
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }
    }
}
