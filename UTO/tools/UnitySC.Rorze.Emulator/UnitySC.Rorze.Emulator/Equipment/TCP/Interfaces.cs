using System;
using System.Net;

namespace UnitySC.Rorze.Emulator.Equipment.TCP
{

    internal delegate void MessageReceivedSentEventHandler(object sender, MessageEventArgs args);

    internal delegate void MessageInfoHandler(object sender, MessageEventArgs args);

    internal delegate void ConnectionStatusEventHandler(object sender, ConnectionStatusEventArgs args);

    internal class ConnectionStatusEventArgs : EventArgs
    {
        ///if event ConnectionEstablished - parameter value true
        ///if event ConnectionFail - parameter false
        public bool IsConnected;
        public readonly string ConnectionState;

        public ConnectionStatusEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
            ConnectionState =
                isConnected ? StringConstants.CommunicationEstablished : StringConstants.CommunicationClosed;
        }
    }

    public class MessageEventArgs
    {
        public string Message;
        public MessageEventArgs(string message)
        {
            Message = message;
        }
    }


    internal interface ITcpSenderReceiver
    {
        event MessageReceivedSentEventHandler OnError;
        event MessageReceivedSentEventHandler MessageReceived;
        event MessageInfoHandler OnInfo;
        event ConnectionStatusEventHandler ConnectionStatusChanged;

        void Send(String data);
        void Connect(int connectionTimeout);

        void Disconnect();

        IPAddress RemoteIpAddress { get; set; }
        int PortIndex { get; set; }
        int MaxReplyLength { get; set; }
        bool IsNotConnected { get; }
    }

    internal class StringConstants
    {
        private StringConstants()
        {
        }

        public const string ConnectionTimeout = "communication timeout expired. Failed to establish communication";
        public const string CommunicationClosedByRemoteSide = "communication is closed by remote side";
        public const string FailedToRecieveMessage = "failed to receive message";
        public const string ErrorWhileRecievingMessage = "error while receiving message";
        public const string MessageReceived = "message {0} is received";
        public const string DebugErrorStringFormat = "type of exception {0}; message: {1}; source {2};";
        public const string FailedToSendMessage = "failed to send message \"{0}\"";
        public const string MessageWasntSent = "message {0} was not sent";
        public const string ErrorWhileSendingMessage = "error while sending message";
        public const string MessageSent = "message {0} is sent";
        public const string CommunicationEstablished = "communication is established";
        public const string ErrorWhileEstablishingCommunication = "error while establishing communication";
        public const string ErrorWhileClosingCommunication = "error while closing communication";
        public const string CommunicationClosed = "communication is closed";
    }


}