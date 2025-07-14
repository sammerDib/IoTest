using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Shared.UI.Message
{
    public class CurvatureCalibrationTerminated
    {
        public Side Side;
        public bool IsSuccess;
        public double CurvatureDynamicsCoeff;

        public CurvatureCalibrationTerminated(Side side, bool isSuccess, double curvatureDynamicsCoeff)
        {
            Side = side;
            IsSuccess = isSuccess;
            CurvatureDynamicsCoeff = curvatureDynamicsCoeff;
        }
    }
}
