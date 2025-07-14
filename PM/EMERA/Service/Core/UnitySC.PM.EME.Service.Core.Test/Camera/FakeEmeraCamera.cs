using System.Collections.Generic;
using System.Windows;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;

namespace UnitySC.PM.EME.Service.Core.Test.Camera
{
    public class FakeEmeraCamera : IEmeraCamera
    {
        public ICameraImage GetCameraImage()
        {
            throw new System.NotImplementedException();
        }

        public bool StartAcquisition()
        {
            throw new System.NotImplementedException();
        }

        public void StopAcquisition()
        {
            throw new System.NotImplementedException();
        }

        public bool IsAcquiring()
        {
            throw new System.NotImplementedException();
        }

        public CameraInfo GetCameraInfo()
        {
            return new CameraInfo { Width = 20, Height = 20 };
        }

        public MatroxCameraInfo GetMatroxCameraInfo()
        {
            throw new System.NotImplementedException();
        }

        public void SetCameraExposureTime(double exposureTime)
        {
        }

        public double GetCameraExposureTime()
        {
            throw new System.NotImplementedException();
        }

        public void SetAOI(Rect aoi)
        {
            throw new System.NotImplementedException();
        }

        public double GetCameraGain()
        {
            throw new System.NotImplementedException();
        }

        public void SetCameraGain(double gain)
        {
            throw new System.NotImplementedException();
        }

        public double GetFrameRate()
        {
            throw new System.NotImplementedException();
        }

        public ColorMode GetColorMode()
        {
            throw new System.NotImplementedException();
        }

        public List<ColorMode> GetColorModes()
        {
            throw new System.NotImplementedException();
        }

        public void SetColorMode(ColorMode colorMode)
        {
            throw new System.NotImplementedException();
        }

        public string GetCameraId()
        {
            throw new System.NotImplementedException();
        }

        public double GetCameraFrameRate()
        {
            throw new System.NotImplementedException();
        }

        ServiceImage IEmeraCamera.SingleAcquisition()
        {
            return new DummyUSPImage(20, 20, 0).ToServiceImage();
        }

        public ServiceImage SingleScaledAcquisition(Int32Rect roi, double scale)
        {
            throw new System.NotImplementedException();
        }

        public void SetImageResolution(Size imageResolution)
        {
            throw new System.NotImplementedException();
        }

        public void SetFrameRate(double framerate)
        {
            throw new System.NotImplementedException();
        }

        ServiceImage IEmeraCamera.GetCameraImage()
        {
            throw new System.NotImplementedException();
        }

        public ServiceImage GetScaledCameraImage(Int32Rect roi, double scale)
        {
            throw new System.NotImplementedException();
        }
    }
}
