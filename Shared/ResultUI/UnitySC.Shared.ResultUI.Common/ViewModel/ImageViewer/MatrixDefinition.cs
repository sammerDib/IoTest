using System;
using System.IO;
using System.Threading;

using UnitySC.Shared.Data.FormatFile;

using static UnitySC.Shared.Data.FormatFile.MatrixFloatFile;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public class MatrixDefinition
    {
        public float[] Values { get; set; }
        public float[] SortedValues { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string Unit { get; set; }
        public double PixelSizeX { get; set; }
        public double PixelSizeY { get; set; }
        public string PixelXUnit { get; set; }
        public string PixelYUnit { get; set; }

        /// <summary>
        /// Gets or sets the resolution of the matrix relative to its original matrix.
        /// </summary>
        public double Resolution { get; set; } = 1.0;        
        public static MatrixDefinition FromMatrixFloatFile(MatrixFloatFile format3daFile)
        {
            float[] values = MatrixFloatFile.AggregateChunks(format3daFile.GetChunkStatus(), format3daFile);
            return new MatrixDefinition
                {
                    Values = values,
                    SortedValues = null,
                    Height = format3daFile.Header.Height,
                    Width = format3daFile.Header.Width,
                    Unit = format3daFile.Header.AdditionnalHeaderData.UnitLabelZ,
                    Extension = "png",
                    FileName = "Thumbnail.png",
                    PixelSizeX = format3daFile.Header.AdditionnalHeaderData.PixelSizeX,
                    PixelSizeY = format3daFile.Header.AdditionnalHeaderData.PixelSizeY,
                    PixelXUnit = format3daFile.Header.AdditionnalHeaderData.UnitLabelX,
                    PixelYUnit = format3daFile.Header.AdditionnalHeaderData.UnitLabelY
                };
        }
        /// <summary>
        /// Process a matrix and writes it to a file, possibly converting it to BCRF format.
        /// </summary>
        /// <param name="matrix">The matrix to process and write to a file.</param>
        /// <param name="filePath">The path to the file where the matrix will be written.</param>
        public static void ProcessMatrix(MatrixDefinition matrix, string filePath)
        {
            try
            {
                CheckValidMatrix(matrix);

                var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
                HeaderData header = CreateHeaderData(matrix);
                switch (fileExtension)
                {
                    default:
                    case ".3da":
                        bool useCompression = true;
                        using (var mff = new MatrixFloatFile())
                        {
                            mff.WriteInFile(filePath, header, matrix.Values, useCompression);
                        }
                        break;

                    case ".bcrf":
                        using (var mff = new MatrixFloatFile())
                        {
                           var buffer = mff.WriteInMemory(header, matrix.Values, false);
                           mff.ToBCRF_File(filePath);
                        }
                        break;

                }
            }
            catch (ArgumentException)
            {
                throw ;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected exceptions when exporting to a 3D file - {ex.Message}");
            }            
        }

        private static void CheckValidMatrix(MatrixDefinition matrix)
        {
            if (matrix == null || (matrix.Values == null) || (matrix.Values.Length == 0) )
            {
                throw new ArgumentException("The matrix is null or empty.");
            }
        }

        private static HeaderData CreateHeaderData(MatrixDefinition matrix)
        {
            HeaderData header = new HeaderData
            {
                Width = matrix.Width,
                Height = matrix.Height,
                Version = FORMAT_VERSION
            };
            AdditionnalHeaderData additionalHeaderData = new AdditionnalHeaderData
            {
                PixelSizeX = matrix.PixelSizeX,
                PixelSizeY = matrix.PixelSizeY,
                UnitLabelX = matrix.PixelXUnit,
                UnitLabelY = matrix.PixelYUnit,
                UnitLabelZ = matrix.Unit
            };
            header.AdditionnalHeaderData = additionalHeaderData;
            return header;
        }


        /// <summary>
        /// Truncates the matrix according to the defined rectangle.
        /// </summary>
        /// <param name="startX">X coordinate of the Top-Left point of the area.</param>
        /// <param name="startY">Y coordinate of the Top-Left point of the area.</param>
        /// <param name="endX">X coordinate of the Bottom-Right point of the area.</param>
        /// <param name="endY">Y coordinate of the Bottom-Right point of the area.</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        public MatrixDefinition ReduceArea(int startX, int startY, int endX, int endY, CancellationToken? cancellationToken)
        {
            int newWidth = endX - startX;
            int newHeight = endY - startY;

            float[] newValues = new float[newWidth * newHeight];

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested) return null;

                    float value = GetValue(x, y);
                    int newX = x - startX;
                    int newY = y - startY;

                    newValues[newX + newY * newWidth] = value;
                }
            }

            return new MatrixDefinition
            {
                Extension = Extension,
                FileName = FileName,
                Unit = Unit,
                Values = newValues,
                Height = newHeight,
                Width = newWidth,
                PixelSizeX = PixelSizeX,
                PixelSizeY = PixelSizeY,
                PixelXUnit = PixelXUnit,
                PixelYUnit = PixelYUnit
            };
        }

        private float GetValue(int x, int y)
        {
            if (x < 0 || x > Width) return float.NaN;
            int index = x + Width * y;
            if (index >= Values.Length) return float.NaN;
            return Values[index];
        }

        public double GetRealX(int x)
        {
            return x * PixelSizeX;
        }

        public string GetRealXAsString(int x)
        {
            return $"{GetRealX(x)} {PixelXUnit}";
        }

        public double GetRealY(int y)
        {
            return y * PixelSizeY;
        }

        public string GetRealYAsString(int y)
        {
            return $"{GetRealY(y)} {PixelYUnit}";
        }
    }
}
