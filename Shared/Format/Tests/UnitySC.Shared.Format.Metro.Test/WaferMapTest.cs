using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    [TestClass]
    public class WaferMapTest
    {
        private const double Precision = 1e-6;

        [TestMethod]
        public void RowToDieReference()
        {
            // Given
            int row = 10;
            int rowDieReference = 13;

            //When
            int rowInDieReference = WaferMap.ConvertRowToDieReference(row, rowDieReference);

            // Then
            Assert.AreEqual(3, rowInDieReference);
        }

        [TestMethod]
        public void RowFromDieReference()
        {
            // Given
            int rowInDieReference = 7;
            int rowDieReference = 13;

            //When
            int row = WaferMap.ConvertRowFromDieReference(rowInDieReference, rowDieReference);

            // Then
            Assert.AreEqual(6, row);
        }

        [TestMethod]
        public void RowConversionSymmetry()
        {
            // Given
            int initialRow = 8;
            int rowDieReference = 13;

            // When applying both conversions
            int rowInDieReference = WaferMap.ConvertRowToDieReference(initialRow, rowDieReference);
            int row = WaferMap.ConvertRowFromDieReference(rowInDieReference, rowDieReference);

            // Then we land back on the initial value
            Assert.AreEqual(initialRow, row);
        }

        [TestMethod]
        public void ColumnToDieReference()
        {
            // Given
            int column = 10;
            int columnDieReference = 15;

            //When
            int columnInDieReference = WaferMap.ConvertColumnToDieReference(column, columnDieReference);

            // Then
            Assert.AreEqual(-5, columnInDieReference);
        }

        [TestMethod]
        public void ColumnFromDieReference()
        {
            // Given
            int columnInDieReference = 7;
            int columnDieReference = 15;

            //When
            int column = WaferMap.ConvertColumnFromDieReference(columnInDieReference, columnDieReference);

            // Then
            Assert.AreEqual(22, column);
        }

        [TestMethod]
        public void ColumnConversionSymmetry()
        {
            // Given
            int initialColumn = 8;
            int columnDieReference = 15;

            // When applying both conversions
            int columnInDieReference = WaferMap.ConvertRowToDieReference(initialColumn, columnDieReference);
            int column = WaferMap.ConvertRowFromDieReference(columnInDieReference, columnDieReference);

            // Then we land back on the initial value
            Assert.AreEqual(initialColumn, column);
        }

        [TestMethod]
        public void WaferRelativePosition()
        {
            // Given wafer map
            WaferMap waferMap = new WaferMap()
            {
                DieGridTopLeftXPosition = -5.2.Millimeters(),
                DieGridTopLeftYPosition = 155.2.Millimeters(),
                DiePitchHeight = 12.3.Millimeters(),
                DiePitchWidth = 11.4.Millimeters(),
                DieReferenceColumnIndex = 3,
                DieReferenceRowIndex = 20,
            };

            // When computing the wafer relative x and y positions
            double resX = waferMap.WaferRelativeXPositionLeftOfColumn(5);
            double resY = waferMap.WaferRelativeYPositionTopOfRow(9);

            // Then it is properly computed
            Assert.AreEqual(86.0, resX, Precision);
            Assert.AreEqual(19.9, resY, Precision);
        }
    }
}
