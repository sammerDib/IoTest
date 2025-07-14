using System;

namespace UnitySC.Shared.Tools
{
    public static class ArrayExtensions
    {
        public static void Fill<T>(this T[] array, T value, int startIndex, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (startIndex < 0 || count < 0 || startIndex + count > array.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid start index or count.");
            }

            for (int i = startIndex; i < startIndex + count; ++i)
            {
                array[i] = value;
            }
        }
    }
}
