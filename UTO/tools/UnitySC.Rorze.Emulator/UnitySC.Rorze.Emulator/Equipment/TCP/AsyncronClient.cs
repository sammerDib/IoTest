using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UnitySC.Rorze.Emulator.Equipment.TCP
{
    /// <summary>
    /// Summary description for AsynchronousClient.
    /// </summary>
    internal class AsynchronousClient : TcpBaseClass
    {
        protected override void ConnectCallback(IAsyncResult ar)
        {
            base.ConnectCallback(ar);
            try
            {
                if (!(ar.AsyncState is Socket socket))
                {
                    OnErroEventSender(StringConstants.ErrorWhileEstablishingCommunication);
                    CloseCommunication();
                    return;
                }

                socket.EndConnect(ar);
                if (!IsNotConnected)
                {
                    byte[] buffer = new byte[MaxReplyLength];
                    socket.BeginReceive(buffer, 0, MaxReplyLength, SocketFlags.None,
                        ReceiveCallback, buffer);
                    ConnectionEstablishedEventSendler();
                }
                else
                {
                    OnErroEventSender(StringConstants.ErrorWhileEstablishingCommunication);
                    CloseCommunication();
                }
            }
            catch (Exception e)
            {
                OnErroEventSender(StringConstants.ErrorWhileEstablishingCommunication);
                CloseCommunication();
            }
        }

        protected override void EstablishConnection()
        {
            if (RemoteIpAddress != null)
            {
                IPEndPoint remoteEp = new IPEndPoint(RemoteIpAddress, PortIndex);
                Interlocked.Exchange(ref IsCleanUp, 0);
                try
                {
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Client.Blocking = false;
                    // Connect to the remote endpoint.
                    Client.BeginConnect(remoteEp, ConnectCallback, Client);
                }
                catch (Exception)
                {
                    OnErroEventSender(StringConstants.ErrorWhileEstablishingCommunication);
                    CloseCommunication();
                }
            }
            else
            {
                Debug.Assert(false, "null in remoteIPAddress for tcp communicator");
            }
        }
    }
}
