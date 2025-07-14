using System;

namespace UnitySC.Shared.Tools
{
    public class ArrayUtils
    {
        public static T[,] JaggedArrayTo2D<T>(T[][] jagged)
        {
            try
            {
                int rows = jagged.Length;
                int cols = jagged[0].Length;

                var result = new T[rows, cols];
                for (int i = 0; i < rows; ++i)
                {
                    for (int j = 0; j < cols; ++j)
                    {
                        result[i, j] = jagged[i][j];
                    }
                }
                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }
    }
}
