using System.Drawing;
using System.Threading.Tasks;

using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    public class DataInputMilCropTo8bits :
         DataInputAsync<MilImage>
    {
        /// <summary>
        /// Target receiving 8 bits images.
        /// </summary>
        public DataInputAsync<MilImage> Target;

        public async Task DisposeAsync()
        {
            //>DataException
            await Target.DisposeAsync();

            _targetBuffer?.Dispose();
        }

        public async Task WriteAsync(Point point, MilImage data)
        {
            await ThreadPoolTools.Post;

            // Crop to 8 bits.
            data.CropTo8bits(ref _targetBuffer);

            //>DataException
            await Target.WriteAsync(point, _targetBuffer).ConfigureAwait(false);
        }

        /// <summary>
        /// This buffer will be updated on each WriteAsync(...), and sent to TargetStream.
        /// Will be set to a clone of the first incoming data.
        /// </summary>
        private MilImage _targetBuffer;
    }
}