using System.Data;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Tools;

using static UnitySC.PM.Shared.Hardware.Camera.CameraBase;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    /// <summary>
    /// Async helper for CameraBase.
    /// T is the data type transmitted to the target stream (ex: MilImage).
    /// M is the message type received from the camera (ex: CameraMessage).
    /// </summary>
    public abstract class DataInputFromCameraBase<T, M> : DataInput<T> where M : class
    {
        /// <summary>
        /// Lower level object.
        /// </summary>
        public CameraBase CameraBase;

        public override async Task ConnectAndInitAsync()
        {
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<M>(this, (r, m) =>
            {
                try
                {
                    //.DataException
                    BackgroundWriteToTargetStream(m);
                }
                catch (DataException ex)
                {
                    _callbackException_ts = ex;
                    RequestStop_ts();
                }
            });

            await ThreadPoolTools.Post;
            CameraBase.Init();
        }

        /// <summary>
        /// Stores any error that occurs during acquisition (and callbacks).
        /// </summary>
        private volatile DataException _callbackException_ts;

        /// <summary>
        /// Writes to the target stream. May be overrided in order to handle error cases (ex: throw DataException in case of a corresponding camera notification).
        /// throws DataException
        /// </summary>
        protected abstract void BackgroundWriteToTargetStream(M message);

        public override async Task DisconnectAsync()
        {
            await ThreadPoolTools.Post;

            //>CommunicationException
            CameraBase.Shutdown();
        }

        public override void RequestStop_ts()
        {
            _waitForStopRequested.TrySetResult(true);
        }

        private TaskCompletionSource<bool> _waitForStopRequested;

        protected override async Task AcquisitionAsync_()
        {
            await _waitForStopRequested.Task.ConfigureAwait(false);
            await ThreadPoolTools.Post;

            //>CommunicationException
            CameraBase.StopContinuousGrab();

            if (_callbackException_ts != null)
            {
                //>DataException
                throw _callbackException_ts;
            }
        }

        public async Task TriggerModeSetAsync(TriggerMode triggerMode)
        {
            await ThreadPoolTools.Post;
            CameraBase.SetTriggerMode(triggerMode);
        }

        protected override async Task StartAcquisitionAsync_()
        {
            _callbackException_ts = null;
            _waitForStopRequested = new TaskCompletionSource<bool>();

            await ThreadPoolTools.Post;
            CameraBase.StartContinuousGrab();
        }

        /// <summary>
        /// Exposure time, in ms.
        /// </summary>
        public async Task<double> ExposureTimeMsGetAsync()
        {
            await ThreadPoolTools.Post;
            return CameraBase.GetExposureTimeMs() * 1000d;
        }

        /// <summary>
        /// Exposure time, in ms.
        /// throws NotSupportedException (call ExposureTimeGet_usAsync() to get the current value):
        ///  In TDI mode, when triggered, the camera exposes continuously, the exposition time per line is the trigger time (linked to table move).
        /// </summary>
        public async Task ExposureTimeMsSetAsync(double exposureTime_ms)
        {
            await ThreadPoolTools.Post;
            CameraBase.SetExposureTimeMs(exposureTime_ms / 1000d);
        }

        /// <summary>
        /// Starts one illumination, and returns once the raw acquisition is ready -and is is ok to move the table-.
        /// throws CommunicationException
        /// The actual image data could be written later on the target stream (due to processing pipelining).
        /// </summary>
        public async Task SoftwareTriggerAsync()
        {
            await ThreadPoolTools.Post;

            //>CommunicationException
            CameraBase.SoftwareTrigger();
            CameraBase.WaitForSoftwareTriggerGrabbed();
        }
    }
}
