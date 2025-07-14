using System;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Tools.Units
{
    [DataContract]
    [Serializable]
    public class Matrix<T> : IEquatable<Matrix<T>>
    {
        [DataMember]
        public int Rows { get; set; }

        [DataMember]
        public int Columns { get; set; }

        // For the serialisation
        private Matrix()
        {
        }

        public Matrix(int rowNb, int columnNb)
        {
            Values = new T[rowNb][];

            for (int i = 0; i < rowNb; i++)
            {
                Values[i] = new T[columnNb];
            }

            Rows = rowNb;
            Columns = columnNb;
        }

        public Matrix(T[,] array2D) : this(array2D.GetLength(0), array2D.GetLength(1))
        {
            for (int Row = 0; Row < Rows; Row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    SetValue(Row, column, array2D[Row, column]);
                }
            }
        }

        public void SetValue(int Row, int column, T value)
        {
            Values[Row][column] = value;
        }

        public T GetValue(int Row, int column)
        {
            return Values[Row][column];
        }

        public bool Equals(Matrix<T> other)
        {
            if (Rows != other.Rows || Columns != other.Columns)
            {
                return false;
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!GetValue(i, j).Equals(other.GetValue(i, j)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [DataMember]
        public T[][] Values { get; set; }
    }
}
