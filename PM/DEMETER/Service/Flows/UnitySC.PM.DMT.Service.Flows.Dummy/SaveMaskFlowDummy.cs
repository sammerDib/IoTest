using System.IO;

using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class SaveMaskFlowDummy : SaveMaskFlow
    {
        public SaveMaskFlowDummy(SaveMaskInput input, ICalibrationManager calibrationManager) : base(input, calibrationManager)
        {
        }

        protected override void Process()
        {
            Result.MaskSide = Input.MaskSide;
            Result.SavePath = Input.SaveFullPath;
            Result.MaskFileName = Path.GetDirectoryName(Input.SaveFullPath);
            Result.SavePath = Path.GetFileName(Input.SaveFullPath);
        }
    }
}
