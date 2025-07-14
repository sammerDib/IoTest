using UnitySC.PM.Shared.Hardware.Camera.DataInput;

namespace UnitySC.PM.Shared.Hardware.Camera.CyberOptics
{
    /// <summary>
    /// Buffers one site. Does not support resolution changes.
    /// </summary>
    public class PSF15CameraBufferCopy : DataInputStreamToAsync<PSF15CameraData, PSF15CameraImages>
    {
        private PSF15CameraImages _buffer;

        protected override PSF15CameraImages CloneOrReuseClone(PSF15CameraData data)
        {
            _buffer.CopyFrom(data);
            return _buffer;
        }
    }
}