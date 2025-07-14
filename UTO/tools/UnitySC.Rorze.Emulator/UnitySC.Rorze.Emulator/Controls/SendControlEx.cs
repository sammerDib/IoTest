using System;
using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls
{
    internal delegate void CalculatePrefixAndPostfix(string messageToSend, out string prefix, out string postfix);
    
    internal class SendControlEx:SendControl
    {
        private CalculatePrefixAndPostfix _prePostCalcul;
        
        public SendControlEx()
        {
            InitializeComponent();
        }

        public CalculatePrefixAndPostfix PrefixAndPostfixCalculator
        {
            set
            {
                if(value != null)
                {
                    _prePostCalcul = value;
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // outMessageTextBox
            // 
            this.outMessageTextBox.TextChanged += OutMessageTextBox_TextChanged;
            // 
            // SendControlEx
            // 
            this.Name = "SendControlEx";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void OutMessageTextBox_TextChanged(object sender, EventArgs e)
        {
           if(_prePostCalcul != null && !IsRawCommand)
           {
               _prePostCalcul(outMessageTextBox.Text, out var prefix,out var postfix);
               
               Prefix = UtilitiesFunctions.FormatString(prefix);
               Postfix = UtilitiesFunctions.FormatString(postfix);
           }
        }
    }
}
