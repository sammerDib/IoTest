using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgosLibrary;
using System.Collections.Generic;
using System.IO;
using System;

namespace UnitySC.Algorithms.Wrapper.Tests
{
    [TestClass]
    public class BareWaferAlignmentTest
    {
        [TestInitialize]
        public void Init() { }

        // WRAPPER TESTS -----------------------------------------------------------------

        [TestMethod]
        public void Expect_bwa_to_fail_without_edge_image()
        {
            string currentPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(currentPath);

            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, Type = WaferType.NOTCH };
            var images = new List<BareWaferAlignmentImageData>();

            // when
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, currentPath);

            // then
            Assert.AreEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should fail without edge image");
        }

        [TestMethod]
        public void Expect_bwa_to_fail_with_1_edge_image()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, Type = WaferType.NOTCH };
            var images = new List<BareWaferAlignmentImageData>();
            images.Add(new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE });

            // when
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData);

            // then
            Assert.AreEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should fail with one edge image");
        }

        [TestMethod]
        public void Expect_bwa_to_fail_with_2_edge_image()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, Type = WaferType.NOTCH };
            var images = new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                                 new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE } };

            // when
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData);

            // then
            Assert.AreEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should fail with two edge image");
        }

        [TestMethod]
        public void Expect_bwa_to_continue_processing_with_3_edges()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, Type = WaferType.NOTCH };

            var images =
                new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE } };
            // when
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData);

            // then
            Assert.AreNotEqual(StatusCode.MISSING_EDGE_IMAGE, result.Status.Code, "BWA should not fail with missing edge image if 3 edge images are given");
        }

        [TestMethod]
        public void Expect_bwa_for_notch_wafer_to_fail_without_notch_image()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, Type = WaferType.NOTCH };
            var images =
                new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE } };

            // when
            var result = BareWaferAlignment.PerformBareWaferAlignment(images, waferData);

            // then
            Assert.AreEqual(StatusCode.MISSING_NOTCH_IMAGE, result.Status.Code);
        }

        [TestMethod]
        public void Expect_bwa_to_fail_if_empty_images_are_provided()
        {
            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, Type = WaferType.NOTCH };
            var images =
                new List<BareWaferAlignmentImageData> { new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.EDGE },
                                                        new BareWaferAlignmentImageData { ExpectedShape = WaferEdgeShape.NOTCH } };

            // when
            try
            {
                BareWaferAlignment.PerformBareWaferAlignment(images, waferData);
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
            Directory.CreateDirectory(reportPath);

            // given
            var waferData = new WaferInfos { RadiusInMicrons = 150000, Type = WaferType.NOTCH };
            double pixelSize = 5.3;
            double expectedWaferShiftX = 111;
            double expectedWaferShiftY = 246;
            double expectedShiftXInMicrons = expectedWaferShiftX * pixelSize;
            double expectedShiftYInMicrons = expectedWaferShiftY * pixelSize;
            double expectedAngleInDegrees = -0.78;
            double expectedAngleInRadians = expectedAngleInDegrees * Math.PI / 180;

            var edgeImage1 =
                Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("metrological_wafer\\EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"));
            edgeImage1.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage1.Centroid.X = -26047;
            edgeImage1.Centroid.Y = -147721;
            edgeImage1.Scale.X = pixelSize;
            edgeImage1.Scale.Y = pixelSize;

            var edgeImage2 =
                Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("metrological_wafer\\EdgeDetection_2_2X_VIS_X_23465_Y_148153.png"));
            edgeImage2.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage2.Centroid.X = 23465;
            edgeImage2.Centroid.Y = 148153;
            edgeImage2.Scale.X = pixelSize;
            edgeImage2.Scale.Y = pixelSize;

            var edgeImage3 =
                Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("metrological_wafer\\EdgeDetection_3_2X_VIS_X_139077_Y_56190.png"));
            edgeImage3.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage3.Centroid.X = 139077;
            edgeImage3.Centroid.Y = 56190;
            edgeImage3.Scale.X = pixelSize;
            edgeImage3.Scale.Y = pixelSize;

            var notchImage =
                Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("metrological_wafer\\EdgeDetection_0_2X_VIS_X_0_Y_-150000.png"));
            notchImage.ExpectedShape = WaferEdgeShape.NOTCH;
            notchImage.Centroid.X = 0;
            notchImage.Centroid.Y = -150000;
            notchImage.Scale.X = pixelSize;
            notchImage.Scale.Y = pixelSize;

            var images = new List<BareWaferAlignmentImageData> { edgeImage1, edgeImage2, edgeImage3, notchImage };

            // when

            var resultWithReport = BareWaferAlignment.PerformBareWaferAlignment(images, waferData, reportPath);

            var resultWithoutReport = BareWaferAlignment.PerformBareWaferAlignment(images, waferData);

            // Then

            double rotationToleranceInRadians = 0.001;
            double shiftToleranceInMicrons = 10 * pixelSize;

            Assert.AreEqual(StatusCode.OK, resultWithReport.Status.Code);
            Assert.AreEqual(expectedShiftXInMicrons, resultWithReport.Shift.X, shiftToleranceInMicrons);
            Assert.AreEqual(expectedShiftYInMicrons, resultWithReport.Shift.Y, shiftToleranceInMicrons);
            Assert.AreEqual(expectedAngleInRadians, resultWithReport.Rotation, rotationToleranceInRadians);

            Assert.AreEqual(StatusCode.OK, resultWithoutReport.Status.Code);
            Assert.AreEqual(expectedShiftXInMicrons, resultWithoutReport.Shift.X, shiftToleranceInMicrons);
            Assert.AreEqual(expectedShiftYInMicrons, resultWithoutReport.Shift.Y, shiftToleranceInMicrons);
            Assert.AreEqual(expectedAngleInRadians, resultWithoutReport.Rotation, rotationToleranceInRadians);
        }

        [TestMethod]
        public void Expect_contour_detection_to_fail_on_notch_image_ExpectedShape()
        {
            // given
            var notchImage = new BareWaferAlignmentImageData(0, 0);
            notchImage.ExpectedShape = WaferEdgeShape.NOTCH;

            // when
            var resultWithReport = BareWaferAlignment.PerformEdgeImageContourExtraction(notchImage);

            // then
            Assert.AreEqual(StatusCode.CANNOT_DETECT_EDGE_ON_NOTCH_IMAGE, resultWithReport.Status.Code,
                            "NOTCH image ExpectedShape contour extraction is useless and must be disallowed in implementation");
        }

        [TestMethod]
        public void Expect_contour_detection_to_work_on_single_edge_image()
        {
            string reportPath = Directory.GetCurrentDirectory() + "\\reports\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(reportPath);

            // given
            var edgeImage =
                Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("metrological_wafer\\EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png"));
            edgeImage.ExpectedShape = WaferEdgeShape.EDGE;
            edgeImage.Centroid.X = -26047;
            edgeImage.Centroid.Y = -147721;
            edgeImage.Scale.X = 5.3;
            edgeImage.Scale.Y = 5.3;

            // when
            var resultWithReport = BareWaferAlignment.PerformEdgeImageContourExtraction(edgeImage, reportPath);
            var resultWithoutReport = BareWaferAlignment.PerformEdgeImageContourExtraction(edgeImage);

            // then
            const int EXPECTED_CONTOUR_POINTS = 1123;
            Assert.AreEqual(StatusCode.OK, resultWithReport.Status.Code);
            Assert.AreEqual(EXPECTED_CONTOUR_POINTS, resultWithReport.Contour.Count);

            Assert.AreEqual(StatusCode.OK, resultWithoutReport.Status.Code);
            Assert.AreEqual(EXPECTED_CONTOUR_POINTS, resultWithoutReport.Contour.Count);
        }
    }
}
