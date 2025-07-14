using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Test
{
    [TestClass]
    public class AnaPositionTest
    {
        [TestMethod]
        public void Expect_ToString_to_be_correct()
        {
            // Given
            var anaPosition = new AnaPosition(new MotorReferential())
            {
                X = 1,
                Y = 2,
                ZTop = 3,
                ZBottom = 4,
                ZPiezoPositions = new List<ZPiezoPosition>()
                {
                    new ZPiezoPosition(new MotorReferential(), "piezoAxis-1", 5.Micrometers()),
                    new ZPiezoPosition(new MotorReferential(), "piezoAxis-2", 6.Micrometers())
                }
            };

            string expectedStringPosition = new StringBuilder()
                .AppendLine("\tReferential = Motor")
                .AppendLine("\tX = 1")
                .AppendLine("\tY = 2")
                .AppendLine("\tZTop = 3")
                .AppendLine("\tZBottom = 4")
                .AppendLine("\tZPiezoPositions =")
                .AppendLine("\t\tFor piezo axis piezoAxis-1, Position = 5")
                .AppendLine("\t\tFor piezo axis piezoAxis-2, Position = 6")
                .ToString();

            // When
            string anaPositionAsString = anaPosition.ToString();

            // Then
            Assert.AreEqual(expectedStringPosition, anaPositionAsString);
        }

        [TestMethod]
        public void Expect_GetPiezoPosition_to_return_position_when_exist()
        {
            // Given
            var expectedPiezoPosition = new ZPiezoPosition(new MotorReferential(), "piezoAxisID", 1.Micrometers());
            var anaPosition = new AnaPosition(new MotorReferential())
            {
                ZPiezoPositions = new List<ZPiezoPosition>() { expectedPiezoPosition }
            };

            // When
            var piezoPosition = anaPosition.GetPiezoPosition("piezoAxisID");

            // Then
            Assert.AreEqual(expectedPiezoPosition, piezoPosition);
        }

        [TestMethod]
        public void Expect_GetPiezoPosition_to_return_null_when_position_do_not_exist()
        {
            // Given
            var anaPosition = new AnaPosition(new MotorReferential());

            // When
            var piezoPosition = anaPosition.GetPiezoPosition("piezoAxisID");

            // Then
            Assert.IsNull(piezoPosition);
        }

        [TestMethod]
        public void Expect_AddOrUpdateZPiezoPosition_to_add_when_position_do_not_exist()
        {
            // Given
            var piezoPosition = new ZPiezoPosition(new MotorReferential(), "piezoAxisID", 1.Micrometers());
            var anaPosition = new AnaPosition(new MotorReferential());
            Assert.AreEqual(0, anaPosition.ZPiezoPositions.Count);

            // When
            anaPosition.AddOrUpdateZPiezoPosition(piezoPosition);

            // Then
            Assert.AreEqual(1, anaPosition.ZPiezoPositions.Count);
            Assert.AreEqual(piezoPosition, anaPosition.ZPiezoPositions.First());
        }

        [TestMethod]
        public void Expect_AddOrUpdateZPiezoPosition_to_update_when_position_already_exist()
        {
            // Given
            var piezoPosition = new ZPiezoPosition(new MotorReferential(), "piezoAxisID", 1.Micrometers());
            var anaPosition = new AnaPosition(new MotorReferential())
            {
                ZPiezoPositions = new List<ZPiezoPosition>() { piezoPosition },
            };
            Assert.AreEqual(1, anaPosition.ZPiezoPositions.Count);
            var newPiezoPosition = new ZPiezoPosition(new MotorReferential(), "piezoAxisID", 2.Micrometers());

            // When
            anaPosition.AddOrUpdateZPiezoPosition(newPiezoPosition);

            // Then
            Assert.AreEqual(1, anaPosition.ZPiezoPositions.Count);
            Assert.AreEqual(newPiezoPosition, anaPosition.ZPiezoPositions.First());
        }

        [TestMethod]
        public void Expect_Equals_to_return_true_with_empty_AnaPositions()
        {
            var anaPosition_A = new AnaPosition(new MotorReferential());
            var anaPosition_B = new AnaPosition(new MotorReferential());

            Assert.AreEqual(anaPosition_A, anaPosition_B);
        }

        [TestMethod]
        public void Expect_GetHashCode_of_similar_positions_to_be_the_same()
        {
            // Given
            var anaPosition_A = new AnaPosition(new MotorReferential())
            {
                X = 1,
                Y = 2,
                ZTop = 3,
                ZBottom = 4,
                ZPiezoPositions = new List<ZPiezoPosition>()
                {
                    new ZPiezoPosition(new MotorReferential(), "piezoAxis-1", 5.Micrometers()),
                    new ZPiezoPosition(new MotorReferential(), "piezoAxis-2", 6.Micrometers())
                }
            };

            var anaPosition_B = new AnaPosition(new MotorReferential())
            {
                X = 1,
                Y = 2,
                ZTop = 3,
                ZBottom = 4,
                ZPiezoPositions = new List<ZPiezoPosition>()
                {
                    new ZPiezoPosition(new MotorReferential(), "piezoAxis-1", 5.Micrometers()),
                    new ZPiezoPosition(new MotorReferential(), "piezoAxis-2", 6.Micrometers())
                }
            };

            Assert.AreEqual(anaPosition_A.GetHashCode(), anaPosition_B.GetHashCode());
        }

        [DataTestMethod]
        [DataRow(1, 1, true)]
        [DataRow(1, 2, false)]
        [DataRow(1, 0.999999999, false)]
        public void Expect_Equals_to_be_consistent_with_expected_results(double positionA, double positionB, bool expectedResult)
        {
            // Given
            var piezoPosition_A = new ZPiezoPosition(new MotorReferential(), "piezoAxisID", positionA.Micrometers());
            var piezoPosition_B = new ZPiezoPosition(new MotorReferential(), "piezoAxisID", positionB.Micrometers());

            var anaPosition_A = new AnaPosition(new MotorReferential())
            {
                ZPiezoPositions = new List<ZPiezoPosition>() { piezoPosition_A },
            };

            var anaPosition_B = new AnaPosition(new MotorReferential())
            {
                ZPiezoPositions = new List<ZPiezoPosition>() { piezoPosition_B },
            };

            // When
            bool positionsAreEqual = anaPosition_A.Equals(anaPosition_B);

            // Then
            Assert.AreEqual(expectedResult, positionsAreEqual);
        }
    }
}
