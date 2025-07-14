using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools.Test
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void Matrix_creation()
        {
            int rowsNb = 3;
            int columnsNb = 10;
            var matrix = new Matrix<string>(rowsNb, columnsNb);

            Assert.AreEqual(rowsNb, matrix.Rows);
            Assert.AreEqual(columnsNb, matrix.Columns);
            for (int r = 0; r < rowsNb; r++)
            {
                for (int c = 0; c < columnsNb; c++)
                {
                    Assert.AreEqual(null, matrix.GetValue(r, c));
                }
            }
        }

        [TestMethod]
        public void Matrix_creation_from_2darray()
        {
            bool[,] array = { { false, false, false }, { false, true, false }, { false, false, false } };
            var matrix = new Matrix<bool>(array);

            Assert.AreEqual(array.GetLength(0), matrix.Rows);
            Assert.AreEqual(array.GetLength(1), matrix.Columns);
            for (int r = 0; r < array.GetLength(0); r++)
            {
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    Assert.AreEqual(array[r, c], matrix.GetValue(r, c));
                }
            }
        }

        [TestMethod]
        public void Matrix_equality()
        {
            bool[,] array1 = { { false, false, false }, { false, true, false }, { false, false, false } };
            bool[,] array2 = { { false, false, false }, { false, false, false }, { false, false, false } };

            var matrix1 = new Matrix<bool>(array1);
            var matrix2 = new Matrix<bool>(array2);

            Assert.IsTrue(matrix1.Equals(matrix1));
            Assert.IsFalse(matrix2.Equals(matrix1));
        }

        [TestMethod]
        public void Matrix_serialization()
        {
            bool[,] array = { { false, false, false }, { false, true, false }, { false, false, false } };
            Matrix<bool> matrix = new Matrix<bool>(array);

            string storageFile = "MatrixSerializationTest.txt";
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(storageFile, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, matrix);
            stream.Close();

            stream = new FileStream(storageFile, FileMode.Open, FileAccess.Read);
            Matrix<bool> matrixNew = (Matrix<bool>)formatter.Deserialize(stream);

            stream.Close();
            File.Delete(storageFile);

            Assert.IsTrue(matrix.Equals(matrixNew));
        }
    }
}
