using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UnitySC.Rorze.Emulator.Equipment.TCP
{
    /// <summary>
    /// Summary description for CommonInterfase.
    /// </summary>
    internal abstract class TcpBaseClass : ITcpSenderReceiver, IDisposable
    {
        // The port number for the remote device.
        private Timer _connectionTimer;
       
        protected Socket Client;
        protected int IsCleanUp;

        #region IDisposable

        ~TcpBaseClass()
        {
            Dispose();
        }

        public void Dispose()
        {
            CloseCommunication();
        }

        #endregion
        
        #region ITCPSenderReceiver
        
        public event MessageReceivedSentEventHandler OnError;
        public event MessageReceivedSentEventHandler MessageReceived;
        public event MessageInfoHandler OnInfo;
        public event ConnectionStatusEventHandler ConnectionStatusChanged;

        public void Connect(int connectionTimeout)
        {
            if (connectionTimeout > 0)
            {
                _connectionTimer = new Timer(HandleConnectionTimeoutExpired,
                                               null, connectionTimeout, 0);
            }
            EstablishConnection();
        }

        public void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            try
            {
                if (!IsNotConnected && Client.Poll(0, SelectMode.SelectWrite))
                {
                    data = data.Replace("\\r", "\r");
                    data = data.Replace("\\n", "\n");
					
                    byte[] byteData = Encoding.ASCII.GetBytes(data);
                    // Begin sending the data to the remote device.
                    Client.Send(byteData, 0, byteData.Length, 0);
                }
            }
            catch (Exception)
            {
                OnErroEventSender(StringConstants.ErrorWhileSendingMessage);
                CloseCommunication();
            }
        }

        public void Disconnect()
        {
            CloseCommunication();
        }

        public virtual bool IsNotConnected
        {
            get
            {
                if (Client != null)
                {
                    if (Client.Connected)
                    {
                        if (Client.Poll(0, SelectMode.SelectRead) && Client.Available == 0)
                        {
                            //connection is closed by the remote side
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return true;
                }
                else
                    return true;
            }
        }

        public IPAddress RemoteIpAddress { get; set; }

        public int PortIndex { get; set; }

        public int MaxReplyLength { get; set; } = 1000;

        #endregion

        #region Protected, private

        protected abstract void EstablishConnection();

        protected virtual void ConnectCallback(IAsyncResult ar)
        {
            if (_connectionTimer != null)
            {
                _connectionTimer.Dispose();
                _connectionTimer = null;
            }
        }
        
        protected void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!IsNotConnected)
                {
                    byte[] buffer = (byte[])ar.AsyncState;

                    int bytesRead = Client.EndReceive(ar);
                    string bufferString = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (bytesRead > 0)
                    {
                        OnInfoEventSender(string.Format(StringConstants.MessageReceived, bufferString));
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessageEventArgs(bufferString));
                        }
                        buffer = new byte[MaxReplyLength];

                        Client.BeginReceive(buffer, 0, MaxReplyLength, SocketFlags.None,
                            ReceiveCallback, buffer);

                    }
                    else
                    {
                        CloseCommunication();
                    }
                }
                else
                {
                    CloseCommunication();
                }
            }
            catch (Exception)
            {
                OnErroEventSender(StringConstants.ErrorWhileRecievingMessage);
                CloseCommunication();
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = Client.EndSend(ar);
                
                if (ar.AsyncState != null && ar.AsyncState is string)
                {
                    string data = (string)ar.AsyncState;
                    if (data.ToCharArray().Length != bytesSent)
                    {
                        OnErroEventSender(string.Format(StringConstants.FailedToSendMessage, data));
                    }
                    else
                    {
                        OnInfoEventSender(string.Format(StringConstants.MessageSent, data));
                    }
                }
            }
            catch (Exception)
            {
                OnErroEventSender(StringConstants.ErrorWhileSendingMessage);
                CloseCommunication();
            }
        }

        protected void OnErroEventSender(string message)
        {
            if (_connectionTimer != null)
            {
                _connectionTimer.Dispose();
                _connectionTimer = null;
            }
            if (OnError != null)
            {
                MessageEventArgs arg = new MessageEventArgs(message);
                OnError(this, arg);
            }
        }

        protected void OnInfoEventSender(string message)
        {
            if (OnInfo != null)
            {
                MessageEventArgs arg = new MessageEventArgs(message);
                OnInfo(this, arg);
            }
        }

        protected void ConnectionEstablishedEventSendler()
        {
            if (_connectionTimer != null)
            {
                _connectionTimer.Dispose();
                _connectionTimer = null;
            }
            if (!IsNotConnected)
            {
                if (ConnectionStatusChanged != null)
                    ConnectionStatusChanged(this, new ConnectionStatusEventArgs(true));
            }
            else
            {
                Debug.Assert(false, "error in connection, no connected");
            }

        }

        private void HandleConnectionTimeoutExpired(object state)
        {
            if (_connectionTimer != null)
            {
                _connectionTimer.Dispose();
                _connectionTimer = null;
            }
            OnErroEventSender(StringConstants.ConnectionTimeout);
            CloseCommunication();
        }
		
        protected string FormatErrorString(Exception exeption)
        {
            return string.Format(StringConstants.DebugErrorStringFormat,
                exeption.GetType(), exeption.Message, exeption.Source);
        }



        protected virtual void CloseCommunication()
        {
            if (Interlocked.CompareExchange(ref IsCleanUp, 1, 0) != 1)
            {
                if (Client != null)
                {
                    try
                    {
                        Client.Shutdown(SocketShutdown.Both);
                        ConnectionStatusChanged?.Invoke(this, new ConnectionStatusEventArgs(false));
                    }
                    catch (Exception)
                    {
                        OnErroEventSender(StringConstants.ErrorWhileClosingCommunication);
                    }
                    Client.Close();
                }
                
                Client = null;
            
            }
        }


        #endregion
    }


}
