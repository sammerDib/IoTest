using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RR75x
{
    internal enum Rr75xStatus
    {
        OperationMode,
        OriginSearchCompleteFlag,
        CommandProcessing,
        OperationStatus,
        MotionSpeed,
        ControllerIdentificationCode,
        ErrorCode
    }
    internal partial class Rr75xStatusWordSpyControl : InputsOutputsControl
    {
        public Rr75xStatusWordSpyControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Rr75xStatus.OperationMode, "0: Initializing/1: Remote/2: Maintenance/3: Recovery", "0");
            paramDataGridView.Rows.Add(2, Rr75xStatus.OriginSearchCompleteFlag, "0: Not completed/1: Completed", "0");
            paramDataGridView.Rows.Add(3, Rr75xStatus.CommandProcessing, "0: Stop/1: Processing", "0");
            paramDataGridView.Rows.Add(4, Rr75xStatus.OperationStatus, "0: Stop/1: Moving/2: Pause", "0");
            paramDataGridView.Rows.Add(5, Rr75xStatus.MotionSpeed, "0: Normal/1 to 9, A to K: Set speed", "0");
            paramDataGridView.Rows.Add(6, Rr75xStatus.ControllerIdentificationCode, "Controller identification code", "00");
            paramDataGridView.Rows.Add(6, Rr75xStatus.ErrorCode, "Identification code for error-occurred controller(s)", "00");
        }

        public string GetStatus(Rr75xStatus status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Rr75xStatus status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetStatus(Rr75xStatus.OperationMode));
            stringBuilder.Append(GetStatus(Rr75xStatus.OriginSearchCompleteFlag));
            stringBuilder.Append(GetStatus(Rr75xStatus.CommandProcessing));
            stringBuilder.Append(GetStatus(Rr75xStatus.OperationStatus));
            stringBuilder.Append(GetStatus(Rr75xStatus.MotionSpeed));
            stringBuilder.Append("/");
            stringBuilder.Append(GetStatus(Rr75xStatus.ControllerIdentificationCode));
            stringBuilder.Append(GetStatus(Rr75xStatus.ErrorCode));

            return stringBuilder.ToString();
        }
    }
}