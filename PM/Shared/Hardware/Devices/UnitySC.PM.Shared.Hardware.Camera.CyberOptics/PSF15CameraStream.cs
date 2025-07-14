using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Camera.DataInput;
using UnitySC.Shared.Ext;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera.CyberOptics
{
    /// <summary>
    /// This camera can only be software triggered.
    /// If PSF15CameraConfig.ContinuouslyTriggering, use the following sequence for acquisition:
    ///  await StartAcquisitionAsync();
    ///  await AcquisitionAsync();  // Call RequestStop_ts() when you want to stop.
    /// If not PSF15CameraConfig.ContinuouslyTriggering, use the following sequence for acquisition:
    ///  await StartAcquisitionAsync();
    ///  await SoftwareTriggerAsync(); // Table may move immediately after that call, as illumination is over. The actual data will be written to the target stream later on -pipelining-.
    ///  // Move table
    ///  // ... repeat await SoftwareTriggerAsync() + move table as long as necessary.
    ///  RequestStop_ts();  // Only when all images habe been acquired -illumination phase-.
    ///  await AcquisitionAsync(); // To wait for the last images to have finished the processing pipeline and the corresponding stream writes to have been called.
    /// </summary>
    public class PSF15CameraStream : DataInputFromCameraBase<PSF15CameraData, PSF15CameraData>
    {
        protected override void BackgroundWriteToTargetStream(PSF15CameraData message)
        {
            //>DataException
            BackgroundTargetStream.Write(message);
        }

        /// <summary>
        /// Runs field calibration.
        /// True => field calibration ok, will be applied on all future acquisitions.
        /// False => target is not valid for field calibration, field calibration has been disabled (restored to factory values).
        /// </summary>
        public async Task<bool> PerformFieldCalibrationAsync()
        {
            await ThreadPoolTools.Post;
            return ((PSF15Camera)CameraBase).PerformFieldCalibration();
        }

        protected override Task StartAcquisitionAsync_()
        {
            ((PSF15CameraConfig)(CameraBase.Config)).ContinuouslyTriggering = (InputMode == DataInputMode.continuous);

            //>CommunicationException
            return base.StartAcquisitionAsync_();
        }
    }
}