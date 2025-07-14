using System.Data;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    public abstract class DataInput<T>
    {
        /// <summary>
        /// Connects to the hardware and runs initialization.
        /// throws CommunicationException.
        /// </summary>
        public abstract Task ConnectAndInitAsync();

        public ILogger ThreadSafeLogger = new NullLogger();

        /// <summary>
        /// Starts the acquision: makes sure that triggers can safely be started.
        /// throws CommunicationException in case of an issue with the device (which should be disconnected).
        /// await AcquisitionAsync_() to get notified of acquisition ending (either by itself -fixed frames count reached or error-, or because RequestStop_ts() has been called).
        /// </summary>
        protected abstract Task StartAcquisitionAsync_();

        /// <summary>
        /// Waits for the acquisition to end (either by itself -fixed frames count reached or error-, or because RequestStop_ts() has been called).
        /// Does not dispose BackgroundTargetStream!
        /// throws DataException (containing any exception concerning BackgroundTargetStream that occured since Start(...)).
        /// throws CommunicationException in case of an issue with the device (which should be disconnected).
        /// This is guaranteed to be called just after StartAcquisitionAsync_().
        /// </summary>
        protected abstract Task AcquisitionAsync_();

        /// <summary>
        /// Starts the acquision: makes sure that triggers can safely be started.
        /// throws CommunicationException in case of an issue with the device (which HAS BEEN disconnected).
        /// await AcquisitionAsync() to get notified of acquisition ending (either by itself -fixed frames count reached or error-, or because RequestStop_ts() has been called).
        /// </summary>
        public async Task StartAcquisitionAsync()
        {
            try
            {
                //.CommunicationException
                await StartAcquisitionAsync_().ConfigureAwait(false);

                //xDataException concerns BackgroundTargetStream, which cannot be used by the synchroneous part of AcquisitionAsync_() (as it is "background").
                //.CommunicationException (the acquisition has not even started (the synchroneous part of AcquisitionAsync_() has thrown an exception).
                _acquisitionTask = AcquisitionAsync_();
            }
            catch (CommunicationException)
            {
                //>CommunicationException
                await DisconnectAsync().ConfigureAwait(false);

                throw;
            }
        }

        private Task _acquisitionTask;

        /// <summary>
        /// Waits for the acquisition to end (either by itself -fixed frames count reached or error-, or because RequestStop_ts() has been called),
        ///  AND for BackgroundTargetStream to be disposed.
        /// throws DataException (containing any exception concerning BackgroundTargetStream that occured since Start(...)).
        /// throws CommunicationException in case of an issue with the device (which HAS BEEN disconnected).
        /// </summary>
        public async Task AcquisitionAsync()
        {
            bool communicationException = false;

            try
            {
                //>DataException
                //.CommunicationException
                await _acquisitionTask.ConfigureAwait(false);
            }
            catch (CommunicationException)
            {
                communicationException = true;

                //>CommunicationException
                await DisconnectAsync().ConfigureAwait(false);

                throw;
            }
            finally
            {
                try
                {
                    //.DataException
                    await BackgroundTargetStream.DisposeAsync().ConfigureAwait(false);
                }
                catch (DataException)
                {
                    if (!communicationException)
                    {
                        throw;
                    }// CommunicationExceptions have priority.
                }

                ThreadSafeLogger.Information("Acquisition outputs flushed.");
            }
        }

        /// <summary>
        /// Requests the acquisision to end. Thread safe.
        /// await AcquisitionAsync() to be notified of the acquisition ending.
        /// Some cameras (like Matrox cameras) may drop any image eventually following the request,
        ///  while others (like the CyberOptics 3d) will make sure all images have been written to the target stream before AcquisitionAsync() returns.
        /// </summary>
        public abstract void RequestStop_ts();

        /// <summary>
        /// Where to send the incoming data.
        /// Caution, during acquisition, calls will be made from a background thread!
        /// </summary>
        public DataInputStream<T> BackgroundTargetStream;

        /// <summary>
        /// Input mode.
        /// Set throws NotSupportedException in case of a mode that is not available.
        /// </summary>
        public DataInputMode InputMode;

        /// <summary>
        /// Disconnects from the hardware, and frees up resources.
        /// throws CommunicationException.
        /// </summary>
        public abstract Task DisconnectAsync();
    }
}