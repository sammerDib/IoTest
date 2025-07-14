using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Service.Core.Recipe.Save;
using UnitySC.PM.EME.Service.Core.Shared.DateTimeProvider;
using UnitySC.PM.EME.Service.Core.Test.Calibration;
using UnitySC.PM.EME.Service.Core.Test.Recipe.Fixtures;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class AdaFileSaverTest
    {
        private static readonly DateTime
            _dateTime = Convert.ToDateTime("01/01/2024 08:00:00", new CultureInfo("fr-FR"));

        private readonly Mock<IDateTimeProvider> _dateTimeHelperMock =
            new Mock<IDateTimeProvider>();

        private readonly string _filePath = Path.Combine(Path.GetTempPath(), "result.ada");

        private readonly PMConfiguration _pmConfiguration = new PMConfiguration
        {
            ToolKey = 45,
            ChamberKey = 3,
            OutputAdaFolder = Path.GetTempPath(),
            OutputAdaFileNameTemplate = "result.ada",
            ChamberId = 1
        };

        private readonly RemoteProductionInfo _remoteProductionInfo = new RemoteProductionInfo
        {
            DFRecipeName = "WaferBestRecipe",
            ModuleRecipeName = "WaferBestModule",
            ModuleStartRecipeTime = _dateTime,
            ProcessedMaterial = new Material
            {
                AcquiredID = "AcquireId",
                CarrierID = "CarrierID",
                ControlJobID = "ControlJobID",
                DeviceID = "DeviceID",
                EquipmentID = "EquipmentID",
                GUIDWafer = new Guid(),
                JobStartTime = _dateTime,
                JobPosition = JobPosition.First,
                LoadportID = 1,
                LotID = "LotID",
                ProcessJobID = "JobID"
            }
        };

        private readonly Mock<IRecipeAcquisitionTemplateComposer> _templateComposerMock =
            new Mock<IRecipeAcquisitionTemplateComposer>();

        private readonly VignetteImageResult _vignetteVisible0Result =
            new VignetteImageResult(Path.GetTempPath(), "basename", 3, 4, ResultType.EME_Visible0,
                1.24.Micrometers(), 200.Millimeters(), "Acquisition1");

        private readonly VignetteImageResult _vignetteVisible90Result =
            new VignetteImageResult(Path.GetTempPath(), "basename", 3, 4, ResultType.EME_Visible90,
                1.24.Micrometers(), 100.Millimeters(), "Acquisition2");
        
        private readonly FullImageResult _fullImageVisible0Result =
            new FullImageResult(Path.GetTempPath(), "basename", ResultType.EME_Visible0,
                1.24.Micrometers(), 200.Millimeters(), "Acquisition1");

        private readonly FullImageResult _fullimageVisible90Result =
            new FullImageResult(Path.GetTempPath(), "basename", ResultType.EME_Visible90,
                1.24.Micrometers(), 100.Millimeters(), "Acquisition2");

        private AdaFileSaver _adaFileSaver;

        [TestInitialize]
        public void Setup()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            _templateComposerMock.Setup(x => x.GetAdaFileName(It.IsAny<Core.Recipe.RecipeAdapter>()))
                .Returns(_filePath);
            _dateTimeHelperMock.Setup(x => x.GetDateTimeNow()).Returns(_dateTime);

            _adaFileSaver = new AdaFileSaver(_pmConfiguration, _templateComposerMock.Object, _dateTimeHelperMock.Object,
                new FakeCalibrationManager());
        }

        [TestMethod]
        public void ShouldSaveAdaFile()
        {
            //Arrange
            var recipe = RecipeFixture.CreateRecipe(_remoteProductionInfo);
            //Act
            _adaFileSaver.GenerateFile(recipe, new List<IAcquisitionImageResult> { _vignetteVisible0Result, _vignetteVisible90Result });

            //Assert
            string result = File.ReadAllText(_filePath);
            result = result.Replace(Path.GetTempPath(), "TEMP_PATH").TrimEnd();
            string expected = @"[HEADER]
Version=10
ModuleNbr=2
[INFO WAFER]
StartProcess=01-01-2024 08:00:00
ADCOutputDataFilePath=default
SummaryFile=default
CarrierStatus=0
WaferID=AcquireId
SlotID=0
LoadPortID=1
StepID=default
DeviceID=DeviceID
JobID=JobID
LotID=LotID
ToolRecipe=WaferBestRecipe
ADCRecipeFileName=default
CorrectorsEnabled=0
WaferType=Notch 200mm
NewOptimalCameraMatrix_1=1
NewOptimalCameraMatrix_2=5
NewOptimalCameraMatrix_3=7
NewOptimalCameraMatrix_4=1
NewOptimalCameraMatrix_5=4
NewOptimalCameraMatrix_6=1
NewOptimalCameraMatrix_7=0
NewOptimalCameraMatrix_8=0
NewOptimalCameraMatrix_9=1
CameraMatrix_1=1
CameraMatrix_2=5
CameraMatrix_3=2
CameraMatrix_4=1
CameraMatrix_5=5
CameraMatrix_6=1
CameraMatrix_7=0
CameraMatrix_8=0
CameraMatrix_9=1
DistortionMatrix_1=1
DistortionMatrix_2=5
DistortionMatrix_3=2
DistortionMatrix_4=1
DistortionMatrix_5=1
TranslationVector_1=4
TranslationVector_2=5
TranslationVector_3=2
RotationVector_1=1
RotationVector_2=5
RotationVector_3=2
AlignerAngle=0
[Module 0]
ResultType=EME_Visible0
ToolKey=45
ChamberKey=3
ADCInputDataFilePath=TEMP_PATH
max_input_mosaic_image=1
image_mosaic_0=basename
nb_column_0=4
nb_line_0=3
ModuleID=9
ChannelID=0
ChamberID=1
pixel_size_x_0=1.24
pixel_size_y_0=1.24
wafer_center_x_0=80645
wafer_center_y_0=80645
[Module 1]
ResultType=EME_Visible90
ToolKey=45
ChamberKey=3
ADCInputDataFilePath=TEMP_PATH
max_input_mosaic_image=1
image_mosaic_0=basename
nb_column_0=4
nb_line_0=3
ModuleID=9
ChannelID=1
ChamberID=1
pixel_size_x_0=1.24
pixel_size_y_0=1.24
wafer_center_x_0=40322
wafer_center_y_0=40322";
            Assert.AreEqual(expected, result);
        }
        
        
        [TestMethod]
        public void ShouldSaveAdaFileWithFullImage()
        {
            //Arrange
            var recipe = RecipeFixture.CreateRecipe(_remoteProductionInfo, true);
            //Act
            _adaFileSaver.GenerateFile(recipe, new List<IAcquisitionImageResult> { _fullImageVisible0Result, _fullimageVisible90Result });

            //Assert
            string result = File.ReadAllText(_filePath);
            result = result.Replace(Path.GetTempPath(), "TEMP_PATH").TrimEnd();
            string expected = @"[HEADER]
Version=10
ModuleNbr=2
[INFO WAFER]
StartProcess=01-01-2024 08:00:00
ADCOutputDataFilePath=default
SummaryFile=default
CarrierStatus=0
WaferID=AcquireId
SlotID=0
LoadPortID=1
StepID=default
DeviceID=DeviceID
JobID=JobID
LotID=LotID
ToolRecipe=WaferBestRecipe
ADCRecipeFileName=default
CorrectorsEnabled=0
WaferType=Notch 200mm
NewOptimalCameraMatrix_1=1
NewOptimalCameraMatrix_2=5
NewOptimalCameraMatrix_3=7
NewOptimalCameraMatrix_4=1
NewOptimalCameraMatrix_5=4
NewOptimalCameraMatrix_6=1
NewOptimalCameraMatrix_7=0
NewOptimalCameraMatrix_8=0
NewOptimalCameraMatrix_9=1
CameraMatrix_1=1
CameraMatrix_2=5
CameraMatrix_3=2
CameraMatrix_4=1
CameraMatrix_5=5
CameraMatrix_6=1
CameraMatrix_7=0
CameraMatrix_8=0
CameraMatrix_9=1
DistortionMatrix_1=1
DistortionMatrix_2=5
DistortionMatrix_3=2
DistortionMatrix_4=1
DistortionMatrix_5=1
TranslationVector_1=4
TranslationVector_2=5
TranslationVector_3=2
RotationVector_1=1
RotationVector_2=5
RotationVector_3=2
AlignerAngle=0
[Module 0]
ResultType=EME_Visible0
ToolKey=45
ChamberKey=3
ADCInputDataFilePath=TEMP_PATH
max_input_full_image=1
ResultType_0=EME_Visible0
image_0=basename.tiff
ModuleID=9
ChannelID=0
ChamberID=1
pixel_size_x_0=1.24
pixel_size_y_0=1.24
wafer_center_x_0=80645
wafer_center_y_0=80645
[Module 1]
ResultType=EME_Visible90
ToolKey=45
ChamberKey=3
ADCInputDataFilePath=TEMP_PATH
max_input_full_image=1
ResultType_0=EME_Visible90
image_0=basename.tiff
ModuleID=9
ChannelID=1
ChamberID=1
pixel_size_x_0=1.24
pixel_size_y_0=1.24
wafer_center_x_0=40322
wafer_center_y_0=40322";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldSaveAdaFileWithNoRemoteProductionInfo()
        {
            //Arrange
            var recipe = RecipeFixture.CreateRecipe();

            //Act
            _adaFileSaver.GenerateFile(recipe, new List<IAcquisitionImageResult> { _vignetteVisible0Result, _vignetteVisible90Result });           
            //Assert
            string result = File.ReadAllText(_filePath);
            result = result.Replace(Path.GetTempPath(), "TEMP_PATH").TrimEnd();
            string expected = @"[HEADER]
Version=10
ModuleNbr=2
[INFO WAFER]
StartProcess=01-01-2024 08:00:00
ADCOutputDataFilePath=default
SummaryFile=default
CarrierStatus=FirstAndLast
WaferID=0-000.00
SlotID=1
LoadPortID=1
StepID=default
DeviceID=default
JobID=default
LotID=default
ToolRecipe=default
ADCRecipeFileName=default
CorrectorsEnabled=0
WaferType=Notch 200mm
NewOptimalCameraMatrix_1=1
NewOptimalCameraMatrix_2=5
NewOptimalCameraMatrix_3=7
NewOptimalCameraMatrix_4=1
NewOptimalCameraMatrix_5=4
NewOptimalCameraMatrix_6=1
NewOptimalCameraMatrix_7=0
NewOptimalCameraMatrix_8=0
NewOptimalCameraMatrix_9=1
CameraMatrix_1=1
CameraMatrix_2=5
CameraMatrix_3=2
CameraMatrix_4=1
CameraMatrix_5=5
CameraMatrix_6=1
CameraMatrix_7=0
CameraMatrix_8=0
CameraMatrix_9=1
DistortionMatrix_1=1
DistortionMatrix_2=5
DistortionMatrix_3=2
DistortionMatrix_4=1
DistortionMatrix_5=1
TranslationVector_1=4
TranslationVector_2=5
TranslationVector_3=2
RotationVector_1=1
RotationVector_2=5
RotationVector_3=2
AlignerAngle=0
[Module 0]
ResultType=EME_Visible0
ToolKey=45
ChamberKey=3
ADCInputDataFilePath=TEMP_PATH
max_input_mosaic_image=1
image_mosaic_0=basename
nb_column_0=4
nb_line_0=3
ModuleID=9
ChannelID=0
ChamberID=1
pixel_size_x_0=1.24
pixel_size_y_0=1.24
wafer_center_x_0=80645
wafer_center_y_0=80645
[Module 1]
ResultType=EME_Visible90
ToolKey=45
ChamberKey=3
ADCInputDataFilePath=TEMP_PATH
max_input_mosaic_image=1
image_mosaic_0=basename
nb_column_0=4
nb_line_0=3
ModuleID=9
ChannelID=1
ChamberID=1
pixel_size_x_0=1.24
pixel_size_y_0=1.24
wafer_center_x_0=40322
wafer_center_y_0=40322";
            Assert.AreEqual(expected, result);
        }

        [TestCleanup]
        public void TearDown()
        {
            File.Delete(_filePath);
            File.Delete(Path.GetTempPath() + @"\result.tmp.ada");
        }
    }
}
