using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;

using static UnitySC.PM.Shared.Hardware.Camera.MatroxCamera.MatroxCameraConfigBase;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class VC151MXCamera : MatroxCameraBase
    {
        private const double FramerateCap = 100;
        private const int MinBufferSize = 2;

        public override string SerialNumber => MdigInquireStringFeature("DeviceSerialNumber");

        public VC151MXCamera(MatroxCameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        public override void Init(MilSystem milSystem, int devNumber)
        {
            Invoke(() =>
            {
                // Init de base
                //.............
                base.Init(milSystem, devNumber);

                // Init MIL
                //.........
                lock (DigitizerId_lock)
                {
                    MIL.MdigAlloc(milSystem, devNumber, "M_DEFAULT", MIL.M_DEFAULT, ref DigitizerId_lock.Value);

                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_GRAB_TIMEOUT, Config.GrabTimeout * 1000);
                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_CORRUPTED_FRAME_ERROR, MIL.M_ENABLE);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "Timed");

                    ColorModes = GetColorModes();

                    SetPixelFormat(Config.Depth);

                    SetGain(Config.Gain);

                    // Lecture des infos de la caméra
                    //...............................
                    Model = MdigInquireStringFeature("DeviceModelName");
                    // SerialNumber = MdigInquireStringFeature("DeviceSerialNumber"); on ne le lit pas maintenant, pour pouvoir hériter de cette classe
                    Version = MdigInquireStringFeature("DeviceVersion");
                    MaxExposureTimeMs = MdigInquireDoubleFeature("ExposureTime", MIL.M_FEATURE_MAX) / 1000.0;
                    MinExposureTimeMs = MdigInquireDoubleFeature("ExposureTime", MIL.M_FEATURE_MIN) / 1000.0;
                    MaxFrameRate = MdigInquireDoubleFeature("AcquisitionFrameRate", MIL.M_FEATURE_MAX);
                    MinFrameRate = MdigInquireDoubleFeature("AcquisitionFrameRate", MIL.M_FEATURE_MIN);
                    MaxGain = MdigInquireDoubleFeature("Gain", MIL.M_FEATURE_MAX);
                    MinGain = MdigInquireDoubleFeature("Gain", MIL.M_FEATURE_MIN);
                    MaxWidth = (int)MdigInquireInt64Feature("WidthMax", MIL.M_FEATURE_VALUE);
                    MaxHeight = (int)MdigInquireInt64Feature("HeightMax", MIL.M_FEATURE_VALUE);
                    MinWidth = (int)MdigInquireInt64Feature("Width", MIL.M_FEATURE_MIN);
                    MinHeight = (int)MdigInquireInt64Feature("Height", MIL.M_FEATURE_MIN);
                    WidthIncrement = (int)MdigInquireInt64Feature("Width", MIL.M_FEATURE_INCREMENT);
                    HeightIncrement = (int)MdigInquireInt64Feature("Height", MIL.M_FEATURE_INCREMENT);

                    PixelDepthBuffer = CalculatePixelDepthBufferBytes(Config.Depth);

                    Width = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_X);
                    Height = (int)MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_SIZE_Y);

                    SetExposureTimeMs(MinExposureTimeMs);
                    SetFrameRate(MaxFrameRate);

                    int buffersToAllocate = CalculateOptimalBufferAllocation();
                    AllocateGrabBuffers(buffersToAllocate);
                }

                State = new DeviceState(DeviceStatus.Ready);
            });
        }

        public override void InitSettings()
        {
            SetAOI(Config.AOI);
            SetDefectivePixelCorrection(Config.DefectivePixelCorrection);
            SetDynamicDefectivePixelCorrection(Config.DynamicDefectivePixelCorrection);
            SetBlackLevel(Config.BlackLevel);
            SetFanOperationMode(Config.FanOperation);
            SetHotPixelCorrection(Config.HotPixel);
            SetFlatFieldCorrection(Config.FlatField);
        }

        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            Invoke(() =>
            {
                Logger.Information($"{Name} ExposureTime:{exposureTime_ms}");
                double exposureTimeUs = exposureTime_ms * 1000;
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "ExposureTime", MIL.M_TYPE_DOUBLE, ref exposureTimeUs);
                    MaxFrameRate = MdigInquireDoubleFeature("AcquisitionFrameRate", MIL.M_FEATURE_MAX);
                    MinFrameRate = MdigInquireDoubleFeature("AcquisitionFrameRate", MIL.M_FEATURE_MIN);
                }
            });
        }

        public override double GetExposureTimeMs()
        {
            return Invoke(() =>
            {
                double exposureTimeUs = MdigInquireDoubleFeature("ExposureTime");
                return exposureTimeUs / 1000;
            });
        }

        public override void SetGain(double gain)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "DigitalAll");
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref gain);
                }
            });
        }

        public override double GetGain()
        {
            return Invoke(() =>
            {
                double gain = double.NaN;
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "GainSelector", MIL.M_TYPE_STRING, "DigitalAll");
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref gain);
                }
                return gain;
            });
        }

        private void SetPixelFormat(int depth)
        {
            string colorMode = GetPixelFormat(depth).ToString();
            if (!ColorModes.Contains(colorMode))
            {
                throw new ApplicationException("Unknown color mode: " + colorMode);
            }
            lock (DigitizerId_lock)
            {
                MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "PixelFormat", MIL.M_TYPE_STRING, colorMode);
            }
        }

        public override void SetTriggerMode(TriggerMode mode)
        {
            Invoke(() =>
            {
                long state = 0xFDFD;
                lock (DigitizerId_lock)
                {
                    MIL.MdigInquire(DigitizerId_lock.Value, MIL.M_GRAB_TRIGGER_STATE, ref state);
                }
                if (state != MIL.M_DISABLE)
                    throw new ApplicationException("Trigger already set");

                string enable;
                string source;

                switch (mode)
                {
                    case TriggerMode.Off:
                        enable = "Off";
                        source = null;
                        state = MIL.M_DISABLE;
                        break;

                    case TriggerMode.Software:
                        enable = "On";
                        source = "Software";
                        state = MIL.M_ENABLE;
                        break;

                    case TriggerMode.Hardware:
                        enable = "On";
                        source = "External";
                        state = MIL.M_ENABLE;
                        break;

                    default:
                        throw new ApplicationException("unknown trigger mode: " + mode);
                }

                Logger.Debug("Trigger mode: " + (source ?? enable));
                lock (DigitizerId_lock)
                {
                    // The acquisitionStop is added because sometimes the Set TriggerMode fails
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_EXECUTE, "AcquisitionStop", MIL.M_TYPE_COMMAND, MIL.M_NULL);
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "ExposureStart");
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, enable);
                    if (source != null)
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, source);
                }
            });
        }

        public override void SoftwareTrigger()
        {
            Invoke(() =>
            {
                Logger.Debug("SoftwareTrigger Waiting Unlock");
                GrabEvent.Reset();

                // La version "normale" fonctionne avec la caméra 29MPix mais pas avec la 155MPix
                //MIL.MdigControl(DigitizerId, MIL.M_GRAB_TRIGGER_SOFTWARE, 1);
                // Équivalent avec les features:
                lock (DigitizerId_lock)
                {
                    Logger.Debug("SoftwareTrigger");
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_EXECUTE, "TriggerSoftware", MIL.M_TYPE_COMMAND, MIL.M_NULL);
                }
            });
        }

        public override void SetAOI(Rect aoi)
        {
            FieldOfView fov = new FieldOfView()
            {
                Width = (int)aoi.Width,
                Height = (int)aoi.Height,
                OffsetX = (int)aoi.X,
                OffsetY = (int)aoi.Y
            };
            SetFOV(fov);
        }

        public override void SetColorMode(string colorMode)
        {
            Invoke(() =>
            {
                InvokeWithStopGrab(() =>
                {
                    int depth = GetPixelDepth(colorMode);
                    SetPixelFormat(depth);
                    PixelDepthBuffer = CalculatePixelDepthBufferBytes(depth);
                    SetImageSizeWithDepth(depth);
                    int buffersToAllocate = CalculateOptimalBufferAllocation();
                    AllocateGrabBuffers(buffersToAllocate);
                });
            });
        }

        public override List<string> GetColorModes()
        {
            var colorModes = new List<string>();
            lock (DigitizerId_lock)
            {
                long pixelFormatStringSize = 0;
                MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_ENUM_ENTRY_COUNT, "PixelFormat", MIL.M_TYPE_MIL_INT, ref pixelFormatStringSize);

                for (int i = 0; i < pixelFormatStringSize; i++)
                {
                    var pixelFormat = new StringBuilder();
                    MIL.MdigInquireFeature(DigitizerId_lock.Value, MIL.M_FEATURE_ENUM_ENTRY_NAME + i, "PixelFormat", MIL.M_TYPE_STRING, pixelFormat);
                    if (Enum.IsDefined(typeof(PixelFormat), pixelFormat.ToString()))
                    {
                        colorModes.Add(pixelFormat.ToString());
                    }
                }
            }
            return colorModes;
        }

        public int CalculateOptimalBufferAllocation()
        {
            return InvokeWithStopGrab<int>(() =>
            {
                var savedExposureTime = GetExposureTimeMs();
                SetExposureTimeMs(MinExposureTimeMs);

                try
                {
                    MaxFrameRate = MdigInquireDoubleFeature("AcquisitionFrameRate", MIL.M_FEATURE_MAX);
                    double cappedFramerate = Math.Min(MaxFrameRate, FramerateCap);

                    lock (DigitizerId_lock)
                    {
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "AcquisitionFrameRate", MIL.M_TYPE_MIL_DOUBLE, ref cappedFramerate);
                    }

                    //Disposing before reallocating the new buffers
                    if (_grabImages != null)
                    {
                        for (int i = 0; i < _grabImages.Count(); i++)
                            _grabImages[i].Dispose();
                        _grabImages = null;
                    }
                    string pixelFormat = MdigInquireStringFeature("PixelFormat");
                    int bitsPerPixel = GetPixelDepth(pixelFormat);
                    long bytesPerBuffer = ((long)bitsPerPixel * Width * Height) / 8;
                    long freeBytes = MIL.MappInquire(MIL.M_DEFAULT, MIL.M_NON_PAGED_MEMORY_FREE);
                    int freeBuffers = (int)Math.Floor((double)freeBytes / bytesPerBuffer);
                    int buffersToAllocate = Math.Min((int)cappedFramerate * 2, freeBuffers);
                    buffersToAllocate = Math.Max(buffersToAllocate, MinBufferSize);

                    return buffersToAllocate;
                }
                catch (Exception ex)
                {
                    Logger.Error($"[CalculateOptimalBufferAllocation] An exception occurred: " + ex.Message);
                    return 20; //default value
                }
                finally
                {
                    SetExposureTimeMs(savedExposureTime);
                }
            });          
        }

        private static int GetPixelDepth(string colorMode)
        {
            if (!Enum.TryParse(colorMode, out PixelFormat pixelFormat))
            {
                throw new ApplicationException("Unkown color mode: " + colorMode);
            }

            return (int)pixelFormat;
        }

        private PixelFormat GetPixelFormat(int depth)
        {
            var pixelFormat = (PixelFormat)depth;
            if (!Enum.IsDefined(typeof(PixelFormat), pixelFormat))
            {
                throw new ApplicationException("Unsupported pixel depth: " + depth);
            }
            return pixelFormat;
        }

        public void SetFOV(FieldOfView fov)
        {
            Invoke(() =>
            {
                InvokeWithStopGrab(() =>
                {
                    // On resette l'offset pour être sûr que le width/height sera valide
                    long zero = 0;
                    lock (DigitizerId_lock)
                    {
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetX", MIL.M_TYPE_MIL_INT32, ref zero);
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetY", MIL.M_TYPE_MIL_INT32, ref zero);

                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Width", MIL.M_TYPE_MIL_INT32, ref fov.Width);
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "Height", MIL.M_TYPE_MIL_INT32, ref fov.Height);
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetX", MIL.M_TYPE_MIL_INT32, ref fov.OffsetX);
                        MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "OffsetY", MIL.M_TYPE_MIL_INT32, ref fov.OffsetY);
                    }

                    Width = (int)fov.Width;
                    Height = (int)fov.Height;
                    int buffersToAllocate = CalculateOptimalBufferAllocation();
                    AllocateGrabBuffers(buffersToAllocate);
                });
            });
        }

        public void SetDefectivePixelCorrection(bool defectivePixelCorrection)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "DefectivePixelCorrection", MIL.M_TYPE_BOOLEAN, ref defectivePixelCorrection);
                }
            });
        }

        public void SetDynamicDefectivePixelCorrection(bool dynamicDefectivePixelCorrection)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "DynamicDefectivePixelCorrection", MIL.M_TYPE_BOOLEAN, ref dynamicDefectivePixelCorrection);
                }
            });
        }

        public void SetBlackLevel(double blackLevel)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "BlackLevel", MIL.M_TYPE_DOUBLE, ref blackLevel);
                }
            });
        }

        public void SetFanOperationMode(FanOperationMode fanOperationMode)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "FanOperationMode", MIL.M_TYPE_STRING, fanOperationMode.ToString());
                }
            });
        }

        public void SetHotPixelCorrection(HotPixelCorrection hotPixelCorrection)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "HotPixelCorrection", MIL.M_TYPE_STRING, hotPixelCorrection.ToString());
                }
            });
        }

        public void SetFlatFieldCorrection(FlatFieldCorrection flatFieldCorrection)
        {
            Invoke(() =>
            {
                lock (DigitizerId_lock)
                {
                    MIL.MdigControlFeature(DigitizerId_lock.Value, MIL.M_FEATURE_VALUE, "FlatFieldCorrection", MIL.M_TYPE_STRING, flatFieldCorrection.ToString());
                }
            });
        }
    }
}
