using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using CO.Psx.Engine;
using CO.Psx.Engine.PsxTcpClientServer;
using CO.Psx.Sensor;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera.CyberOptics
{
    /// <summary>
    /// CyberOptics PSF15 3d sensor.
    /// Register on Messenger for PSF15Data type.
    ///  PSF15Data is part of the camera buffers, and can only be used in the Messenger callback: no reference should be kept.
    ///  Use CyberImage.Copy() if you need to keep a reference to the acquired data out of the Messenger callback scope!
    /// Since the camera only supports software triggers, no error notification is being sent on the messenger:
    ///  any error will be simply forwarded as exception with the next call to this object.
    /// </summary>
    public class PSF15Camera : USPCameraBase
    {
        public PSF15Camera(PSF15CameraConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
            _illuminationTask.TrySetResult(true);
        }

        public override double GetExposureTimeMs()
        {
            throw new NotImplementedException("Only illumination intensity can be set up!");
        }

        public override double GetGain()
        {
            return 1d;
        }

        public override void SetExposureTimeMs(double exposureTime)
        {
            throw new NotImplementedException("Only illumination intensity can be set up!");
        }

        public override void SetGain(double gain)
        {
            throw new NotImplementedException();
        }

        public override void SetTriggerMode(TriggerMode mode)
        {
            throw new NotImplementedException("Only software triggers are supported: the table waits for each illumination before moving.");
        }

        public new PSF15CameraConfig Config => (PSF15CameraConfig)(base.Config);

        /// <summary>
        /// Set in case of an error with the camera.
        /// Shutdown() should be called.
        /// sync through _psxEngine_lock.
        /// </summary>
        private CommunicationException _cameraError_sync;

        public override void Init()
        {
            base.Init();

            PsxDiagnostics.Instance.PsxMessageAvailable += (object sender, PsxDiagnosticsEventArgs e) =>
            {
                // TODO sde logger.
            };
            PsxDiagnostics.Instance.PsxUnhandledExceptionOccurred += (object sender, PsxUnhandledExceptionEventArgs e) =>
            {
                // Report the error to the different listeners (includes buffer underrun errors).
                lock (_psxEngine_lock)
                {
                    if (_cameraError_sync != null)
                    {
                        return; // Only forward the first exception.
                    }

                    if (e.Exception is CommunicationException)
                    {
                        _cameraError_sync = (CommunicationException)(e.Exception);
                    }
                    else
                    {
                        _cameraError_sync = new CommunicationException("", e.Exception);
                    }

                    // Free anybody waiting, so that he gets the exception.
                    _nbImagesInProcessingStack_sync = 0;
                    NotifyImageSentListeners_ts();

                    _illuminationTask.TrySetResult(false);
                }
            };
            PsxDiagnostics.Instance.EnableAllMessageTypes(true);

            _psxEngine_lock = new PsxTcpEngine(Config.TcpServerAddress, Config.TcpServerPort, false);
            lock (_psxEngine_lock)
            {
                _psxEngine_lock.Initialize(DeviceType.RealCyberPfsDevice);
                _helper = new PFSHelper(_psxEngine_lock.PsxDevice);

                PsxSettings newSettings = new PsxSettings(DeviceType.RealCyberPfsDevice); // Create a new settings object, using the device type as an argument for default values
                newSettings.Copy(_psxEngine_lock.PsxDevice.PsxSettings); // Copy the existing settings values to the new object

                newSettings.TCPTransferMode.Value = Config.FetchRawImages ? PsxTcpImageTypes.ProcessedGroupsRaws : PsxTcpImageTypes.Processed;
                newSettings.PFSChannelGroupCombinationTechnique.Value = ChannelGroupCombinationTechnique.PointCloudMerge;
                newSettings.OptimizeSiteIlluminationSpecs.Value = true;

                _psxEngine_lock.UpdatePsxSettings(newSettings); // Apply the settings to the engine

                // Find the led rings.
                IList<PsxIlluminator> ledRings = _psxEngine_lock.PsxDevice.AvailableIlluminators;
                foreach (PsxIlluminator illuminator in ledRings)
                {
                    if (illuminator.Name == "RinglightLow")
                    {
                        _ledRingLow = illuminator;
                    }
                    else if (illuminator.Name == "RinglightHigh")
                    {
                        _ledRingHigh = illuminator;
                    }
                }

                // Listen to events.
                _psxEngine_lock.SiteCaptureComplete += (object sender, SiteCaptureCompleteEventArgs e) =>
                {
                    // Illumination is complete, ready to move the table.
                    _illuminationTask.TrySetResult(true);
                };

                _psxEngine_lock.RawImageProcessingComplete += (object sender, RawImageProcessingCompleteEventArgs images) =>
                {
                    lock (_psxEngine_lock)
                    {
                        if (_cameraError_sync != null)
                        {
                            return; // Don't send any image if an error has been detected.
                        }

                        Messenger.Send<PSF15CameraData>(new PSF15CameraData() { Data = images.PsxSiteImageStack });

                        // Notify the camera that the buffers may be reused.
                        _psxEngine_lock.ReleaseImageStack(images.PsxSiteImageStack);
                        _nbImagesInProcessingStack_sync--;

                        NotifyImageSentListeners_ts();
                    }
                };
            }
        }

        /// <summary>
        /// LED rings.
        /// </summary>
        private PsxIlluminator _ledRingLow;

        private PsxIlluminator _ledRingHigh;

        /// <summary>
        /// Set each time acquired data has been sent through the messenger and given back as available buffer to the camera.
        /// Sync through _psxEngine_lock.
        /// </summary>
        private readonly List<ManualResetEvent> _cImageSentToMessengerListeners_sync = new List<ManualResetEvent>();

        private void NotifyImageSentListeners_ts()
        {
            lock (_psxEngine_lock)
            {
                foreach (ManualResetEvent mre in _cImageSentToMessengerListeners_sync)
                {
                    mre.Set();
                }
                _cImageSentToMessengerListeners_sync.Clear();
            }
        }

        /// <summary>
        /// Number of images in the processing pipeline (trigger started, but image not yet through the messenger).
        /// Sync through _psxEngine_lock.
        /// </summary>
        private Int32 _nbImagesInProcessingStack_sync;

        /// <summary>
        /// Access to the CyberOptics API.
        /// </summary>
        private PsxTcpEngine _psxEngine_lock;

        /// <summary>
        /// throws CommunicationException
        /// </summary>
        public override void Shutdown()
        {
            lock (_psxEngine_lock)
            {
                _psxEngine_lock.Terminate();
                _psxEngine_lock.Dispose();

                if (_cameraError_sync != null)
                {
                    throw _cameraError_sync;    // Notify if an error was detected just before shutdown.
                }
            }
        }

        public override USPImage SingleGrab()
        {
            throw new NotImplementedException("This camera does not support USPImage. Use StartContinousGrab(), listen to the Messenger, and trigger once!");
        }

        /// <summary>
        /// The table can move as soon as the illumination is complete, even if the following steps in the processing pipeline
        ///  (Initial height reconstruction and 2D processing -> Height reconstruction combination -> TCP Transfer) are not complete yet.
        /// _cameraError_sync has to be checked after waiting for _illuminationTask.
        /// </summary>
        private TaskCompletionSource<bool> _illuminationTask = new TaskCompletionSource<bool>();

        /// <summary>
        /// throws CommunicationException
        /// </summary>
        public override void SoftwareTrigger()
        {
            // Never start a new illumination before the previous one is complete.
            _illuminationTask.Task.Wait();
            _illuminationTask = new TaskCompletionSource<bool>();

            lock (_psxEngine_lock)
            {
                if (_cameraError_sync != null)
                {
                    throw _cameraError_sync;
                }

                _nbImagesInProcessingStack_sync++;

                _psxEngine_lock.AcquireSite(_siteSpec_sync);
                _psxEngine_lock.Trigger();
            }
        }

        /// <summary>
        /// Runs field calibration.
        /// True => field calibration ok, will be applied on all future acquisitions.
        /// False => target is not valid for field calibration, field calibration has been disabled (restored to factory values).
        /// </summary>
        public bool PerformFieldCalibration()
        {
            lock (_psxEngine_lock)
            {
                return _psxEngine_lock.PerformFieldCalibration(true);
            }
        }

        /// <summary>
        /// Last settings used.
        /// Sync through _psxEngine_lock.
        /// </summary>
        private SiteSpec _siteSpec_sync;

        private PFSHelper _helper;

        /// <summary>
        /// Acquisition -> Initial height reconstruction and 2D processing -> Height reconstruction combination -> TCP Transfer
        /// </summary>
        private const Int32 CPipelineSize = 4;

        public override async void StartContinuousGrab()
        {
            _stopRequested_ts = false;

            lock (_psxEngine_lock)
            {
                // 3D.
                _siteSpec_sync = _helper.CreateHeightReconstructionSiteSpec(
                    (Config.SideIntensity_percent <= 0f) ? -1 : (Int32)(Config.WrapLength_um), HeightImageAlgorithmOption.Option_3_4, Config.SideIntensity_percent,
                    (Config.TopIntensity_percent <= 0f) ? -1 : (Int32)(Config.WrapLength_um), HeightImageAlgorithmOption.Option_3_4, Config.TopIntensity_percent,
                    (Config.SpecularIntensity_percent <= 0f) ? -1 : ((Int32)(Config.WrapLength_um)) >> 1, HeightImageAlgorithmOption.Option_3_4, Config.SpecularIntensity_percent);

                // 2D.
                if (Config.Illuminate2DImage)
                {
                    ChannelSpec twoDChannel = new ChannelSpec(_psxEngine_lock.PsxDevice, ChannelSpecDefault.Empty);
                    twoDChannel.AddChannel(_psxEngine_lock.PsxDevice.GetChannelForCameraAndDLPAzimuth(-1, -1));
                    IlluminationSpec twoDIllumination = new IlluminationSpec(twoDChannel);

                    if (Config.TwoDIntensityProjector_percent > 0f)
                    {
                        twoDIllumination.SetDlpIllumination(_psxEngine_lock.PsxDevice.DlpImageList.GetDlpImage("white"), twoDChannel);
                        twoDIllumination.DlpIntensity = Config.TwoDIntensityProjector_percent;
                    }

                    if (Config.TwoDIntensityLEDRingLow_percent > 0)
                    {
                        twoDIllumination.SetIllumination(_ledRingLow, Config.TwoDIntensityLEDRingLow_percent);
                    }
                    if (Config.TwoDIntensityLEDRingHigh_percent > 0)
                    {
                        twoDIllumination.SetIllumination(_ledRingHigh, Config.TwoDIntensityLEDRingHigh_percent);
                    }

                    _siteSpec_sync.AddIlluminationSpec(twoDIllumination);
                }
            }

            if (Config.ContinuouslyTriggering)
            {
                _START:
                // Simulate SoftwareTrigger() calls.
                await ThreadPoolTools.Post;

                Task task;
                lock (_psxEngine_lock)
                {
                    if (_stopRequested_ts || (_cameraError_sync != null))
                    {
                        // Stop sending triggers.
                        return;
                    }

                    if (_nbImagesInProcessingStack_sync >= (CPipelineSize + 1))
                    {
                        // No need to fill the pipeline more: wait for one pic to come out before triggering a new illumination again.
                        ManualResetEvent mre = new ManualResetEvent(false);
                        _cImageSentToMessengerListeners_sync.Add(mre);
                        task = mre.cAsTask_ts();
                    }
                    else
                    {
                        // Start a new illumination.
                        _nbImagesInProcessingStack_sync++;

                        _illuminationTask = new TaskCompletionSource<bool>();
                        task = _illuminationTask.Task;

                        _psxEngine_lock.AcquireSite(_siteSpec_sync);
                        _psxEngine_lock.Trigger();
                    }
                }

                await task.ConfigureAwait(false);

                goto _START;
            }
        }

        /// <summary>
        /// Used to stop auto-triggering upon StopContinuousGrab();
        /// </summary>
        private volatile bool _stopRequested_ts;

        /// <summary>
        /// In order to stop acquisition, stop calling SoftwareTrigger() and call this function to wait for the last image to have been sent through the Messenger.
        /// throws CommunicationException
        /// </summary>
        public override void StopContinuousGrab()
        {
            _stopRequested_ts = true;
            ManualResetEvent mre;

            _START:
            // Wait for the last image to have gone through the processing pipeline.
            lock (_psxEngine_lock)
            {
                if (_cameraError_sync != null)
                {
                    throw _cameraError_sync;
                }

                if (_nbImagesInProcessingStack_sync == 0)
                {
                    return;
                }

                mre = new ManualResetEvent(false);
                _cImageSentToMessengerListeners_sync.Add(mre);
            }

            // Wait for the next image going through the messenger.
            mre.WaitOne();
            goto _START;
        }

        /// <summary>
        /// Wait for this before moving the table to the next position.
        /// throws CommunicationException
        /// The table can move as soon as the immumination is complete,
        ///  even if the following steps (Initial height reconstruction and 2D processing -> Height reconstruction combination -> TCP Transfer) are not complete yet -pipelining-.
        /// </summary>
        public override void WaitForSoftwareTriggerGrabbed()
        {
            _illuminationTask.Task.Wait();

            lock (_psxEngine_lock)
            {
                if (_cameraError_sync != null)
                {
                    throw _cameraError_sync;
                }
            }
        }
    }
}
