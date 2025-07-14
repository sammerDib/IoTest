using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.AlgorithmManager;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Proxy;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class SaveImageFlowDummy : SaveImageFlow
    {
        public SaveImageFlowDummy(SaveImageInput input, IDMTAlgorithmManager algorithmManager, ICalibrationManager calibrationManager, DbRegisterAcquisitionServiceProxy dbRegisterAcquisitionResultService)
            : base(input, algorithmManager, calibrationManager, dbRegisterAcquisitionResultService)
        {
        }

        protected override void Process()
        {
            Result.SavePath = Input.SaveFullPath;
            Result.ImageSide = Input.ImageSide;
            Result.ImageName = Input.ImageName;

            var imageToSave = !(Input.ImageDataToSave is null)
                ? Input.ImageDataToSave.ConvertToUSPImageMil(!Input.Keep32BitsDepth)
                : Input.ImageMilToSave;
            imageToSave.ToServiceImage().SaveToFile(Result.SavePath);

            ResultType restyp = ResultType.Empty;
            
            restyp = Input.DMTResultType.ToResultType();
            RegisterAdaImage(restyp);

        }
    }
}
