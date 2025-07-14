using System.Threading.Tasks;

using UnitySC.Shared.LibMIL;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    public class DataInputMilToAsync : DataInputStreamToAsync<MilImage, MilImage>
    {
        protected override MilImage CloneOrReuseClone(MilImage data)
        {
            if (_buffer == null)
            {
                _buffer = data.Clone();
            }
            else
            {
                MilImage.Copy(data, _buffer);
            }

            return _buffer;
        }

        public override async Task DisposeAsync()
        {
            //>DataException
            await base.DisposeAsync();

            _buffer?.Dispose();
            _buffer = null;
        }

        /// <summary>
        /// Temporary buffer.
        /// </summary>
        private MilImage _buffer;
    }
}