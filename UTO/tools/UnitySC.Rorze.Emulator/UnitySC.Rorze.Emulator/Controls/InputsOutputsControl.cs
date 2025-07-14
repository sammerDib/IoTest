using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnitySC.Rorze.Emulator.Controls
{
    public partial class InputsOutputsControl : UserControl
    {
        public InputsOutputsControl()
        {
            InitializeComponent();
        }

        public string GetStatus(int statusIndex)
        {
            return paramDataGridView.Rows[statusIndex].Cells[valueColumn.Index].Value.ToString();
        }

        public void SetStatus(int statusIndex, string value)
        {
            paramDataGridView.Rows[statusIndex].Cells[valueColumn.Index].Value = value;
        }

        public virtual string GetConcatenatedStatuses()
        {
            return string.Empty;
        }

        public event EventHandler StatusChanged;
        private void ParamDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == valueColumn.Index)
            {
                if (StatusChanged != null)
                {
                    StatusChanged(this, EventArgs.Empty);
                }
            }
        }

        protected string BitArrayToHexaString(BitArray input)
        {
            var bytes = new byte[(input.Length - 1) / 8 + 1];

            input.CopyTo(bytes, 0);
            
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes.Reverse())
            {
                stringBuilder.Append($"{b:X2}");
            }

            return stringBuilder.ToString();
        }

        public void UpdateStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateStatus));
            }
            else
            {
                paramDataGridView.Update();
            }
        }
    }
}
