using System.IO;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.DMT.Service.Flows.SaveImage
{
    public class SaveMaskFlow : FlowComponent<SaveMaskInput, SaveMaskResult, SaveImageConfiguration>
    {
        private readonly ICalibrationManager _calibrationManager;
        
        public SaveMaskFlow(SaveMaskInput input, ICalibrationManager calibrationManager) : base(input, "SaveMaskFlow")
        {
            _calibrationManager = calibrationManager;
        }

        protected override void Process()
        {

            USPImageMil maskToSave = Input.MaskToSave.ConvertToUSPImageMil(false);
            try
            {
                SetProgressMessage($"Starting {Name} for {Input.MaskSide}side");
                SaveMask(maskToSave);

                SetProgressMessage("Updating ADA file.");
                UpdateAdaFile();
                Result.MaskSide = Input.MaskSide;
                Result.SavePath = Input.SaveFullPath;
                Result.MaskFileName = Path.GetDirectoryName(Input.SaveFullPath);
                Result.SavePath = Path.GetFileName(Input.SaveFullPath);
            }
            finally
            {
                maskToSave.Dispose();
            }
        }

        private void UpdateAdaFile()
        {
            string maskFileName = Path.GetFileName(Input.SaveFullPath);
            lock (Input.AdaWriterLock)
            {
                foreach (var resultType in Input.RecipeResults)
                {
                    Input.AdaWriterForSide.WriteMetaData(resultType, "MaskFileName", maskFileName);
                }                    
            }
        }

        private void SaveMask(USPImageMil maskToSave)
        {
            var perspectiveCalibration = _calibrationManager.GetPerspectiveCalibrationForSide(Input.MaskSide);
            if (Configuration.UsePerspectiveCalibration && !(perspectiveCalibration is null))
            {
                using (USPImageMil calibratedImage = perspectiveCalibration.Transform(maskToSave))
                {
                    calibratedImage.Save(Input.SaveFullPath);
                    SetProgressMessage($"Successfully saved mask with perspective calibration to {Input.SaveFullPath}");
                }
            }
            else
            {
                maskToSave.Save(Input.SaveFullPath);
                SetProgressMessage($"Successfully saved mask without perspective calibration to {Input.SaveFullPath}");
            }
        }
    }
}
