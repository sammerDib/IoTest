using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Data.Enum;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    public interface ICalibrationManager
    {
        GlobalTopoCameraCalibrationResult GetGlobalTopoCameraCalibrationResultBySide(Side side);

        GlobalTopoSystemCalibrationResult GetGlobalTopoSystemCalibrationResultBySide(Side side);

        DMTTransform GetPerspectiveCalibrationForSide(Side side);

        ImageData GetUniformityCorrectionCalibImageBySide(Side side);

        void Init();
        
        void Shutdown();
    }
}
