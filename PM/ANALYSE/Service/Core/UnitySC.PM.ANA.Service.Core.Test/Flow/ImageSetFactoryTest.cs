using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class ImageSetFactoryTest
    {
        [TestInitialize]
        public void Init()
        {
        }

        /// <summary>
        /// Counts coordinates match between two collections
        /// </summary>
        /// <param name="expectedCoordinatePairs">list of int pairs x,y where x is at index 0 and y at index 1</param>
        /// <param name="imageDatas">ImageData with centroid coordinates to check</param>
        /// <returns>How many ImageData have centroid coordinates matching expected ones</returns>
        public int CountMatchingCoordinatePairs(List<List<double>> expectedCoordinatePairs, List<BareWaferAlignmentImageData> imageDatas)
        {
            int matchCounter = 0;
            foreach (var imageData in imageDatas)
            {
                foreach (var coordinatePair in expectedCoordinatePairs)
                {
                    var imageCentroid = imageData.Centroid;
                    bool xDoMatch = (imageCentroid.X.Equals(coordinatePair[0]));
                    bool yDoMatch = (imageCentroid.Y.Equals(coordinatePair[1]));
                    if (xDoMatch && yDoMatch)
                    {
                        matchCounter++;
                    }
                }
            }
            return matchCounter;
        }

        // WRAPPER TESTS -----------------------------------------------------------------

        [TestMethod]
        public void Should_produce_right_positions_for_300mm_notch_wafer()
        {
            // given
            var waferCharacteristics = new WaferDimensionalCharacteristic();
            waferCharacteristics.Diameter = 300.Millimeters();
            waferCharacteristics.WaferShape = WaferShape.Notch;

            var expectedEdgeCoordinates = new List<List<double>>()
       {
           new List<double>(){ -150000, 0},
           new List<double>(){ 150000, 0  },
           new List<double>(){ 0, 150000 },
       };

            var expectedNotchCoordinates = new List<List<double>>()
       {
           new List<double>(){0,-150000 }
       };

            // when
            var imagePositions = ImageSetCentroidFactory.GetImageDataListFor(waferCharacteristics);

            // then
            var edgeImages = imagePositions.FindAll(delegate (BareWaferAlignmentImageData imageData) { return imageData.ExpectedShape == WaferEdgeShape.EDGE; });
            Assert.AreEqual(3, edgeImages.Count, "We should have 3 edges images for such wafer");

            int imageWithGoodCentroidCounter = CountMatchingCoordinatePairs(expectedEdgeCoordinates, edgeImages);
            Assert.AreEqual(3, imageWithGoodCentroidCounter, "Edges images centroid should be the expected ones");

            var notchImage = imagePositions.FindAll(delegate (BareWaferAlignmentImageData imageData) { return imageData.ExpectedShape == WaferEdgeShape.NOTCH; });
            Assert.AreEqual(1, notchImage.Count, "We should have 1 notch image for such wafer");
            imageWithGoodCentroidCounter = CountMatchingCoordinatePairs(expectedNotchCoordinates, notchImage);

            Assert.AreEqual(1, imageWithGoodCentroidCounter, "Wafer notch image centroid should have expected coordinates");
        }

        [TestMethod]
        public void Should_throw_UnsupportedWaferException_when_it_cannot_produce_result()
        {
            var waferCharacteristics = new WaferDimensionalCharacteristic();
            waferCharacteristics.Diameter = 300.Millimeters();

            try
            {
                ImageSetCentroidFactory.GetImageDataListFor(waferCharacteristics);
            }
            catch (UnsupportedWaferException)
            {
                // ok!
            }
        }
    };
}
