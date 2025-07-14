using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.DummyCamera
{
    public class DummyIDSCamera : USPCameraBase
    {
        private CameraConfigBase _config;
        private double _exposureTimeMs = 1.0;
        private double _gain = 1.0;
        private double _frameRate = 5.0;
        private TriggerMode _triggerMode;
        private string _colorMode = "";
        private List<string> _images = new List<string>();
        private readonly ManualResetEvent _synchro = new ManualResetEvent(false);
        private byte _color = 0xFD;

        //High framerate to make sure at least 1 image is grabbed during some tests
        private double _maxFrameRate = 1000;
        private readonly int _depth;

        public override double MaxFrameRate => _maxFrameRate;

        public DummyIDSCamera(CameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
            _config = config;
            _depth = config.Depth;
            Width = 1920;
            Height = 1080;
            MaxExposureTimeMs = 0.023;
            MaxExposureTimeMs = 60_000;
            MinGain = 0;
            MaxGain = 32;
            Model = "DummyIDSCamera";
            SerialNumber = "DummySN";
            State = new DeviceState(DeviceStatus.Ready);
            //The first image created always takes the longest time so we create it here to not interfere with the test timings
            var testImage = new DummyUSPImage(10, 20, 0, true);
        }

        public override void Init()
        {
            base.Init();
            Logger.Information("Init IdsCamera as dummy");
        }

        public override void InitSettings()
        {
            // Nothing to do
        }

        // Implémentation de CameraBase
        public override void Shutdown()
        { }

        public override double GetExposureTimeMs()
        {
            return _exposureTimeMs;
        }

        public override double GetGain()
        {
            return _gain;
        }

        public override double GetFrameRate()
        {
            return _frameRate;
        }

        public override string GetColorMode()
        {
            return _colorMode;
        }

        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            _exposureTimeMs = exposureTime_ms;
        }

        public override void SetGain(double gain)
        {
            _gain = gain;
        }

        public override void SetFrameRate(double frameRate)
        {
            _frameRate = frameRate;
        }

        public override void SetColorMode(string colorMode)
        {
            _colorMode = colorMode;
        }

        public override void SetTriggerMode(TriggerMode mode)
        {
            _triggerMode = mode;
        }

        public override void SetAOI(Rect aoi)
        {
        }

        public override USPImage SingleGrab()
        {
            USPImage procimg = GetNextImage();
            Messenger.Send(new CameraMessage() { Camera = this, Image = procimg });
            return procimg;
        }

        public override void SoftwareTrigger()
        {
            _synchro.Reset();
            SingleGrab();
            _synchro.Set();
        }

        public override void WaitForSoftwareTriggerGrabbed()
        {
            int timePerAcquisitionInMilliSec = (int)(1000 / _frameRate);
            _synchro.WaitOne(timePerAcquisitionInMilliSec * 10);
        }

        public override void StartContinuousGrab()
        {
            IsAcquiring = true;
            if (_triggerMode == TriggerMode.Off)
                Task.Run(() => GrabTask());
        }

        public override void StopContinuousGrab()
        {
            IsAcquiring = false;
        }

        private void GrabTask()
        {
            while (IsAcquiring)
            {
                int timePerAcquisitionInMilliSec = (int)(1000 / _frameRate);
                Thread.Sleep(timePerAcquisitionInMilliSec);
                SoftwareTrigger();
            }
        }

        protected virtual USPImage GetNextImage()
        {
            _color = (byte)((_color + 1) % 256);
            return new DummyUSPImage(Width, Height, _color, true, _depth);
        }

        public override List<string> GetColorModes()
        {
            return new List<string>(); // TODO
        }

        public override void SetImageResolution(Size size)
        {
            Width = (int)size.Width;
            Height = (int)size.Height;
        }
    }
}
