using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Service.Core.Recipe.AcquisitionPath;
using UnitySC.PM.EME.Service.Core.Test.Flow;
using UnitySC.PM.EME.Service.Shared.TestUtils;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class AcquisitionPathTest : TestWithMockedHardware<AutoFocusCameraTest>
    {
        [TestMethod]
        public void Serpentine_NextPosition_ShouldReturnCorrectPositions()
        {
            // Arrange
            const int nbImagesX = 6;
            const int nbImagesY = 8;

            var path = new SerpentinePath(300, 60, 80, nbImagesX, nbImagesY);
            var expectedPositions = new List<(double, double)>
            {
                (-121, 111), (-71, 111), (-21, 111), (29, 111), (79, 111), (129, 111),
                (129, 73.5), (79, 73.5), (29, 73.5), (-21, 73.5), (-71, 73.5), (-121, 73.5),
                (-121, 36), (-71, 36), (-21, 36), (29, 36), (79, 36), (129, 36),
                (129, -1.5), (79, -1.5), (29, -1.5), (-21, -1.5), (-71, -1.5), (-121, -1.5),
                (-121, -39), (-71, -39), (-21, -39), (29, -39), (79, -39), (129, -39),
                (129, -76.5), (79, -76.5), (29, -76.5), (-21, -76.5), (-71, -76.5), (-121, -76.5),
                (-121, -114), (-71, -114), (-21, -114), (29, -114), (79, -114), (129, -114),
                (129, -151.5), (79, -151.5), (29, -151.5), (-21, -151.5), (-71, -151.5), (-121, -151.5),
            };

            // Act
            var actualPositions = Enumerable.Range(0, nbImagesX * nbImagesY)
                .Select(i => path.NextPosition().Position)
                .ToList();

            // Assert
            CollectionAssert.AreEqual(expectedPositions, actualPositions);
        }

        [TestMethod]
        public void Serpentine_IsLastPosition_ShouldReturnTrueForLastPosition()
        {
            // Arrange
            const int nbImagesX = 6;
            const int nbImagesY = 8;

            var path = new SerpentinePath(300, 60, 80, nbImagesX, nbImagesY);
            // Move to the last position
            for (int i = 0; i < nbImagesX * nbImagesY; i++)
            {
                path.NextPosition();
            }

            // Act
            bool isLast = path.IsLastPosition();

            // Assert
            Assert.IsTrue(isLast, "IsLastPosition should return true when the last position is reached.");
        }

        [TestMethod]
        public void TypeWriter_NextPosition_ShouldReturnCorrectPositions()
        {
            // Arrange
            const int nbImagesX = 6;
            const int nbImagesY = 8;

            var path = new TypewriterPath(300, 60, 80, nbImagesX, nbImagesY);
            var expectedPositions = new List<(double, double)>
            {
                (-121, 111), (-71, 111), (-21, 111), (29, 111), (79, 111), (129, 111),
                (-121, 73.5), (-71, 73.5), (-21, 73.5), (29, 73.5), (79, 73.5), (129, 73.5),
                (-121, 36), (-71, 36), (-21, 36), (29, 36), (79, 36), (129, 36),
                (-121, -1.5), (-71, -1.5), (-21, -1.5), (29, -1.5), (79, -1.5), (129, -1.5),
                (-121, -39), (-71, -39), (-21, -39), (29, -39), (79, -39), (129, -39),
                (-121, -76.5), (-71, -76.5), (-21, -76.5), (29, -76.5), (79, -76.5), (129, -76.5),
                (-121, -114), (-71, -114), (-21, -114), (29, -114), (79, -114), (129, -114),
                (-121, -151.5), (-71, -151.5), (-21, -151.5), (29, -151.5), (79, -151.5), (129, -151.5),
            };

            // Act 
            var actualPositions = Enumerable.Range(0, nbImagesX * nbImagesY)
                .Select(i => path.NextPosition().Position)
                .ToList();

            // Assert
            CollectionAssert.AreEqual(expectedPositions, actualPositions);
        }

        [TestMethod]
        public void TypeWriter_IsLastPosition_ShouldReturnTrueForLastPosition()
        {
            // Arrange
            const int nbImagesX = 6;
            const int nbImagesY = 8;

            var path = new TypewriterPath(300, 60, 80, nbImagesX, nbImagesY);

            // Act
            for (int i = 0; i < nbImagesX * nbImagesY; i++)
            {
                path.NextPosition();
            }

            bool isLast = path.IsLastPosition();

            // Assert
            Assert.IsTrue(isLast, "IsLastPosition should return true when the last position is reached.");
        }
    }
}
