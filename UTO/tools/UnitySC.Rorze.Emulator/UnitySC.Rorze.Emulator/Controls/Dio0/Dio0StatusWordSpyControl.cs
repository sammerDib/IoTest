using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.Dio0
{
    internal enum Dio0Status
    {
        OperationMode1,
        OperationMode2,
        CommandProcessing,
        ControllerIdentificationCode,
        ErrorCode
    }
    internal partial class Dio0StatusWordSpyControl : InputsOutputsControl
    {
        public Dio0StatusWordSpyControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Dio0Status.OperationMode1, "0: Initializing/1: Remote", "0");
            paramDataGridView.Rows.Add(2, Dio0Status.OperationMode2, "0: Initializing/1: Remote", "0");
            paramDataGridView.Rows.Add(3, Dio0Status.CommandProcessing, "0: Stop/1: Processing", "0");
            paramDataGridView.Rows.Add(6, Dio0Status.ControllerIdentificationCode, "Controller identification code", "00");
            paramDataGridView.Rows.Add(6, Dio0Status.ErrorCode, "Identification code for error-occurred controller(s)", "00");
        }

        public string GetStatus(Dio0Status status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Dio0Status status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetStatus(Dio0Status.OperationMode1));
            stringBuilder.Append(GetStatus(Dio0Status.OperationMode2));
            stringBuilder.Append(GetStatus(Dio0Status.CommandProcessing));
            stringBuilder.Append("0");
            stringBuilder.Append("0");
            stringBuilder.Append("/");
            stringBuilder.Append(GetStatus(Dio0Status.ControllerIdentificationCode));
            stringBuilder.Append(GetStatus(Dio0Status.ErrorCode));

            return stringBuilder.ToString();
        }
    }
}
