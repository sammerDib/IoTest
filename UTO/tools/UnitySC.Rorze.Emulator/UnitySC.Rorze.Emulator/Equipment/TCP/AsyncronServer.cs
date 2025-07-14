using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UnitySC.Rorze.Emulator.Equipment.TCP
{

    /// <summary>
    /// Summary description for AsynchronousServer.
    /// </summary>
    internal class AsynchronousServer : TcpBaseClass
    {
        private Socket _listener;

        protected override void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                base.ConnectCallback(ar);
                if (_listener == null) return;
                Client = _listener.EndAccept(ar);
                if (!IsNotConnected)
                {
                    byte[] buffer = new byte[MaxReplyLength];
                    Client.BeginReceive(buffer, 0, MaxReplyLength, SocketFlags.None,
                        ReceiveCallback, buffer);
                    ConnectionEstablishedEventSendler();
                }
                else
                    OnErroEventSender(StringConstants.ConnectionTimeout);
            }
            catch (Exception)
            {
                OnErroEventSender(StringConstants.ErrorWhileEstablishingCommunication);
                CloseCommunication();
            }
        }

        protected override void EstablishConnection()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(RemoteIpAddress, PortIndex);
            Interlocked.Exchange(ref IsCleanUp, 0);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _listener.Bind(remoteEndPoint);
                _listener.Listen(1);
                _listener.BeginAccept(ConnectCallback, _listener);
            }
            catch (Exception)
            {
                OnErroEventSender(StringConstants.ErrorWhileEstablishingCommunication);
                CloseCommunication();
            }
        }

        protected override void CloseCommunication()
        {
           base.CloseCommunication();
          
               if (_listener != null)
               {
                   lock (_listener)
                   {
                       _listener.Close();
                       _listener = null;
                   }
               }
           
        }
       
    }
}

/*

 

        protected override void CloseCommunication()
        {
            if (Interlocked.CompareExchange(ref IsCleanUp, 1, 0) != 1)
            {
                if (listener != null)
                {
                    try
                    {
                        if (!IsNotConnected)
                        {
                            client.Shutdown(SocketShutdown.Both);
                            SetTCPTraceEvent(StringConstants.CommunicationClosed, string.Empty, TraceLevelType.Info);
                        }
                    }
                    catch (Exception e)
                    {
                        SetTCPTraceEvent(StringConstants.ErrorWhileClosingCommunication,
                                         FormatErrorString(e), TraceLevelType.Debug);
                    }
                    listener.Close();
                }
                base.CloseCommunication();
                client = null;
                listener = null;
            }
        }
    }
}


*/