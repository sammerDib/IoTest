using System.Text;

namespace UnitySC.Rorze.Emulator.Controls.RA420
{
    internal enum Ra420Status
    {
        OperationMode,
        OriginSearchCompleteFlag,
        CommandProcessing,
        OperationStatus,
        MotionSpeed,
        ControllerIdentificationCode,
        ErrorCode
    }
    internal partial class Ra420StatusWordSpyControl : InputsOutputsControl
    {
        public Ra420StatusWordSpyControl()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            paramDataGridView.Rows.Clear();

            paramDataGridView.Rows.Add(1, Ra420Status.OperationMode, "0: Initializing/1: Remote/2: Maintenance/3: Recovery", "0");
            paramDataGridView.Rows.Add(2, Ra420Status.OriginSearchCompleteFlag, "0: Not completed/1: Completed", "0");
            paramDataGridView.Rows.Add(3, Ra420Status.CommandProcessing, "0: Stop/1: Processing", "0");
            paramDataGridView.Rows.Add(4, Ra420Status.OperationStatus, "0: Stop/1: Moving/2: Temporarily stop", "0");
            paramDataGridView.Rows.Add(5, Ra420Status.MotionSpeed, "0: Normal/1 to A: Maintenance", "0");
            paramDataGridView.Rows.Add(6, Ra420Status.ControllerIdentificationCode, "Controller identification code", "00");
            paramDataGridView.Rows.Add(6, Ra420Status.ErrorCode, "Identification code for error-occurred controller(s)", "00");
        }

        public string GetStatus(Ra420Status status)
        {
            return GetStatus((int)status);
        }

        public void SetStatus(Ra420Status status, string value)
        {
            SetStatus((int)status, value);
        }

        public override string GetConcatenatedStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(GetStatus(Ra420Status.OperationMode));
            stringBuilder.Append(GetStatus(Ra420Status.OriginSearchCompleteFlag));
            stringBuilder.Append(GetStatus(Ra420Status.CommandProcessing));
            stringBuilder.Append(GetStatus(Ra420Status.OperationStatus));
            stringBuilder.Append(GetStatus(Ra420Status.MotionSpeed));
            stringBuilder.Append("/");
            stringBuilder.Append(GetStatus(Ra420Status.ControllerIdentificationCode));
            stringBuilder.Append(GetStatus(Ra420Status.ErrorCode));

            return stringBuilder.ToString();
        }
    }
}