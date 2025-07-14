using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnitySC.Shared.Format.HAZE;
using UnitySC.Shared.Format.Base;
using ColorMap = UnitySC.Shared.Data.ColorMap.ColorMap;
using UnitySC.Shared.Data.ColorMap;

namespace UnitySC.Shared.Display.HAZE
{
    public sealed class HazeDisplay : IResultDisplay
    {
        #region Fields

        private const int ThumbnailMarginPx = 1;
        private const int ThumbnailTargetImageSize = 256;

        #endregion

        #region Properties

        public IExportResult ExportResult { get; } = new ExportResultBase<DataHaze>();

        public ColorMap TableColorMap { get; private set; }

        #endregion

        public Bitmap DrawImage(IResultDataObject dataobj, params object[] inprm)
        {
            if (inprm == null || inprm.Length != 1 || !(inprm[0] is HazeImageConfiguration))
            {
                throw new ArgumentNullException($"{nameof(HazeDisplay)}.{nameof(DrawImage)} {nameof(inprm)} must contain an istance of {nameof(HazeImageConfiguration)} at index 0");
            }

            if (!(dataobj is DataHaze dataHaze))
            {
                throw new ArgumentNullException($"{nameof(HazeDisplay)}.{nameof(DrawImage)} {nameof(dataobj)} must be defined");
            }

            var configuration = (HazeImageConfiguration)inprm[0];

            if (configuration.HazeMapIndex < 0 || configuration.HazeMapIndex >= dataHaze.HazeMaps.Count)
            {
                throw new IndexOutOfRangeException($"{nameof(HazeDisplay)}.{nameof(DrawImage)} {nameof(HazeImageConfiguration)}.{nameof(HazeImageConfiguration.HazeMapIndex)} is out of range");
            }

            var hazeMap = dataHaze.HazeMaps[configuration.HazeMapIndex];

            // on utilise la colormap setter dans le display
            var colors = TableColorMap.Colors;
            int colorsLength = colors.Length;

            var bitmap = new Bitmap(hazeMap.Width, hazeMap.Heigth);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);

            float range = configuration.MaxValue - configuration.MinValue;
            float a = colorsLength / range;
            float b = -configuration.MinValue * colorsLength / range;

            Color GetPixel(int x, int y)
            {
                float value = GetMeasureFromCoordinate(x, y, hazeMap.HazeMeasures, hazeMap.Width);
                int colorIndex = GetColorIndexFromValue(value, a, b, colorsLength);
                return colorIndex < 0 ? Color.Transparent : colors[colorIndex];
            }

            ProcessBitmap(bitmap, 0, GetPixel);

            return bitmap;
        }

        public bool GenerateThumbnailFile(IResultDataObject dataobj, params object[] inprm)
        {
            if (inprm == null)
                throw new ArgumentNullException($"{nameof(HazeDisplay)}.{nameof(GenerateThumbnailFile)}");

            // Arg in Prm expected : 0 default haze colormap name
            if (inprm.Length != 1)
                throw new ArgumentException($"{nameof(HazeDisplay)}.{nameof(GenerateThumbnailFile)} Bad Argument Size {nameof(inprm)}");

            string colorMapHazeSettingsName = (string)inprm[0];
            var colorMap = ColorMapHelper.ColorMaps.SingleOrDefault(map => map.Name.Equals(colorMapHazeSettingsName));

            if(colorMap == null)
                throw new ArgumentNullException($"{nameof(HazeDisplay)}.{nameof(GenerateThumbnailFile)} ColorMap named '{colorMapHazeSettingsName}' not found");

            var colors = colorMap.Colors;
            int colorsLength = colors.Length;

            var hazeData = dataobj as DataHaze;
            if (hazeData == null) 
                throw new ArgumentNullException($"{nameof(HazeDisplay)}.{nameof(GenerateThumbnailFile)} {nameof(dataobj)} is not of type {nameof(DataHaze)}");

            // Get the first HazeMap
            var hazeMap = hazeData.HazeMaps.First();

            var thumbBitmap = new Bitmap(ThumbnailTargetImageSize, ThumbnailTargetImageSize);
            var graphics = Graphics.FromImage(thumbBitmap);
            graphics.Clear(Color.Transparent);

            const int rectSize = ThumbnailTargetImageSize - 2 * ThumbnailMarginPx;
            float range = hazeMap.Max_ppm - hazeMap.Min_ppm;
            float a = colorsLength / range;
            float b = -hazeMap.Min_ppm * colorsLength / range;

            // It is necessary to add 1 to the final ratio to avoid truncating the source
            int heightRatio = hazeMap.Heigth / rectSize + 1;
            int widthRatio = hazeMap.Width / rectSize + 1;

            Color GetPixel(int x, int y)
            {
                float value = GetMeasureFromCoordinate(x, y, widthRatio, heightRatio, hazeMap.HazeMeasures, hazeMap.Width, hazeMap.Heigth);
                int colorIndex = GetColorIndexFromValue(value, a, b, colorsLength);
                return colorIndex < 0 ? Color.Transparent : colors[colorIndex];
            }

            ProcessBitmap(thumbBitmap, ThumbnailMarginPx, GetPixel);

            string sThumbFilePath = FormatHelper.ThumbnailPathOf(hazeData);
            string directoryName = Path.GetDirectoryName(sThumbFilePath);
            if (string.IsNullOrEmpty(directoryName)) return false;
            Directory.CreateDirectory(directoryName);
            thumbBitmap.Save(sThumbFilePath, ImageFormat.Png);

            return true;
        }

        #region Privates

        private static void ProcessBitmap(Bitmap bitmap, int margin, Func<int, int, Color> getPixel)
        {
            var sw = new Stopwatch();
            sw.Start();

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            var ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            int heightInPixels = bitmapData.Height - 2 * margin;
            int widthInBytes = (bitmapData.Width - 2 * margin) * bytesPerPixel;

            Parallel.For(0, heightInPixels, y =>
            {
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    var color = getPixel(x / 4, y);
                    pixels[currentLine + x] = color.B;
                    pixels[currentLine + x + 1] = color.G;
                    pixels[currentLine + x + 2] = color.R;
                    pixels[currentLine + x + 3] = color.A;
                }
            });

            // Copy modified bytes back
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            sw.Stop();
            Console.WriteLine($"Processed using ProcessBitmap method in {sw.ElapsedMilliseconds} ms.");
        }

        private static int GetColorIndexFromValue(float value, float a, float b, int colorsLength)
        {
            if (value == 0) return -1;
            float fVal = value * a + b;
            int colorIndex = (int)Math.Round(fVal);
            if (colorIndex < 0) colorIndex = 0;
            else if (colorIndex >= colorsLength)
                colorIndex = colorsLength - 1;

            return colorIndex;
        }

        private static float GetMeasureFromCoordinate(int x, int y, int xRatio, int yRatio, float[] measures, int rowSize, int columnSize)
        {
            int columnStart = x * xRatio;
            int rowStart = y * yRatio;
            float[] evaluatedMeasures = new float[xRatio * yRatio];
            int arrayIndex = 0;

            for (int effectiveY = 0; effectiveY < yRatio; effectiveY++)
            {
                int currentRow = (rowStart + effectiveY) * rowSize;
                if (currentRow >= measures.Length) break;

                for (int effectiveX = 0; effectiveX < xRatio; effectiveX++)
                {
                    int currentColumn = effectiveX + columnStart;
                    if (currentColumn >= columnSize) break;

                    int valueIndex = currentColumn + currentRow;
                    if (valueIndex >= measures.Length) break;
                    float value = measures[valueIndex];
                    evaluatedMeasures[arrayIndex] = value;
                    arrayIndex++;
                }
            }
            return evaluatedMeasures.Average();
        }

        private static float GetMeasureFromCoordinate(int x, int y, float[] measures, int rowSize)
        {
            int valueIndex = x + y * rowSize;
            if (valueIndex >= measures.Length) return 0;
            return measures[valueIndex];
        }

        #endregion
        
        public List<ResultDataStats> GenerateStatisticsValues(IResultDataObject dataobj, params object[] inprm)
        {
            var hazeData = dataobj as DataHaze;
            if (hazeData == null) throw new ArgumentNullException($"{nameof(HazeDisplay)}.{nameof(GenerateStatisticsValues)} {nameof(dataobj)} is not of type {nameof(DataHaze)}");
            return hazeData.GetStats();
        }
        
        public Color GetColorCategory(IResultDataObject dataobj, string sCategoryName)
        {
            throw new NotImplementedException();
        }
        
        public void UpdateInternalDisplaySettingsPrm(params object[] inprm)
        {
            if (inprm == null)
                throw new ArgumentNullException("HazeDisplay.UpdateInternalDisplaySettingsPrm");

            // Arg in Prm expected : 0 ColorMap
            if (inprm.Length == 0)
                throw new ArgumentException("HazeDisplay.UpdateInternalDisplaySettingsPrm Empty inprm");

            if (inprm.Length > 1)
                throw new ArgumentException("HazeDisplay.UpdateInternalDisplaySettingsPrm too much inprm");

            if (inprm.Length == 1)
            {
                // Arg in Prm expected : 0 ColorMap Name
                string colormapname = (string)inprm[0];
                var Defaultcolormap = ColorMapHelper.ColorMaps.SingleOrDefault(map => map.Name.Equals(colormapname));
                TableColorMap = Defaultcolormap;
            }
        }
    }
}
