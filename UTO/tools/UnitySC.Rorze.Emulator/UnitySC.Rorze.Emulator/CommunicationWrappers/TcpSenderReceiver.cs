using System;
using System.Net;
using UnitySC.Rorze.Emulator.Common;
using UnitySC.Rorze.Emulator.Equipment.TCP;

namespace UnitySC.Rorze.Emulator.CommunicationWrappers
{
    internal class TcpSenderReceiver : IDisposable
    {
        public SenderReceiverEvent SenderReceiverStatusChanged;
        public SenderReceiverEvent SenderReceiverMesageReceived;
        public ConnectionEvent ConnectionStateChanged;
        public bool IsConnected => !_tcpSenderReceiver?.IsNotConnected ?? false;

        private ITcpSenderReceiver _tcpSenderReceiver;

        #region Public

        public void Connect(bool thisItServer, IPAddress ipAddress, int portNumber)
        {
            Disconnect();

            if (thisItServer)
            {
                _tcpSenderReceiver = new AsynchronousServer();
            }
            else
            {
                _tcpSenderReceiver = new AsynchronousClient();
            }

            _tcpSenderReceiver.OnError += HandleTcpSenderReceiverErroMessageReceived;
            _tcpSenderReceiver.MessageReceived += HandleTcpSenderReceiverMessageReceived;
            _tcpSenderReceiver.OnInfo += HandleTcpSenderReceiverInfoMessageReceived;
            _tcpSenderReceiver.ConnectionStatusChanged += HandleTcpSenderReceiverConnectionStatusChanged;

            _tcpSenderReceiver.RemoteIpAddress = ipAddress;
            _tcpSenderReceiver.PortIndex = portNumber;
            _tcpSenderReceiver.MaxReplyLength = 100;
            _tcpSenderReceiver.Connect(Constants.TcpConnectionTimeout);
        }

        public void Disconnect()
        {
            if (_tcpSenderReceiver != null)
            {
                _tcpSenderReceiver.Disconnect();
                Clean();
            }
        }

        public void SendMessage(string message)
        {
            string sentString = UtilitiesFunctions.FormatString(message);

            if (_tcpSenderReceiver != null && !_tcpSenderReceiver.IsNotConnected)
            {

                try
                {
                    _tcpSenderReceiver.Send(UtilitiesFunctions.ReFormatString(message));
                }
                catch (Exception exeption)
                {
                    if (SenderReceiverStatusChanged != null)
                    {
                        SenderReceiverStatusChanged("ERROR - Failed To Send: " + exeption.Message);
                    }
                    return;
                }

                //dataLogControl.LogMessageOut(@sentString, DateTime.Now);
                if (SenderReceiverStatusChanged != null)
                {
                    SenderReceiverStatusChanged("The following message has been sent: " + sentString);
                }
            }
            else
            {
                if (SenderReceiverStatusChanged != null)
                {
                    SenderReceiverStatusChanged("ERROR - Cannot Send: TCP Connection is not established");
                }
            }
        }

        #endregion

        #region Private

        #region ITCPSenderReceiver events handlers
        private void HandleTcpSenderReceiverConnectionStatusChanged(object sender, ConnectionStatusEventArgs args)
        {
            if (SenderReceiverStatusChanged != null)
            {
                string strToSend;
                if (args.IsConnected)
                {
                    strToSend = "TCP Connection is established";
                }
                else
                {
                    strToSend = "TCP Connection is closed";
                }
                SenderReceiverStatusChanged(strToSend);
            }

            if (ConnectionStateChanged != null)
            {
                ConnectionStateChanged(args.IsConnected);
            }

            if (!args.IsConnected)
            {
                Clean();
            }
            else
            {
                if(SenderReceiverMesageReceived != null)
                {
                    SenderReceiverMesageReceived(null);
                }
            }
        }

        private void HandleTcpSenderReceiverErroMessageReceived(object sender, MessageEventArgs args)
        {
            if (SenderReceiverStatusChanged != null)
            {
                SenderReceiverStatusChanged(args.Message);
            }
            Disconnect();
            if (ConnectionStateChanged != null)
            {
                ConnectionStateChanged(false);
            }

        }

        private void HandleTcpSenderReceiverInfoMessageReceived(object sender, MessageEventArgs args)
        {
            if (SenderReceiverStatusChanged != null)
            {
                SenderReceiverStatusChanged(args.Message);
            }
        }

        private void HandleTcpSenderReceiverMessageReceived(object sender, MessageEventArgs args)
        {
            if (SenderReceiverMesageReceived != null)
            {
                SenderReceiverMesageReceived(args.Message);
            }
        }

        #endregion

        private void Clean()
        {
            if (_tcpSenderReceiver != null)
            {
                _tcpSenderReceiver.OnInfo -= HandleTcpSenderReceiverInfoMessageReceived;
                _tcpSenderReceiver.OnError -= HandleTcpSenderReceiverInfoMessageReceived;
                _tcpSenderReceiver.MessageReceived -= HandleTcpSenderReceiverMessageReceived;
                _tcpSenderReceiver.ConnectionStatusChanged -= HandleTcpSenderReceiverConnectionStatusChanged;
                _tcpSenderReceiver = null;
            }
        }

        #endregion

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Disconnect();
        }
    }
}
