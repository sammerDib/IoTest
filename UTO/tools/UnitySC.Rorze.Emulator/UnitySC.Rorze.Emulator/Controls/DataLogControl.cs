using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using UnitySC.Rorze.Emulator.Common;

namespace UnitySC.Rorze.Emulator.Controls
{
    internal class DataLogControl : UserControl
    {
		private Panel _mainPanel;
		private TextBox _dataLogTextBox;
        private Button _clearButton;
		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private readonly Container _components = null;

        private readonly int _maxCommunicationLogSize = Constants.MaxCommunicationLogSize;
		
		public DataLogControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			_dataLogTextBox.Clear();
		}
		
		private void DataLogControl_Layout(object sender, LayoutEventArgs e)
		{
            SuspendLayout();
            _mainPanel.Location = new Point(4, 4);
            _mainPanel.Size = new Size(Size.Width - 12, Size.Height - 12);
			
            //equipmentLabel.Location = new Point(24, 32);
            //comLogStaticLabel.Location = new Point(24, 48);

            lock (_dataLogTextBox)
            {
                _dataLogTextBox.Location = new Point(_mainPanel.Location.X + 8,
                    16 + _clearButton.Height + 16);
            }

            lock (_dataLogTextBox)
            {
                _dataLogTextBox.Size = new Size(_mainPanel.Size.Width - _dataLogTextBox.Location.X - 8,
                    _mainPanel.Size.Height - _dataLogTextBox.Location.Y - 8);
            }
            lock (_dataLogTextBox)
            {
                _clearButton.Location = new Point(_dataLogTextBox.Location.X + _dataLogTextBox.Width - _clearButton.Width, 16);
            }
            ResumeLayout();
			
		}
		
		
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(_components != null)
				{
					_components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this._mainPanel = new System.Windows.Forms.Panel();
            this._clearButton = new System.Windows.Forms.Button();
            this._dataLogTextBox = new System.Windows.Forms.TextBox();
            this._mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this._mainPanel.Controls.Add(this._clearButton);
            this._mainPanel.Controls.Add(this._dataLogTextBox);
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._mainPanel.Location = new System.Drawing.Point(0, 0);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(322, 380);
            this._mainPanel.TabIndex = 0;
            // 
            // clearButton
            // 
            this._clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._clearButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._clearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._clearButton.Location = new System.Drawing.Point(215, 15);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(96, 24);
            this._clearButton.TabIndex = 50;
            this._clearButton.Text = "Clear";
            this._clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // dataLogTextBox
            // 
            this._dataLogTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._dataLogTextBox.Location = new System.Drawing.Point(8, 60);
            this._dataLogTextBox.Multiline = true;
            this._dataLogTextBox.Name = "_dataLogTextBox";
            this._dataLogTextBox.ReadOnly = true;
            this._dataLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._dataLogTextBox.Size = new System.Drawing.Size(304, 308);
            this._dataLogTextBox.TabIndex = 6;
            // 
            // DataLogControl
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this._mainPanel);
            this.Name = "DataLogControl";
            this.Size = new System.Drawing.Size(322, 380);
            this.VisibleChanged += new System.EventHandler(this.DataLogControl_VisibleChanged);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.DataLogControl_Layout);
            this._mainPanel.ResumeLayout(false);
            this._mainPanel.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		
		public void LogMessageIn(string message, DateTime messageTime)
		{
			MessageAdd("H: " + UtilitiesFunctions.FormatString(message), messageTime);
		}
		
		public void LogMessageOut(string message,DateTime messageTime)
		{
			MessageAdd("E: " + UtilitiesFunctions.FormatString(message), messageTime);
		}
		
		private void Clear()
		{
			lock(_dataLogTextBox)
			{
				_dataLogTextBox.Text = string.Empty;
			}
		}
			
				
		delegate void MessageAddDelegate(string message, DateTime messageTime);
		private void MessageAdd(string message, DateTime messageTime)
        {
            if (!_dataLogTextBox.IsDisposed)
            {
                if (!InvokeRequired)
                {
                    message = "[" + messageTime.ToString("T", DateTimeFormatInfo.InvariantInfo) + ":" + (messageTime.Millisecond.ToString()).PadLeft(3, '0') + "]: " + message;
                    lock (_dataLogTextBox)
                    {
                        _dataLogTextBox.AppendText(message + "\r\n");
                        while (_dataLogTextBox.Lines.Length >= _maxCommunicationLogSize)
                        {
                            _dataLogTextBox.Text = _dataLogTextBox.Text.Remove(0, _dataLogTextBox.Lines[0].Length + 2);
                        }
                        _dataLogTextBox.SelectionStart = _dataLogTextBox.Text.Length - 1;
                        _dataLogTextBox.ScrollToCaret();
                    }
                }
                else
                {
                    MessageAddDelegate messageAdd = MessageAdd;
                    BeginInvoke(messageAdd, message, messageTime);
                }
            }
        }

		private void clearButton_Click(object sender, EventArgs e)
		{
			Clear();
		}

        private void DataLogControl_VisibleChanged(object sender, EventArgs e)
		{
			lock(_dataLogTextBox)
			{
				if(_dataLogTextBox.Text != null && _dataLogTextBox.Text.Length >1)
				{
					_dataLogTextBox.SelectionStart = _dataLogTextBox.Text.Length - 1;
					_dataLogTextBox.ScrollToCaret();
				}
			}
		}
	}
}
