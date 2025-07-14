using System;
using System.Timers;

namespace UnitySC.Rorze.Emulator.CommunicationWrappers
{
    internal delegate void SenderReceiverEvent(string status);
    internal delegate void ConnectionEvent(bool isConnected);

    internal class ReceivedStringManager : IDisposable
    {
        private readonly Timer _termStringTimer;
        private string _awaitedTermString;
        string _receivedMessage = string.Empty;
        readonly string[] _terminatedStringToAwait = { "\r\n", "\r", "" };
        public SenderReceiverEvent MessageReceived;

        #region Public

        public ReceivedStringManager()
        {
            _termStringTimer = GetTimer();
            _awaitedTermString = string.Empty;
        }

        public void AddMessage(string s)
        {
            lock (this)
            {
                _receivedMessage += s;

                foreach (string str in _terminatedStringToAwait)
                {
                    if (_receivedMessage.IndexOf(str, StringComparison.Ordinal) != -1)
                    {
                        RestartTerminatedStringTimer(str);
                        break;
                    }
                }
            }
        }

        public void Clean()
        {
            if (_termStringTimer.Enabled)
                _termStringTimer.Stop();

            _awaitedTermString = string.Empty;
        }

        #endregion

        #region Private

        #region Timers Operations
        private Timer GetTimer()
        {
            Timer timer = new Timer();
            timer.AutoReset = false;
            timer.Enabled = false;
            timer.Interval = 500;
            timer.Elapsed += Timer_Elapsed;
            return timer;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!(sender is Timer timer)) return;
            if (timer.Enabled)
                timer.Stop();

            ShowReceivedMessage(_awaitedTermString);
            _awaitedTermString = string.Empty;
        }

        private void RestartTerminatedStringTimer(string terminatedString)
        {
            if (_awaitedTermString != terminatedString)
            {
                _awaitedTermString = terminatedString;

                if (_termStringTimer.Enabled)
                    _termStringTimer.Stop();
            }

            if (!_termStringTimer.Enabled)
                _termStringTimer.Start();
        }

        #endregion

        #region Received Message Operations

        private void ShowReceivedMessage(string terminatingSymbols)
        {
            lock (this)
            {
                if (_receivedMessage.Length == 0)
                    return;

                if (terminatingSymbols.Length == 0)
                {
                    if (MessageReceived != null)
                    {
                        MessageReceived(_receivedMessage);
                    }
                    _receivedMessage = string.Empty;
                }
                else
                {
                    while (_receivedMessage.IndexOf(terminatingSymbols, StringComparison.Ordinal) != -1)
                    {
                        var strToShow = _receivedMessage.Substring(0,
                            _receivedMessage.IndexOf(terminatingSymbols, StringComparison.Ordinal) +
                            terminatingSymbols.Length);
                        _receivedMessage = _receivedMessage.Substring(
                            _receivedMessage.IndexOf(strToShow, StringComparison.Ordinal) + strToShow.Length);


                        if (MessageReceived != null)
                        {
                            MessageReceived(strToShow);
                        }
                    }
                }
            }

            AddMessage(string.Empty);
        }

        #endregion

        #endregion

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Clean();
            _termStringTimer.Dispose();
        }
    }
}
