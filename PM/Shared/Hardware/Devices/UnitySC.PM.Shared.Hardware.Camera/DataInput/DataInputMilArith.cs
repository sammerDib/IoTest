using System;
using System.Drawing;
using System.Threading.Tasks;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    public class DataInputMilArith :
        DataInputAsync<MilImage>
    {
        public async Task DisposeAsync()
        {
            await Target?.DisposeAsync();
            _targetBuffer.Dispose();
        }

        public async Task WriteAsync(Point point, MilImage data)
        {
            await ThreadPoolTools.Post;

            if (_targetBuffer == null)
            {
                _targetBuffer = data.Clone();
            }

            MilImage.Arith(data, SecondParameter, _targetBuffer, Operation_s64);

            await Target.WriteAsync(point, _targetBuffer).ConfigureAwait(false);
        }

        /// <summary>
        /// Arithmetic operation is TargetBuffer = Operation_s64([inputImage], SecondParameter).
        /// </summary>
        public Int64 Operation_s64 = MIL.M_DIV_CONST;

        public double SecondParameter = 1d;

        /// <summary>
        /// Target for processed data.
        /// </summary>
        public DataInputAsync<MilImage> Target;

        /// <summary>
        /// This buffer will be updated on each WriteAsync(...), and sent to TargetStream.
        /// Will be set to a clone of the first incoming data.
        /// </summary>
        private MilImage _targetBuffer;
    }
}
