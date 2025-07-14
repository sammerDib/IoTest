using System.Collections.Generic;
using System.Windows;

using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;

namespace UnitySC.PM.EME.Hardware.Camera
{
    public interface IEmeraCamera
    {
        ServiceImage GetCameraImage();
        ServiceImage GetScaledCameraImage(Int32Rect roi, double scale);
        ServiceImage SingleAcquisition();
        ServiceImage SingleScaledAcquisition(Int32Rect roi, double scale);
        bool StartAcquisition();
        void StopAcquisition();
        bool IsAcquiring();
        CameraInfo GetCameraInfo();
        MatroxCameraInfo GetMatroxCameraInfo();
        void SetCameraExposureTime(double exposureTime);
        double GetCameraExposureTime();
        void SetAOI(Rect aoi);
        void SetImageResolution(Size imageResolution);
        void SetFrameRate(double framerate);
        double GetCameraGain();
        void SetCameraGain(double gain);
        double GetFrameRate();
        ColorMode GetColorMode();
        List<ColorMode> GetColorModes();
        void SetColorMode(ColorMode colorMode);
        string GetCameraId();
        double GetCameraFrameRate();
    }
}
