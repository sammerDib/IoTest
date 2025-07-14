using System;
using System.Windows.Forms;
using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls
{
    internal partial class SendControl : UserControl
    {
        private string _stringTerminatingSymbol = string.Empty;
        private readonly string _stringPrefixSymbol = string.Empty;
        protected bool IsRawCommand;
        private string _stationName = string.Empty;
        private string _prompt = string.Empty;

        public SendControl()
        {
            InitializeComponent();
        }

        public string Postfix
        {
            protected set
            {
                if (value != null)
                {
                    _stringTerminatingSymbol = value;
                    ShowPostfix();
                }
            }
            get => _stringTerminatingSymbol;
        }
        
        public string Prefix
        {
            protected set
            {
                if (value != null)
                {
                    _stationName = string.Empty;
                    _prompt = value;
                    ShowPrefix();
                }
            }
            get => _stationName + _prompt;
        }
        /// <summary>
        /// Postfix value to add to the end of sent string
        /// </summary>
        public string StringTerminatingSymbol
        {
            set
            {
                _stringTerminatingSymbol = value ?? string.Empty;

                if (!IsRawCommand)
                    ShowPostfix();
            }

        }

        /// <summary>
        /// Station Name is used by some equipment. Used as a part of prefix 
        /// (prefix = station name + prompt)
        /// </summary>
        public string StationName
        {
            set
            {
                _stationName = value ?? string.Empty;

                if (!IsRawCommand)
                    ShowPrefix();
            }
        }
        /// <summary>
        /// Prompt is used by some equipment. Used as a part of prefix 
        /// (prefix = station name + prompt)
        /// </summary>
        public string Prompt
        {
            set
            {
                _prompt = value ?? string.Empty;

                if (!IsRawCommand)
                    ShowPrefix();
            }
        }
        /// <summary>
        /// Message To Send
        /// </summary>
        public string OutMessage
        {
            set => outMessageTextBox.Text = value == null ? string.Empty : UtilitiesFunctions.FormatString(value);
            get => outMessageTextBox.Text;
        }

        public event EventHandler<SendMessageEventArgs> MessageSentPressed;
        private void SendBtn_Click(object sender, EventArgs e)
        {
            if (MessageSentPressed != null)
            {
                string messageToSend =
                                        UtilitiesFunctions.ReFormatString(
                                        prefixTextBox.Text
                                        + outMessageTextBox.Text
                                        + postfixTextBox.Text);
                SendMessageEventArgs args = new SendMessageEventArgs(messageToSend);
                MessageSentPressed(this, args);

                outMessageTextBox.Text = string.Empty;
            }
        }

        private void RawCommandChkBox_Click(object sender, EventArgs e)
        {
            IsRawCommand = rawCommandChkBox.Checked;

            if (IsRawCommand)
            {
                prefixTextBox.Text = "";
                postfixTextBox.Text = "";
            }
            else
            {
                ShowPostfix();
                ShowPrefix();
            }
        }

        private void ShowPrefix()
        {
            prefixTextBox.Text = (!string.IsNullOrEmpty(_stringPrefixSymbol))
                ? _stringPrefixSymbol
                : UtilitiesFunctions.FormatString(_stationName)
                  + UtilitiesFunctions.FormatString(_prompt);
        }
        private void ShowPostfix()
        {
            postfixTextBox.Text =
                UtilitiesFunctions.FormatString(_stringTerminatingSymbol);
        }

        private void SendControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 || e.KeyChar == '`')
            {
                e.Handled = false;
                SendBtn_Click(this, null);
            }
        }
    }

    public class SendMessageEventArgs : EventArgs
    {
        public readonly string FullMessage;

        public readonly byte[] FullMessageByte;

        public SendMessageEventArgs(string fullMessage)
        {
            FullMessage = fullMessage;
        }

        public SendMessageEventArgs(byte[] fullMessage)
        {
            FullMessageByte = fullMessage;
        }
    }
}
