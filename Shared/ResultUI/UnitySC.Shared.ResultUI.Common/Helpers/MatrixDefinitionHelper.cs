using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;

namespace UnitySC.Shared.ResultUI.Common.Helpers
{
    public static class MatrixDefinitionHelper
    {
        public static MatrixDefinition Interpolate(MatrixDefinition matrix, CancellationToken? cancellationToken, int maxSize = 1000)
        {
            int currentSize = Math.Max(matrix.Width, matrix.Height);

            if (currentSize > maxSize)
            {
                float scale = maxSize / (float)currentSize;
                float[] newMatrixValues = BilinearScale(matrix.Values, matrix.Width, matrix.Height, scale, cancellationToken);

                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return null;

                return new MatrixDefinition
                {
                    Extension = matrix.Extension,
                    FileName = matrix.FileName,
                    Height = (int)(matrix.Height * scale),
                    Width = (int)(matrix.Width * scale),
                    Unit = matrix.Unit,
                    Values = newMatrixValues,
                    Resolution = scale,
                    PixelSizeX = matrix.PixelSizeX * scale,
                    PixelSizeY = matrix.PixelSizeY * scale,
                    PixelXUnit = matrix.PixelXUnit,
                    PixelYUnit = matrix.PixelYUnit
                };
            }

            return matrix;
        }

        private static float[] BilinearScale(float[] matrix, int matrixSizeX, int matrixSizeY, float scale, CancellationToken? cancellationToken)
        {
            int newWidth = (int)(matrixSizeX * scale);
            int newHeight = (int)(matrixSizeY * scale);

            float[] newMatrix = new float[newWidth * newHeight];

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return null;

                    float gx = (float)x / newWidth * (matrixSizeX - 1);
                    float gy = (float)y / newHeight * (matrixSizeY - 1);

                    int gxi = (int)gx;
                    int gyi = (int)gy;

                    float c00 = GetValue(matrix, matrixSizeX, gxi, gyi);
                    float c10 = GetValue(matrix, matrixSizeX, gxi + 1, gyi);
                    float c01 = GetValue(matrix, matrixSizeX, gxi, gyi + 1);
                    float c11 = GetValue(matrix, matrixSizeX, gxi + 1, gyi + 1);

                    float value = (c00 + c10 + c01 + c11) / 4;

                    newMatrix[x + y * newWidth] = value;
                }
            }

            return newMatrix;
        }

        private static float GetValue(IReadOnlyList<float> matrix, int matrixSizeX, int x, int y)
        {
            if (x < 0 || x > matrixSizeX) return float.NaN;
            int index = x + matrixSizeX * y;
            if (index >= matrix.Count) return float.NaN;
            return matrix[index];
        }
    }
}
