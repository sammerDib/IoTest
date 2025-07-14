using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnitySC.Rorze.Emulator.CommunicationWrappers;
using UnitySC.Rorze.Emulator.Controls;

namespace UnitySC.Rorze.Emulator.Equipment.BaseEquipmentControl
{
    internal partial class BaseEquipmentControl : UserControl, IEquipmentControl
    {
        #region Fields
        private Timer _timer;
        private TcpSenderReceiver _tcpSenderReceiver;
        private bool _autoConnect = true;
        #endregion

        #region Constructor

        protected BaseEquipmentControl()
        {
            InitializeComponent();

            _tcpSenderReceiver = new TcpSenderReceiver();
            _tcpSenderReceiver.SenderReceiverStatusChanged += HandleTcpPortStatusChanged;
            _tcpSenderReceiver.SenderReceiverMesageReceived += HandleTcpPortMessageReceived;
            _tcpSenderReceiver.ConnectionStateChanged += HandleTcpConnectionChangedEvent;
        }

        #endregion

        #region IEquipmentControl

        public bool AutoResponseEnabled
        {
            get => autorespButton.Checked;
            set => autorespButton.Checked = value;
        }

        public virtual bool AutoResponse(string toRespondeTo)
        {
            if (!autorespButton.Checked)
            {
                return false;
            }

            List<string> responses = GetResponses(toRespondeTo);

            for (int i = 0; i < responses.Count; i++)
            {
                string response = responses[i];
                if (response != string.Empty)
                {
                    if (i > 0)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    SendMessage(response);
                    TreatResponse(toRespondeTo, response);
                }
                System.Threading.Thread.Sleep(100);
            }
            return true;
        }

        public virtual void Clean()
        {
            _tcpSenderReceiver.SenderReceiverStatusChanged -= HandleTcpPortStatusChanged;
            _tcpSenderReceiver.SenderReceiverMesageReceived -= HandleTcpPortMessageReceived;
            _tcpSenderReceiver.ConnectionStateChanged -= HandleTcpConnectionChangedEvent;
            _tcpSenderReceiver.Dispose();
        }

        public void Setup(string ipAddress, string portId, bool isServer = true)
        {
            ipTextBox.Text = ipAddress;
            portTextBox.Text = portId;
            serverRadioButton.Checked = isServer;
            clientRadioButton.Checked = !isServer;

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            Connect();
        }

        #endregion

        #region Protected methods

        protected virtual void TreatResponse(string toRespondeTo, string response)
        {
            //default behavior => do nothing
        }


        protected virtual void SendMessage(string message)
        {
            dataLogControl.LogMessageOut(message, DateTime.Now);
            _tcpSenderReceiver.SendMessage(message);
        }

        protected virtual List<string> GetResponses(string toRespondeTo)
        {
            return new List<string>();
        }

        protected virtual void SendConnectedMessage()
        {
            //Do nothing in base class
        }

        #endregion

        #region Event Handlers

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer?.Stop();
            if (!_tcpSenderReceiver.IsConnected && _autoConnect)
            {
                Connect();
            }
            _timer?.Start();
        }

        private void AutoResponseButton_CheckedChanged(object sender, EventArgs e)
        {
            sendControl.Enabled = !autorespButton.Checked;
        }

        private void SendControl_MessageSentPressed(object sender, SendMessageEventArgs e)
        {
            SendMessage(e.FullMessage);
        }

        #endregion

        #region TCP

        private void connectButton_Click(object sender, EventArgs e)
        {
            _autoConnect = true;
            Connect();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            _autoConnect = false;
            _tcpSenderReceiver.Disconnect();
            connectButton.Enabled = true;
            disconnectButton.Enabled = false;
        }

        private void HandleTcpPortMessageReceived(string receivedMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(delegate
                {
                    HandleTcpPortMessageReceived(receivedMessage);
                }));
                return;
            }

            if (receivedMessage != null)
            {
                dataLogControl.LogMessageIn((@receivedMessage), DateTime.Now);
            }

            Task.Run(
                () =>
                {
                    if (AutoResponseEnabled)
                    {
                        AutoResponse(receivedMessage);
                    }
                });
        }

        private void Connect()
        {
            IPAddress ipAddress = IPAddress.Parse(ipTextBox.Text);
            int portNum = int.Parse(portTextBox.Text);

            _tcpSenderReceiver.Connect(serverRadioButton.Checked, ipAddress, portNum);

            connectButton.Enabled = false;
            disconnectButton.Enabled = true;
        }

        private void HandleTcpPortStatusChanged(string status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(delegate
                {
                    HandleTcpPortStatusChanged(status);
                }));
                return;
            }

            tcpStatusBarPanel.Text = @status;
        }

        private void HandleTcpConnectionChangedEvent(bool isConnected)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(delegate
                {
                    HandleTcpConnectionChangedEvent(isConnected);
                }));
                return;
            }

            connectButton.Enabled = !isConnected;
            disconnectButton.Enabled = isConnected;

            Task.Run(
                () =>
                {
                    //Need to send the connected message when the emulator is configured as a client
                    if (isConnected && !serverRadioButton.Checked)
                    {
                        SendConnectedMessage();
                    }
                });
        }

        #endregion
    }
}
