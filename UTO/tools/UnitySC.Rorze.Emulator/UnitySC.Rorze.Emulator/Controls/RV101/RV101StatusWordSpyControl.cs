using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RV101
{
    internal enum Rv101Status
    {
        OperationMode,
        OriginReturnCompletion,
        ProcessingCommand,
        OperationStatus,
        MotionSpeed,
        ErrorCode
    }
    internal partial class Rv101StatusWordSpyControl : InputsOutputsControl
    {
        public Rv101StatusWordSpyControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Rv101Status.OperationMode, "0: Initializing/1: Remote/2: Maintenance/3: Recovery", "0");
            paramDataGridView.Rows.Add(2, Rv101Status.OriginReturnCompletion, "0: Not completed/1: Completed", "0");
            paramDataGridView.Rows.Add(3, Rv101Status.ProcessingCommand, "0: Stop/1: Processing", "0");
            paramDataGridView.Rows.Add(4, Rv101Status.OperationStatus, "0: Stop/1: Moving/2: Temporarily stop", "0");
            paramDataGridView.Rows.Add(5, Rv101Status.MotionSpeed, "0: Normal/1 to A: Maintenance", "0");
            paramDataGridView.Rows.Add(6, Rv101Status.ErrorCode, "Identification code for error-occurred controller(s)", "0000");
        }

        public string GetStatus(Rv101Status status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Rv101Status status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetStatus(Rv101Status.OperationMode));
            stringBuilder.Append(GetStatus(Rv101Status.OriginReturnCompletion));
            stringBuilder.Append(GetStatus(Rv101Status.ProcessingCommand));
            stringBuilder.Append(GetStatus(Rv101Status.OperationStatus));
            stringBuilder.Append(GetStatus(Rv101Status.MotionSpeed));
            stringBuilder.Append("/");
            stringBuilder.Append(GetStatus(Rv101Status.ErrorCode));

            return stringBuilder.ToString();
        }
    }
}
