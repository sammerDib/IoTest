using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.Dio2
{
    internal enum Dio2Status
    {
        OperationMode1,
        OperationMode2,
        CommandProcessing,
        ErrorCode
    }
    internal partial class Dio2StatusWordSpyControl : InputsOutputsControl
    {
        public Dio2StatusWordSpyControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio2Status.OperationMode1, "0: Initializing/1: Remote", "0");
            paramDataGridView.Rows.Add(2, Dio2Status.OperationMode2, "0: Initializing/1: Remote", "0");
            paramDataGridView.Rows.Add(3, Dio2Status.CommandProcessing, "0: Stop/1: Processing", "0");
            paramDataGridView.Rows.Add(4, Dio2Status.ErrorCode, "Identification code for error-occurred controller(s)", "00");
        }

        public string GetStatus(Dio2Status status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio2Status status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetStatus(Dio2Status.OperationMode1));
            stringBuilder.Append(GetStatus(Dio2Status.OperationMode2));
            stringBuilder.Append(GetStatus(Dio2Status.CommandProcessing));
            stringBuilder.Append("0");
            stringBuilder.Append("0");
            stringBuilder.Append("/");
            stringBuilder.Append("00");
            stringBuilder.Append(GetStatus(Dio2Status.ErrorCode));

            return stringBuilder.ToString();
        }
    }
}
