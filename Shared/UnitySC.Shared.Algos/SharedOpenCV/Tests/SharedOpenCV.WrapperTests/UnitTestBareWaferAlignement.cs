using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

using System.Collections.Generic;

using UnitySCSharedAlgosOpenCVWrapper;
using System.Linq;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestBareWaferAlignement
    {
        [TestMethod]
        public void Expect_bwa_to_fail_without_edge_image()
        {
            string currentPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(currentPath);

            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };
            var images = new List<BareWaferAlignmentImageData>();

            // when
            int edgeDetectionVersion = 2;
            int notchDetectionVersion = 3;
            int cannyThreshold = 200;
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold, currentPath);

            // then
            Assert.AreEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should fail without edge image");

            if (!Directory.EnumerateFileSystemEntries(currentPath).Any())
                Directory.Delete(currentPath);
        }

        [TestMethod]
        public void Expect_bwa_to_fail_with_1_edge_image()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };
            var images = new List<BareWaferAlignmentImageData>();
            images.Add(new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE });

            // when
            int notchDetectionVersion = 3;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold);

            // then
            Assert.AreEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should fail with one edge image");
        }

        [TestMethod]
        public void Expect_bwa_to_fail_with_2_edge_image()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };
            var images = new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                                 new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE } };

            // when
            int notchDetectionVersion = 3;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold);

            // then
            Assert.AreEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should fail with two edge image");
        }

        [TestMethod]
        public void Expect_bwa_to_continue_processing_with_3_edges()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };

            var images =
                new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE } };
            // when
            int notchDetectionVersion = 3;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold);

            // then
            Assert.AreNotEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should not fail with missing edge image if 3 edge images are given");
        }

        [TestMethod]
        public void Expect_bwa_for_notch_wafer_to_fail_without_notch_image()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };
            var images =
                new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE } };

            // when
            int notchDetectionVersion = 3;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold);

            // then
            Assert.AreEqual(StatusCode.MISSING_NOTCH_IMAGE, result.Status.Code);
        }

        [TestMethod]
        public void Expect_bwa_to_fail_if_empty_images_are_provided()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };
            var images =
                new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.NOTCH } };

            // when
            try
            {
                int notchDetectionVersion = 3;
                int edgeDetectionVersion = 2;
                int cannyThreshold = 200;
                BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold);
                Assert.Fail("BWA should fail with empty images");
            }
            catch (Exception)
            {
                // NO OP
            }
        }

        [TestMethod]
        public void Expect_bwa_to_work_on_trivial_image()
        {
            string reportPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            string imageFolder = "good_images";
                Directory.CreateDirectory(reportPath);

            DirectoryInfo dir = new DirectoryInfo(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder));

            FileInfo[] Files = dir.GetFiles("*.png");

                List<string> fileNames = new List<string>();
                //Xcoords & Ycoords coordinates in the stage referential
                List<int> Xcoords = new List<int>();
                List<int> Ycoords = new List<int>();
                List<EdgePosition> edgePositions = new List<EdgePosition>();

                foreach (FileInfo file in Files)
                {
                    string fileName = file.Name;

                    switch (fileName)
                    {
                    case "BOTTOM.png":
                            Xcoords.Add(0);
                            Ycoords.Add(-150000);
                            edgePositions.Add(EdgePosition.BOTTOM);
                            break;
                    case "LEFT.png":
                            Xcoords.Add(-150000);
                            Ycoords.Add(0);
                            edgePositions.Add(EdgePosition.LEFT);
                            break;
                    case "RIGHT.png":
                            Xcoords.Add(150000);
                            Ycoords.Add(0);
                            edgePositions.Add(EdgePosition.RIGHT);
                            break;
                    case "TOP.png":
                            Xcoords.Add(0);
                            Ycoords.Add(150000);
                            edgePositions.Add(EdgePosition.TOP);
                            break;
                    }
                    fileNames.Add(fileName);
                }

                // given
                var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };
                double pixelSize = 2.125;
                double expectedShiftXInMicrons = -494;
                double expectedShiftYInMicrons = -461;
                double expectedAngleInDegrees = 0.002;

                var edgeImage1 =
                Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[1]));
                edgeImage1.ExpectedShape = WaferEdgeShape.EDGE;
                edgeImage1.Centroid.X = Xcoords[1];
                edgeImage1.Centroid.Y = Ycoords[1];
                edgeImage1.Scale.X = pixelSize;
                edgeImage1.Scale.Y = pixelSize;
                edgeImage1.EdgePosition = edgePositions[1];

                var edgeImage2 =
                Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[2]));
                edgeImage2.ExpectedShape = WaferEdgeShape.EDGE;
                edgeImage2.Centroid.X = Xcoords[2];
                edgeImage2.Centroid.Y = Ycoords[2];
                edgeImage2.Scale.X = pixelSize;
                edgeImage2.Scale.Y = pixelSize;
                edgeImage2.EdgePosition = edgePositions[2];

                var edgeImage3 =
                Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[3]));
                edgeImage3.ExpectedShape = WaferEdgeShape.EDGE;
                edgeImage3.Centroid.X = Xcoords[3];
                edgeImage3.Centroid.Y = Ycoords[3];
                edgeImage3.Scale.X = pixelSize;
                edgeImage3.Scale.Y = pixelSize;
                edgeImage3.EdgePosition = edgePositions[3];



                var notchImage =
                Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[0]));
                notchImage.ExpectedShape = WaferEdgeShape.NOTCH;
                notchImage.Centroid.X = Xcoords[0];
                notchImage.Centroid.Y = Ycoords[0];
                notchImage.Scale.X = pixelSize;
                notchImage.Scale.Y = pixelSize;
                notchImage.EdgePosition = edgePositions[0];

                var images = new List<BareWaferAlignmentImageData> { edgeImage1, edgeImage2, edgeImage3, notchImage };

                // when

                int notchDetectionVersion = 3;
                int edgeDetectionVersion = 2;
                int cannyThreshold = 200;
                var resultWithReport = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold, reportPath, ReportOption.OverlayReport);

                var resultWithoutReport = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold);

                // Then

                double rotationToleranceInDegrees = 0.001;
                double shiftToleranceInMicrons = 40.0;

            Assert.AreEqual(StatusCode.OK, resultWithReport.Status.Code);
            Assert.AreEqual(expectedShiftXInMicrons, resultWithReport.Shift.X, shiftToleranceInMicrons);
            Assert.AreEqual(expectedShiftYInMicrons, resultWithReport.Shift.Y, shiftToleranceInMicrons);
            Assert.AreEqual(expectedAngleInDegrees, resultWithReport.Rotation, rotationToleranceInDegrees);

            Assert.AreEqual(StatusCode.OK, resultWithoutReport.Status.Code);
            Assert.AreEqual(expectedShiftXInMicrons, resultWithoutReport.Shift.X, shiftToleranceInMicrons);
            Assert.AreEqual(expectedShiftYInMicrons, resultWithoutReport.Shift.Y, shiftToleranceInMicrons);
            Assert.AreEqual(expectedAngleInDegrees, resultWithoutReport.Rotation, rotationToleranceInDegrees);

                if (!Directory.EnumerateFileSystemEntries(reportPath).Any())
                    Directory.Delete(reportPath);
            }

        [TestMethod]
        public void Expect_bwa_to_work_on_patterned_border_images()
        {
            string reportPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            string imageFolder = "patterned_border";
            Directory.CreateDirectory(reportPath);

            DirectoryInfo dir = new DirectoryInfo(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder));

            FileInfo[] Files = dir.GetFiles("*.jpg");

            List<string> fileNames = new List<string>();
            //Xcoords & Ycoords coordinates in the stage referential
            List<int> Xcoords = new List<int>();
            List<int> Ycoords = new List<int>();
            List<EdgePosition> edgePositions = new List<EdgePosition>();

            foreach (FileInfo file in Files)
            {
                string fileName = file.Name;

                switch (fileName)
                {
                    case "BOTTOM.jpg":
                        Xcoords.Add(0);
                        Ycoords.Add(-150000);
                        edgePositions.Add(EdgePosition.BOTTOM);
                        break;
                    case "LEFT.jpg":
                        Xcoords.Add(-150000);
                        Ycoords.Add(0);
                        edgePositions.Add(EdgePosition.LEFT);
                        break;
                    case "RIGHT.jpg":
                        Xcoords.Add(150000);
                        Ycoords.Add(0);
                        edgePositions.Add(EdgePosition.RIGHT);
                        break;
                    case "TOP.jpg":
                        Xcoords.Add(0);
                        Ycoords.Add(150000);
                        edgePositions.Add(EdgePosition.TOP);
                        break;
                }
                fileNames.Add(fileName);
            }

            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, NotchWidthInMicrons = 3000, Type = WaferType.NOTCH };
            double pixelSize = 2.125;
            double expectedShiftXInMicrons = 87;
            double expectedShiftYInMicrons = 319;
            double expectedAngleInDegrees = -0.0006;

            var edgeImage1 =
            Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[1]));
            edgeImage1.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage1.Centroid.X = Xcoords[1];
            edgeImage1.Centroid.Y = Ycoords[1];
            edgeImage1.Scale.X = pixelSize;
            edgeImage1.Scale.Y = pixelSize;
            edgeImage1.EdgePosition = edgePositions[1];

            var edgeImage2 =
            Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[2]));
            edgeImage2.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage2.Centroid.X = Xcoords[2];
            edgeImage2.Centroid.Y = Ycoords[2];
            edgeImage2.Scale.X = pixelSize;
            edgeImage2.Scale.Y = pixelSize;
            edgeImage2.EdgePosition = edgePositions[2];

            var edgeImage3 =
            Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[3]));
            edgeImage3.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage3.Centroid.X = Xcoords[3];
            edgeImage3.Centroid.Y = Ycoords[3];
            edgeImage3.Scale.X = pixelSize;
            edgeImage3.Scale.Y = pixelSize;
            edgeImage3.EdgePosition = edgePositions[3];



            var notchImage =
            Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("BWA/" + imageFolder + "/" + fileNames[0]));
            notchImage.ExpectedShape = WaferEdgeShape.NOTCH;
            notchImage.Centroid.X = Xcoords[0];
            notchImage.Centroid.Y = Ycoords[0];
            notchImage.Scale.X = pixelSize;
            notchImage.Scale.Y = pixelSize;
            notchImage.EdgePosition = edgePositions[0];

            var images = new List<BareWaferAlignmentImageData> { edgeImage1, edgeImage2, edgeImage3, notchImage };

            // when

            int notchDetectionVersion = 3;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;
            var resultWithReport = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, edgeDetectionVersion, notchDetectionVersion, cannyThreshold, reportPath, ReportOption.OverlayReport);

            // Then

            double rotationToleranceInDegrees = 0.001;
            double shiftToleranceInMicrons = 40.0;

            Assert.AreEqual(StatusCode.OK, resultWithReport.Status.Code);
            Assert.AreEqual(expectedShiftXInMicrons, resultWithReport.Shift.X, shiftToleranceInMicrons);
            Assert.AreEqual(expectedShiftYInMicrons, resultWithReport.Shift.Y, shiftToleranceInMicrons);
            Assert.AreEqual(expectedAngleInDegrees, resultWithReport.Rotation, rotationToleranceInDegrees);

            if (!Directory.EnumerateFileSystemEntries(reportPath).Any())
                Directory.Delete(reportPath);
        }

        [TestMethod]
        public void Expect_contour_detection_to_fail_on_notch_image_ExpectedShape()
        {
            // given
            var notchImage = new BareWaferAlignmentImageData(0, 0);
            notchImage.ExpectedShape = WaferEdgeShape.NOTCH;

            int waferDiameterInMm = 300;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;

            // when
            var resultWithReport = BareWaferAlignment.PerformEdgeImageContourExtraction(notchImage, waferDiameterInMm, edgeDetectionVersion, cannyThreshold);

            // then
            Assert.AreEqual(StatusCode.CANNOT_DETECT_EDGE_ON_NOTCH_IMAGE, resultWithReport.Status.Code,
                            "NOTCH image ExpectedShape contour extraction is useless and must be disallowed in implementation");
        }

        [TestMethod]
        public void Expect_contour_detection_old_version_to_work_on_single_edge_image()
        {
            string reportPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(reportPath);

            // given
            var edgeImage =
                Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"));
            edgeImage.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage.Centroid.X = -26047;
            edgeImage.Centroid.Y = -147721;
            edgeImage.Scale.X = 5.3;
            edgeImage.Scale.Y = 5.3;

            int waferDiameterInMm = 300;
            int edgeDetectionVersion = 1;
            int cannyThreshold = 200;

            // when
            var resultWithReport = BareWaferAlignment.PerformEdgeImageContourExtraction(edgeImage, waferDiameterInMm, edgeDetectionVersion, cannyThreshold, reportPath, ReportOption.OverlayReport);
            var resultWithoutReport = BareWaferAlignment.PerformEdgeImageContourExtraction(edgeImage, waferDiameterInMm, edgeDetectionVersion, cannyThreshold);

            // then
            const int EXPECTED_CONTOUR_POINTS = 870;
            Assert.AreEqual(StatusCode.OK, resultWithReport.Status.Code);
            Assert.AreEqual(EXPECTED_CONTOUR_POINTS, resultWithReport.Contour.Count);

            Assert.AreEqual(StatusCode.OK, resultWithoutReport.Status.Code);
            Assert.AreEqual(EXPECTED_CONTOUR_POINTS, resultWithoutReport.Contour.Count);

            if (!Directory.EnumerateFileSystemEntries(reportPath).Any())
                Directory.Delete(reportPath);
        }

        [TestMethod]
        public void Expect_contour_detection_to_work_on_single_edge_image()
        {
            string reportPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(reportPath);

            // given
            var edgeImage =
                Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"));
            edgeImage.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage.Centroid.X = -26047;
            edgeImage.Centroid.Y = -147721;
            edgeImage.Scale.X = 5.3;
            edgeImage.Scale.Y = 5.3;

            int waferDiameterInMm = 300;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;

            // when
            var resultWithReport = BareWaferAlignment.PerformEdgeImageContourExtraction(edgeImage, waferDiameterInMm, edgeDetectionVersion, cannyThreshold, reportPath, ReportOption.OverlayReport);
            var resultWithoutReport = BareWaferAlignment.PerformEdgeImageContourExtraction(edgeImage, waferDiameterInMm, edgeDetectionVersion, cannyThreshold);

            // then
            const int EXPECTED_CONTOUR_POINTS = 1125;
            Assert.AreEqual(StatusCode.OK, resultWithReport.Status.Code);
            Assert.AreEqual(EXPECTED_CONTOUR_POINTS, resultWithReport.Contour.Count);

            Assert.AreEqual(StatusCode.OK, resultWithoutReport.Status.Code);
            Assert.AreEqual(EXPECTED_CONTOUR_POINTS, resultWithoutReport.Contour.Count);

            if (!Directory.EnumerateFileSystemEntries(reportPath).Any())
                Directory.Delete(reportPath);
        }

        [TestMethod]
        public void Expect_contour_to_fail_on_single_empty_image()
        {
            string reportPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(reportPath);

            // given
            var edgeImage =
                Helpers.CreateBWAImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("noContour.png"));
            edgeImage.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage.Centroid.X = 150000;
            edgeImage.Centroid.Y = 0;
            edgeImage.Scale.X = 2.125;
            edgeImage.Scale.Y = 2.125;

            var waferDiameterInMm = 300;
            int edgeDetectionVersion = 2;
            int cannyThreshold = 200;

            // when
            var resultWithReport = BareWaferAlignment.PerformEdgeImageContourExtraction(edgeImage, waferDiameterInMm, edgeDetectionVersion, cannyThreshold, reportPath, ReportOption.OverlayReport);

            // then
            const int EXPECTED_CONTOUR_POINTS = 0;
            Assert.AreEqual(StatusCode.UNKNOWN_ERROR, resultWithReport.Status.Code);
            Assert.AreEqual(EXPECTED_CONTOUR_POINTS, resultWithReport.Contour.Count);

            if (!Directory.EnumerateFileSystemEntries(reportPath).Any())
                Directory.Delete(reportPath);
        }

    }
}
