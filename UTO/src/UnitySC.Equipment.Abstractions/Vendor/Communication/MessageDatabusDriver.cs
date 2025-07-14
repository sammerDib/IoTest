using System.Timers;

using Agileo.MessageDataBus;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{

    /// <summary>
    /// This a tag based driver that integrates the message data bus.
    /// </summary>
    public abstract class MessageDatabusDriver : Driver
    {
        protected MessageDataBus Tags { get; private set; }

        public override bool IsCommunicationEnabled => Tags.IsDriverConnected(_mdbDriverName);

        /// <summary>
        /// The timer used to poll tag values.
        /// </summary>
        private Timer _pollingTimer;

        /// <summary>
        /// The elapsing time in milliseconds between two polling requests.
        /// </summary>
        private int _timing;

        /// <summary>
        /// The name of the driver used by the MDB
        /// </summary>
        private readonly string _mdbDriverName;

        protected MessageDatabusDriver(string category, byte port, string mdbDriverName)
            : base(category, port)
        {
            _mdbDriverName = mdbDriverName;
        }

        public virtual void Configure(MessageDataBus tags, int timing)
        {
            Tags = tags;
            _timing = timing;
            ActivatePolling();
        }

        public override bool EnableCommunications()
        {
            // [FTa] The communication is not handled by the driver but by the MDB.
            return false;
        }

        public override void Disconnect()
        {
            // [FTa] The communication is not handled by the driver but by the MDB.
            DeactivatePolling();
        }

        private void DeactivatePolling()
        {
            _pollingTimer.Elapsed -= _pollingTimer_Elapsed;
            _pollingTimer.Enabled = false;
        }

        private void ActivatePolling()
        {
            if (_timing > 0)
            {
                _pollingTimer = new Timer(_timing);
                _pollingTimer.AutoReset = false;
                _pollingTimer.Elapsed += _pollingTimer_Elapsed;
                _pollingTimer.Enabled = true;
            }
        }

        private void _pollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _pollingTimer.Enabled = false;

            //Do nothing

            _pollingTimer.Enabled = true;
        }

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (_pollingTimer != null)
            {
                _pollingTimer.Elapsed -= _pollingTimer_Elapsed;
                _pollingTimer.Enabled = false;
            }

            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
