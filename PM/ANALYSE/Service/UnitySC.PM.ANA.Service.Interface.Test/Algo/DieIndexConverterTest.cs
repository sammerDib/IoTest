using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Interface.Test.Algo
{
    [TestClass]
    public class DieIndexConverterTest
    {
        [TestMethod]
        public void RowToDieReference()
        {
            // Given
            int row = 10;
            int rowDieReference = 13;

            //When
            int rowInDieReference = DieIndexConverter.ConvertRowToDieReference(row, rowDieReference);

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
            int row = DieIndexConverter.ConvertRowFromDieReference(rowInDieReference, rowDieReference);

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
            int rowInDieReference = DieIndexConverter.ConvertRowToDieReference(initialRow, rowDieReference);
            int row = DieIndexConverter.ConvertRowFromDieReference(rowInDieReference, rowDieReference);

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
            int columnInDieReference = DieIndexConverter.ConvertColumnToDieReference(column, columnDieReference);

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
            int column = DieIndexConverter.ConvertColumnFromDieReference(columnInDieReference, columnDieReference);

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
            int columnInDieReference = DieIndexConverter.ConvertRowToDieReference(initialColumn, columnDieReference);
            int column = DieIndexConverter.ConvertRowFromDieReference(columnInDieReference, columnDieReference);

            // Then we land back on the initial value
            Assert.AreEqual(initialColumn, column);
        }

        [TestMethod]
        public void RowToDieReference_WithDieReference()
        {
            // Given
            int row = 10;
            var dieReference = new DieIndex(15, 13);

            //When
            int rowInDieReference = DieIndexConverter.ConvertRowToDieReference(row, dieReference);

            // Then
            Assert.AreEqual(3, rowInDieReference);
        }

        [TestMethod]
        public void RowFromDieReference_WithDieReference()
        {
            // Given
            int rowInDieReference = 7;
            var dieReference = new DieIndex(15, 13);

            //When
            int row = DieIndexConverter.ConvertRowFromDieReference(rowInDieReference, dieReference);

            // Then
            Assert.AreEqual(6, row);
        }

        [TestMethod]
        public void RowConversionSymmetry_WithDieReference()
        {
            // Given
            int initialRow = 8;
            var dieReference = new DieIndex(15, 13);

            // When applying both conversions
            int rowInDieReference = DieIndexConverter.ConvertRowToDieReference(initialRow, dieReference);
            int row = DieIndexConverter.ConvertRowFromDieReference(rowInDieReference, dieReference);

            // Then we land back on the initial value
            Assert.AreEqual(initialRow, row);
        }

        [TestMethod]
        public void ColumnToDieReference_WithDieReference()
        {
            // Given
            int column = 10;
            var dieReference = new DieIndex(15, 13);

            //When
            int columnInDieReference = DieIndexConverter.ConvertColumnToDieReference(column, dieReference);

            // Then
            Assert.AreEqual(-5, columnInDieReference);
        }

        [TestMethod]
        public void ColumnFromDieReference_WithDieReference()
        {
            // Given
            int columnInDieReference = 7;
            var dieReference = new DieIndex(15, 13);

            //When
            int column = DieIndexConverter.ConvertColumnFromDieReference(columnInDieReference, dieReference);

            // Then
            Assert.AreEqual(22, column);
        }

        [TestMethod]
        public void ColumnConversionSymmetry_WithDieReference()
        {
            // Given
            int initialColumn = 8;
            var dieReference = new DieIndex(15, 13);

            // When applying both conversions
            int columnInDieReference = DieIndexConverter.ConvertRowToDieReference(initialColumn, dieReference);
            int column = DieIndexConverter.ConvertRowFromDieReference(columnInDieReference, dieReference);

            // Then we land back on the initial value
            Assert.AreEqual(initialColumn, column);
        }

        [TestMethod]
        public void DieIndexToDieReference()
        {
            // Given
            var die = new DieIndex(10, 10);
            var dieReference = new DieIndex(15, 13);

            //When
            DieIndex dieInDiereference = die.ToDieReference(dieReference);

            // Then
            Assert.AreEqual(new DieIndex(-5, 3), dieInDiereference);
        }

        [TestMethod]
        public void DieIndexFromDieReference()
        {
            // Given
            var dieInDieReference = new DieIndex(7, 7);
            var dieReference = new DieIndex(15, 13);

            //When
            DieIndex die = dieInDieReference.FromDieReference(dieReference);

            // Then
            Assert.AreEqual(new DieIndex(22, 6), die);
        }

        [TestMethod]
        public void DieIndexConversionSymmetry()
        {
            // Given
            var initialDie = new DieIndex(8, 13);
            var dieReference = new DieIndex(15, 13);

            // When applying both conversions
            DieIndex dieInDieReference = initialDie.ToDieReference(dieReference);
            DieIndex die = dieInDieReference.FromDieReference(dieReference);

            // Then we land back on the initial value
            Assert.AreEqual(initialDie, die);
        }
    }
}
