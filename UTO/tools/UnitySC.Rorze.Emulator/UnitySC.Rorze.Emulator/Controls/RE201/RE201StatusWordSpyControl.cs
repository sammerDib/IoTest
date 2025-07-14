using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RE201
{
    internal enum Re201Status
    {
        OperationMode,
        OriginReturnCompletion,
        ProcessingCommand,
        OperationStatus,
        MotionSpeed,
        ErrorCode
    }
    internal partial class Re201StatusWordSpyControl : InputsOutputsControl
    {
        public Re201StatusWordSpyControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Re201Status.OperationMode, "0: Initializing/1: Remote/2: Maintenance/3: Recovery", "0");
            paramDataGridView.Rows.Add(2, Re201Status.OriginReturnCompletion, "0: Not completed/1: Completed", "0");
            paramDataGridView.Rows.Add(3, Re201Status.ProcessingCommand, "0: Stop/1: Processing", "0");
            paramDataGridView.Rows.Add(4, Re201Status.OperationStatus, "0: Stop/1: Moving/2: Temporarily stop", "0");
            paramDataGridView.Rows.Add(5, Re201Status.MotionSpeed, "0: Normal/1 to A: Maintenance", "0");
            paramDataGridView.Rows.Add(6, Re201Status.ErrorCode, "Identification code for error-occurred controller(s)", "0000");
        }

        public string GetStatus(Re201Status status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Re201Status status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetStatus(Re201Status.OperationMode));
            stringBuilder.Append(GetStatus(Re201Status.OriginReturnCompletion));
            stringBuilder.Append(GetStatus(Re201Status.ProcessingCommand));
            stringBuilder.Append(GetStatus(Re201Status.OperationStatus));
            stringBuilder.Append(GetStatus(Re201Status.MotionSpeed));
            stringBuilder.Append("/");
            stringBuilder.Append(GetStatus(Re201Status.ErrorCode));

            return stringBuilder.ToString();
        }
    }
}