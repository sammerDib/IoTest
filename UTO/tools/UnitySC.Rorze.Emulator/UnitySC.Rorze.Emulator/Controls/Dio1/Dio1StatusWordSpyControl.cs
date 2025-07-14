using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.Dio1
{
    internal enum Dio1Status
    {
        OperationMode1,
        OperationMode2,
        CommandProcessing,
        ErrorCode
    }
    internal partial class Dio1StatusWordSpyControl : InputsOutputsControl
    {
        public Dio1StatusWordSpyControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio1Status.OperationMode1, "0: Initializing/1: Remote", "0");
            paramDataGridView.Rows.Add(2, Dio1Status.OperationMode2, "0: Initializing/1: Remote", "0");
            paramDataGridView.Rows.Add(3, Dio1Status.CommandProcessing, "0: Stop/1: Processing", "0");
            paramDataGridView.Rows.Add(4, Dio1Status.ErrorCode, "Identification code for error-occurred controller(s)", "00");
        }

        public string GetStatus(Dio1Status status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio1Status status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetStatus(Dio1Status.OperationMode1));
            stringBuilder.Append(GetStatus(Dio1Status.OperationMode2));
            stringBuilder.Append(GetStatus(Dio1Status.CommandProcessing));
            stringBuilder.Append("0");
            stringBuilder.Append("0");
            stringBuilder.Append("/");
            stringBuilder.Append("00");
            stringBuilder.Append(GetStatus(Dio1Status.ErrorCode));

            return stringBuilder.ToString();
        }
    }
}
