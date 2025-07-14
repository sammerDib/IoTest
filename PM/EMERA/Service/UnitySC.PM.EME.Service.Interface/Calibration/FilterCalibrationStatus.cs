namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    public class FilterCalibrationStatus
    {

        public FilterCalibrationStatus() { }
        public FilterCalibrationStatus(FilterCalibrationState state, string message = null)
        {
            State = state;
            Message = message;
        }

        public FilterCalibrationState State { get; set; }

        public string Message { get; set; }

    }
}
