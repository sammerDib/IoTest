using System;
using System.Drawing;
using System.Windows.Forms;

namespace UnitySC.Rorze.Emulator.Controls
{
    internal partial class BitCheckControl : UserControl
    {
        /// <summary>
        /// pen to draw line from check box to description label
        /// </summary>
        private readonly Pen _blackBluePen;
        /// <summary>
        /// value to return for number of bit
        /// </summary>
        private byte _number = byte.MaxValue;
        /// <summary>
        /// to show bit number or not
        /// </summary>
        private bool _isToShowNumber;
        /// <summary>
        /// To Fire or Not BitCheckedStateChanged event
        /// </summary>
        private bool _isToFireBitCheckedStateChanged = true;
        /// <summary>
        /// 
        /// </summary>
        public BitCheckControl()
        {
            InitializeComponent();
            _blackBluePen = new Pen(Brushes.Black, (float)0.9);
            Number = 0;
        }

        #region Public
        /// <summary>
        /// Event that notify that bit check state is changed
        /// </summary>
        public event EventHandler<BitCheckedStateChangedEventArgs> BitCheckedStateChanged;
        /// <summary>
        /// Bit number, if less then 0 - no number are defined
        /// </summary>
        public byte Number
        {
            set
            {
                _number = value;
                numberLabel.Text = !_isToShowNumber ? string.Empty : value.ToString();
            }
            get => _number;
        }
        /// <summary>
        /// To show bit number or not
        /// </summary>
        public bool ToShowNumber
        {
            set
            {
                if (_isToShowNumber != value)
                {
                    _isToShowNumber = value;
                    Number = _number;
                }
            }
            get => _isToShowNumber;
        }
        /// <summary>
        /// Property for bit checked state
        /// </summary>
        public bool CheckedState
        {
            set => chkBox.Checked = value;
            get => chkBox.Checked;
        }
        /// <summary>
        /// To stop fire event
        /// </summary>
        public void SuspendBitCheckedEvent()
        {
            _isToFireBitCheckedStateChanged = false;
        }
        /// <summary>
        /// To start fire Event
        /// </summary>
        public void ResumeBitCheckedEvent()
        {
            _isToFireBitCheckedStateChanged = true;
        }
        #endregion

        #region Auxilary
        /// <summary>
        /// need to draw line from check box to description label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxDown_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;

            Point start = new Point((chkBox.Location.X + chkBox.Size.Height) / 2, chkBox.Location.Y + chkBox.Size.Height);
            Point end = new Point(start.X, Location.Y + Size.Height);

            gr.DrawLine(_blackBluePen, start, end);
        }
        private void CheckBoxDown_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox && _isToFireBitCheckedStateChanged && BitCheckedStateChanged != null)
            {
                BitCheckedStateChangedEventArgs args = new BitCheckedStateChangedEventArgs(checkBox.Checked, _number);
                BitCheckedStateChanged(this, args);
            }
        }
        #endregion

    }

    #region EventArgs
    public class BitCheckedStateChangedEventArgs : EventArgs
    {
        public readonly bool IsChecked;
        public readonly byte BitNumber;

        public BitCheckedStateChangedEventArgs(bool isChecked, byte bitNumber)
        {
            IsChecked = isChecked;
            BitNumber = bitNumber;
        }
    }
    #endregion
}
