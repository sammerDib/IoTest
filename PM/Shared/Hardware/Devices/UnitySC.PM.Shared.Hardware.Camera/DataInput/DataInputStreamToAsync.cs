using System.Data;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    /// <summary>
    /// Copies data to a temporary buffer (fast), and lets a slower target process the copy.
    /// Drops frames coming too fast for the target.
    /// TODO sde this could be tranformed into a buffering class, where a buffer size of 1 would correspond to the current behaviour.
    /// </summary>
    public abstract class DataInputStreamToAsync<T, A> : DataInputStream<T>
    {
        public virtual async Task DisposeAsync()
        {
            //>DataException
            await _slowerTask;
            await Target.DisposeAsync();
        }

        public virtual void Write(System.Drawing.Point point, T data)
        {
            if (_slowerTask.IsCompleted)
            {
                //>DataException
                _slowerTask = CloneAndProcess(point, data);
            }
            else if (OverflowMode_u8 == OverflowMode.ThrowException)
            {
                throw new DataException("Data coming too fast");
            }// Drop images that come too quickly for the async processing stack.
        }

        /// <summary>
        /// If the data comes in too fast for the async target, Write(...) will
        ///  drop frames (default)
        ///  or
        ///  throw DataException
        /// </summary>
        public enum OverflowMode : byte { Drop = 0, ThrowException };

        public OverflowMode OverflowMode_u8 = OverflowMode.Drop;

        /// <summary>
        /// Runs async processing.
        /// throws DataException
        /// </summary>
        private Task _slowerTask = Task.FromResult(false);

        /// <summary>
        /// Runs async processing.
        /// throws DataException
        /// </summary>
        private Task CloneAndProcess(System.Drawing.Point point, T data)
        {
            // Copy data to the intermediate buffer, as we have to give data back to the acquisition process as soon as possible.

            //>DataException
            return Target.WriteAsync(point, CloneOrReuseClone(data));
        }

        /// <summary>
        /// Target stream that is allowed longer writes.
        /// </summary>
        public DataInputAsync<A> Target;

        /// <summary>
        /// Creates a clone (may reuse a buffer).
        /// Time critical.
        /// </summary>
        protected abstract A CloneOrReuseClone(T data);
    }
}