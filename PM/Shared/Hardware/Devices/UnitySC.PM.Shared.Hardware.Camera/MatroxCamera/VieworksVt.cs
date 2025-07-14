using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    /// <summary>
    /// In line mode, the camera exposes continuously, and does not take the exposition time setting into account.
    ///  The exposition time per line is then the trigger time (linked to table move).
    ///  Note: the framegrabber can output a strobe that is shorter than the trigger delay, and that can limit the actual incoming light per line, if connected to the lighting module.
    /// </summary>
    public class VieworksVt : MatroxCameraBase
    {
        public VieworksVt(MatroxCameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        public new VieworksVtMatroxCameraConfig Config => (VieworksVtMatroxCameraConfig)base.Config;

        public override double GetGain()
        {
            return 1d;
        }

        public override void SoftwareTrigger()
        {
            throw new NotImplementedException();
        }

        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            if (IsSimulated)
            {
                _exposureTimeSimulated_ms = exposureTime_ms;
                return;
            }

            lock (DigitizerId_lock)
            {
                try
                {
                    // The camera supports exposure time only in area mode. Try setting the value.
                    double exposure_us = exposureTime_ms * 1000d;
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureTime", MIL.M_TYPE_DOUBLE, ref exposure_us);
                }
                catch (MILException ex) { throw new NotSupportedException("", ex); }
            }
        }

        private double _exposureTimeSimulated_ms = 100d;

        public override double GetExposureTimeMs()
        {
            if (IsSimulated)
            {
                return _exposureTimeSimulated_ms;
            }

            lock (DigitizerId_lock)
            {
                double exposure_us = -46d;
                MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureTime", MIL.M_TYPE_DOUBLE, ref exposure_us);
                return exposure_us / 1000d;
            }
        }

        public override void SetGain(double gain)
        {
            throw new NotImplementedException();
        }

        public class Temperature
        {
            /// <summary>
            /// Last measured temperature.
            /// </summary>
            public double Temp_degCel;

            /// <summary>
            /// Critical temperature.
            /// </summary>
            public double TempLimit_degCel;

            public bool ExceedingLimit => Temp_degCel >= TempLimit_degCel;
        }

        /// <summary>
        /// Called from a background thread approx. each 4.6s with temperature info.
        /// Runs as soon as the camera is connected, and until Dispose() is called.
        /// Acquisition should be stopped in case of Temperature.ExceedingLimit.
        ///  This helps the temperature to go down, but may not be enougth.
        /// Even if not running, the powered on camera may hit temperature limit.
        ///  In that case, the only way is to inform the user that it should be switched off!
        /// </summary>
        public Action<Temperature> BackgroundTemperatureInfo;

        /// <summary>
        /// Used to stop temperature monitoring.
        /// </summary>
        private readonly CancellationTokenSource _cTempMonitoring = new CancellationTokenSource();

        private async Task TemperatureMonitoringAsync()
        {
            CHECKTEMP:
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(4.6d), _cTempMonitoring.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (IsSimulated)
            {
                BackgroundTemperatureInfo?.Invoke(new Temperature()
                {
                    TempLimit_degCel = Config.TemperatureLimit_degCel,
                    Temp_degCel = 46d
                });
            }
            else
            {
                double deviceTemperatureInC = -46d;
                lock (DigitizerId_lock)
                {
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "DeviceTemperature", MIL.M_TYPE_DOUBLE, ref deviceTemperatureInC);
                }

                BackgroundTemperatureInfo?.Invoke(new Temperature()
                {
                    TempLimit_degCel = Config.TemperatureLimit_degCel,
                    Temp_degCel = deviceTemperatureInC
                });
            }

            goto CHECKTEMP;
        }

        private Task _temperatureMonitoringTask;

        public override void SetTriggerMode(TriggerMode mode)
        {
            if (IsSimulated)
            {
                _triggerModeSimulated = mode;
                return;
            }

            lock (DigitizerId_lock)
            {
                switch (mode)
                {
                    case TriggerMode.Off:
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, "Off");
                        break;

                    default:
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, "On");
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, "CXPin");
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerActivation", MIL.M_TYPE_STRING, "RisingEdge");

                        // Send timer 1 output to cxp.
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TL_TRIGGER + MIL.M_IO_MODE, MIL.M_OUTPUT);
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TL_TRIGGER + MIL.M_IO_INTERRUPT_ACTIVATION, MIL.M_ANY_EDGE);
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TL_TRIGGER + MIL.M_IO_SOURCE, MIL.M_TIMER1);

                        // Configure timer 1 to create a 1µs signal when notified.
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_DURATION, CxpTriggerDuration_ns);
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_OUTPUT_INVERTER, MIL.M_DISABLE);
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_TRIGGER_ACTIVATION, MIL.M_EDGE_RISING);
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_STATE, MIL.M_ENABLE);
                        break;
                }

                // Set timer 1 input.
                switch (mode)
                {
                    case TriggerMode.Hardware:
                        // Start timer 1 immediately after each incoming hardware trigger.
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_DELAY, 0);
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER0 + MIL.M_TIMER_TRIGGER_SOURCE, MIL.M_AUX_IO0);
                        break;

                    case TriggerMode.Software:
                        // Ask the frame grabber to start timer 1 at a constant rate (timer period = M_TIMER_DURATION + M_TIMER_DELAY).
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_DELAY, (GetExposureTimeMs() * 1000000d) - CxpTriggerDuration_ns);
                        MIL.MdigControl(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_TRIGGER_SOURCE, MIL.M_CONTINUOUS);
                        break;
                }
            }
        }

        private TriggerMode _triggerModeSimulated;

        /// <summary>
        /// Duration of the ON signal sent to the camera on CXP.
        /// </summary>
        private const double CxpTriggerDuration_ns = 1000d;

        public new TriggerMode TriggerMode
        {
            get
            {
                if (IsSimulated)
                {
                    return _triggerModeSimulated;
                }

                lock (DigitizerId_lock)
                {
                    //MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "");    // This is documented, but not allowed on Vieworks TDI 23k.
                    StringBuilder sb = new StringBuilder();
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, sb);

                    if (sb.ToString() == "Off")
                    {
                        return TriggerMode.Off;
                    }

                    MIL_INT timerTriggerSource = -46;
                    MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_TIMER1 + MIL.M_TIMER_TRIGGER_SOURCE, ref timerTriggerSource);
                    switch ((Int32)timerTriggerSource)
                    {
                        case MIL.M_SOFTWARE:
                            return TriggerMode.Software;

                        default:
                            return TriggerMode.Hardware;
                    }
                }
            }
        }

        /// <summary>
        /// Changes camera mode.
        /// Returns the new image size.
        /// </summary>
        private Point SetMode(CameraMode mode)
        {
            lock (DigitizerId_lock)
            {
                if (IsSimulated)
                {
                    Width = 23360;
                    switch (mode)
                    {
                        case CameraMode.tdi:
                            // Scan line per line.
                            // Let the framegrabber stitch lines before calling the image callback.
                            Height = ((VieworksVtMatroxCameraConfig)Config).TdiModeLineBufferingCount;
                            break;

                        case CameraMode.area:
                            Height = 256;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    Int32 height = -46;
                    switch (mode)
                    {
                        case CameraMode.tdi:
                            // Scan line per line.
                            MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OperationMode", MIL.M_TYPE_STRING, "TDI");

                            // Let the framegrabber stitch lines before calling the image callback.
                            height = ((VieworksVtMatroxCameraConfig)Config).TdiModeLineBufferingCount;
                            MIL.MdigControl(DigitizerId_lock.Value, MIL.M_SOURCE_SIZE_Y, height);
                            break;

                        case CameraMode.area:
                            MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OperationMode", MIL.M_TYPE_STRING, "Area");
                            MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "DeviceScanType", MIL.M_TYPE_STRING, "Areascan");   // Can only be set if OperationMode is Area.

                            // In area mode, M_SOURCE_SIZE_Y takes its value automatically.
                            MIL_INT sourceSizeYInPixels = 0;
                            MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SOURCE_SIZE_Y, ref sourceSizeYInPixels);
                            height = (Int32)sourceSizeYInPixels;
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    // Refresh image size.
                    Int64 width = -46L;
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "WidthMax", MIL.M_TYPE_INT64, ref width);
                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_SOURCE_SIZE_X, width);

                    checked
                    {
                        Width = (Int32)width;
                    }
                    Height = height;
                }

                return new Point(Width, Height);
            }
        }

        public override void Dispose()
        {
            _cTempMonitoring.Cancel();

            try
            {
                _temperatureMonitoringTask.Wait();
            }
            catch (AggregateException) { }

            base.Dispose();
        }

        public override void Init(MilSystem milSystem, int digNumber)
        {
            VieworksVtMatroxCameraConfig config = (VieworksVtMatroxCameraConfig)Config;
            string dcfFileName = (config.CameraMode == CameraMode.tdi) ? "DefaultLineScan.dcf" : "DefaultFrameScan.dcf";

            lock (DigitizerId_lock)
            {
                base.Init(milSystem, digNumber);
                if (!IsSimulated)
                {
                    MIL.MdigAlloc(milSystem, digNumber, Path.Combine(config.DcfFolderPath, dcfFileName), MIL.M_DEFAULT, ref DigitizerId_lock.Value);

                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "PixelFormat", MIL.M_TYPE_STRING,
                       (config.Depth == 12) ? "Mono12" : "Mono8");

                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TestPattern", MIL.M_TYPE_STRING, "Off"/*"GreyHorizontalRamp"*/);
                }
            }

            // Init image size.
            SetMode(config.CameraMode);

            // Allocate buffers.
            AllocateGrabBuffers(config.GlobalBuffersSize_bytes / ImageSize_bytes);

            // Monitor temperature.
            _temperatureMonitoringTask = TemperatureMonitoringAsync();
        }
    }
}
