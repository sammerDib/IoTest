using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.AlgorithmManager;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

using Point = System.Windows.Point;

namespace UnitySC.PM.DMT.Service.Flows.SaveImage
{
    public class SaveImageFlow : FlowComponent<SaveImageInput, SaveImageResult, SaveImageConfiguration>
    {
        private readonly IDMTAlgorithmManager _algorithmManager;

        private readonly ICalibrationManager _calibrationManager;

        private readonly DbRegisterAcquisitionServiceProxy _dbRegisterAcqResultServiceProxy;

        public const int MaximumNumberOfSteps = 4;

        public SaveImageFlow(SaveImageInput input, IDMTAlgorithmManager algorithmManager, ICalibrationManager calibrationManager,
                              DbRegisterAcquisitionServiceProxy dbRegisterAcquisitionResultService)
            : base(input, "SaveImageFlow")
        {
            _algorithmManager = algorithmManager;
            _calibrationManager = calibrationManager;
            _dbRegisterAcqResultServiceProxy = dbRegisterAcquisitionResultService;
        }

        protected override void Process()
        {
            USPImageMil imageToSave;
            if (!(Input.ImageDataToSave is null))
            {
                imageToSave = Input.ImageDataToSave.ConvertToUSPImageMil(!Input.Keep32BitsDepth);
            }
            else
            {
                bool isBrightFieldMeasure = Input.DMTResultType == DMTResult.BrightField_Front || Input.DMTResultType == DMTResult.BrightField_Back;
                if (isBrightFieldMeasure && Input.ApplyUniformityCorrection)
                {
                    imageToSave = ApplyUniformityCorrection();
                }
                else
                {
                    imageToSave = Input.ImageMilToSave;
                }
            }

            ResultType restyp = ResultType.Empty;
            ResultState acqResState = ResultState.Error;
            try
            {
                restyp = Input.DMTResultType.ToResultType();
            }
            catch
            {
                restyp = ResultType.Empty;
            }

            try
            {
                acqResState = SaveImageAndUpdateAda(imageToSave, restyp);
            }
            finally
            {
                if (Input.InternalDbResItemId != -1)
                {
                    FinalizeRegisterImage(acqResState);
                }

                imageToSave.Dispose();
            }
        }

        private ResultState SaveImageAndUpdateAda(USPImageMil imageToSave, ResultType restyp)
        {
            ResultState acqResState;
            var transform = _calibrationManager.GetPerspectiveCalibrationForSide(Input.ImageSide);
            if (Configuration.UsePerspectiveCalibration && !(transform is null))
            {
                SetProgressMessage($"Applying perspective calibration before saving image {Input.ImageName} to the disk");

                using (var transformedImage = transform.Transform(imageToSave))
                {
                    SaveImage(transformedImage);
                    SetProgressMessage("Successfully transformed and saved image to disk. Sending acquisition result to the database");
                }
            }
            else
            {
                SetProgressMessage($"Saving image {Input.ImageName} without perspective calibration to the disk");
                SaveImage(imageToSave);
            }

            SetProgressMessage("Successfully saved image to disk. Updating internal ADA inforamtion");
            acqResState = ResultState.Ok;

            RegisterAdaImage(restyp);
            Result.SavePath = Input.SaveFullPath;
            Result.ImageName = Input.ImageName;
            Result.ImageSide = Input.ImageSide;
            return acqResState;
        }

        private USPImageMil ApplyUniformityCorrection()
        {
            USPImageMil imageToSave;
            var transform = _calibrationManager.GetPerspectiveCalibrationForSide(Input.ImageSide);
            if (transform is null)
            {
                throw new Exception("Cannot retrieve informations from perspective calibration");
            }
            var pixelSize = (float)transform.PixelSize.Micrometers().Millimeters;
            var waferDimensions = Input.RemoteProductionInfo?.ProcessedMaterial?.WaferDimension;
            var waferRadiusInMm = waferDimensions != null ? (float)waferDimensions.Millimeters / 2 : 150.0f;
            var imageData = ImageUtils.CreateImageDataFromUSPImageMil(Input.ImageMilToSave);
            var uniformityCorrectionImage = _calibrationManager.GetUniformityCorrectionCalibImageBySide(Input.ImageSide);
            if (uniformityCorrectionImage != null)
            {
                var correctedImage = PSDCalibration.ApplyUniformityCorrection(imageData,
                    uniformityCorrectionImage, pixelSize, waferRadiusInMm,
                    Input.UniformityCorrectionTargetSaturationLevel,
                    Input.UniformityCorrectionAcceptableRatioOfSaturatedPixels);
                imageToSave = correctedImage.ConvertToUSPImageMil(!Input.Keep32BitsDepth);
            }
            else
            {
                Logger.Error($"Uniformity calibration is missing : cannot apply correction");
                imageToSave = Input.ImageMilToSave;
            }

            return imageToSave;
        }

        private void SaveImage(USPImageMil imageToSave)
        {
            CheckCancellation();

            
            string fullPathName = Input.SaveFullPath;
            string acquisitionFileName = Path.GetFileName(fullPathName);
            string acquisitionDirectory = Path.GetDirectoryName(fullPathName);
            SetProgressMessage($"Writing image to {fullPathName}");
            try
            {
                imageToSave.Save(fullPathName);
            }
            catch (Exception)
            {
                Logger.Debug($"Failed to save image to {fullPathName}");
                throw;
            }

            string thumbFileName = InPreRegisterAcqHelper.FullAcqFilePathThumbnail(acquisitionDirectory, acquisitionFileName);
            SaveThumbNailPng(imageToSave, Path.Combine(acquisitionDirectory, thumbFileName));
        }

        private void FinalizeRegisterImage(ResultState state)
        {
            if (Input.InternalDbResItemId != -1)
            {
                Logger.Debug($"Register Done <{Input.DMTResultType}> InternalDBResItemId  = {Input.InternalDbResItemId} ResultState = {state}");
                _dbRegisterAcqResultServiceProxy.UpdateResultAcquisitionState(Input.InternalDbResItemId, state);
            }
        }

        private void SaveThumbNailPng(USPImageMil procimg, string path)
        {
            try
            {
                var bitmap = procimg.ConvertToBitmap();
                const int height = 256;
                int width = procimg.Width * height / procimg.Height;
                var destRect = new Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height);

                destImage.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    bitmap.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception)
            {
                Logger.Debug("Failed to save the thumbnail image");
            }
        }

        protected void RegisterAdaImage(ResultType resultType)
        {
            if (resultType == ResultType.Empty)
                return;

            Side side = resultType.GetSide();

            lock (Input.AdaWriterLock)
            {
                Input.AdaWriter.AddFullImage(resultType, Input.SaveFullPath);
                Input.AdaWriter.WriteMetaData(resultType, "ExposureTime", $"{Input.ExposureTimeMs}");
                var calibration = _calibrationManager.GetPerspectiveCalibrationForSide(side);
                if (Configuration.UsePerspectiveCalibration && !(calibration is null))
                {
                    var center = calibration.MicronsToCalibratedImage(new Point(0, 0));
                    Input.AdaWriter.WriteRectangularMatrix(resultType, calibration.PixelSize, calibration.PixelSize, center.X, center.Y, /*angle*/0.0);
                }
            }
        }
    }
}
