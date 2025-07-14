using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestPSDCalibration
    {
        /*
        [TestMethod]
        public void PSDCalibration_wrapper_nominal_case()
        {
            // Given

            var cam1 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath(".\\..\\..\\Tests\\Data\\calib\\camera\\RH1_Reflectivity.tiff"));
            var cam2 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath(".\\..\\..\\Tests\\Data\\calib\\camera\\RH2_Reflectivity.tiff"));
            var cam3 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath(".\\..\\..\\Tests\\Data\\calib\\camera\\RV1_Reflectivity.tiff"));
            var cam4 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath(".\\..\\..\\Tests\\Data\\calib\\camera\\RV2_Reflectivity.tiff"));
            var cam5 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath(".\\..\\..\\Tests\\Data\\calib\\camera\\Standard_Reflectivity.tiff"));
            var cam6 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath(".\\..\\..\\Tests\\Data\\calib\\camera\\T+_Reflectivity.tiff"));

            var camImg = new ImageData[] { cam1, cam2, cam3, cam4, cam5, cam6 };
            Point2f leftCheckerBoard = new Point2f(-130.97f, 31.5f);
            Point2f topCheckerBoard = new Point2fPoint2f(-31.5, 130.97);
            Point2f rightCheckerBoard = new Point2f(67.97, 31.5);
            Point2f bottomCheckerBoard = new Point2f(-31.5, -67.97);
            CheckerBoardsOrigins checkerBoardsOrigins = new CheckerBoardsOrigins(leftCheckerBoard, bottomCheckerBoard, rightCheckerBoard, topCheckerBoard);

            bool useAllCheckerBoards = true;
            int squareXNumber = 11;
            int squareYNumber = 11;
            float squareSizeMm = 7.0f;
            float pixelSize = 0.030f; //30 microns
            CheckerBoardsSettings checkerBoardsSettings = new CheckerBoardsSettings(checkerBoardsOrigins, squareXNumber, squareYNumber, squareSizeMm, pixelSize, useAllCheckerBoards);

            float edgeExclusion = 50;
            float waferRadius = 150;
            float nbPtsScreen = 500;
            float frangePixels = 32;
            float screenPixelSize = 0.2451f; //245.1 microns
            InputSystemParameters parameters = new InputSystemParameters(checkerBoardsSettings, edgeExclusion, waferRadius, nbPtsScreen, frangePixels, screenPixelSize);

            // When : Run image registration
            CalibrationParameters intrinsicCameraParams = PSDCalibration.CalibrateCamera(camImg, checkerBoardsSettings, false);

            //var checkerBoardImg = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath(".\\..\\..\\Tests\\Data\\calib\\system\\WI.png"));
            //ImageData phaseMapX = new ImageData(); //Need to read 32float image
            //ImageData phaseMapY = new ImageData(); //Need to read 32float image
            //SystemParameters system = PSDCalibration.CalibrateSystem(phaseMapX, phaseMapY, checkerBoardImg, intrinsicCameraParams.CameraIntrinsic, intrinsicCameraParams.Distortion, parameters);

            // Then
        }*/
    }
}
