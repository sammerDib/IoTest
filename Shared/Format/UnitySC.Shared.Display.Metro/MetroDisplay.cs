using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Display.Metro
{
    public sealed class MetroDisplay : IResultDisplay
    {
        private const int ImageMarginPx = 1;
        private const int ImageTargetImageSize = 512;

        private const int ThumbnailMarginPx = 1;
        private const int ThumbnailTargetImageSize = 256;

        private const int BowTextFontSize = 22;
        private const int WarpTextFontSize = 18;

        #region IResultDisplay Members

        public IExportResult ExportResult { get; } = new MetroExportResult();

        public Bitmap DrawImage(IResultDataObject dataobj, params object[] inprm)
        {
            if (dataobj == null)
                throw new ArgumentNullException($"{nameof(MetroDisplay)}.{nameof(GenerateThumbnailFile)} {nameof(dataobj)} must be defined");

            if (!(dataobj is MetroResult dataMetro))
                throw new ArgumentNullException($"{nameof(MetroDisplay)}.{nameof(GenerateThumbnailFile)} {nameof(dataobj)} is not of type {nameof(MetroResult)}");

            var image = new Bitmap(ImageTargetImageSize, ImageTargetImageSize);
            ProcessBitmapFromData(dataMetro, image, ImageMarginPx);

            return image;
        }

        public bool GenerateThumbnailFile(IResultDataObject dataobj, params object[] inprm)
        {
            if (dataobj == null)
                throw new ArgumentNullException($"{nameof(MetroDisplay)}.{nameof(GenerateThumbnailFile)} {nameof(dataobj)} must be defined");

            if (!(dataobj is MetroResult dataMetro))
                throw new InvalidCastException($"{nameof(MetroDisplay)}.{nameof(GenerateThumbnailFile)} {nameof(dataobj)} is not of type {nameof(MetroResult)}");

            var thumbBitmap = new Bitmap(ThumbnailTargetImageSize, ThumbnailTargetImageSize);

            switch (dataobj.ResType)
            {
                case ResultType.ANALYSE_Bow:
                    Length bow = ComputeBowValue(dataMetro);
                    ProcessBitmapFromDataLength(dataMetro, bow, thumbBitmap, BowTextFontSize, false, ThumbnailMarginPx);
                    break;

                case ResultType.ANALYSE_Warp:
                    Length warp = ComputeWarpValue(dataMetro);
                    ProcessBitmapFromData(dataMetro, thumbBitmap, ThumbnailMarginPx);
                    ProcessBitmapFromDataLength(dataMetro, warp, thumbBitmap, WarpTextFontSize, true, ThumbnailMarginPx);
                    break;

                default:
                    ProcessBitmapFromData(dataMetro, thumbBitmap, ThumbnailMarginPx);
                    break;
            }

            string sThumbFilePath = FormatHelper.ThumbnailPathOf(dataMetro);
            string directoryName = Path.GetDirectoryName(sThumbFilePath);

            if (string.IsNullOrEmpty(directoryName))
                return false;

            Directory.CreateDirectory(directoryName);
            thumbBitmap.Save(sThumbFilePath, ImageFormat.Png);

            return true;
        }

        public List<ResultDataStats> GenerateStatisticsValues(IResultDataObject dataobj, params object[] inprm)
        {
            if (!(dataobj is MetroResult metroResult))
                throw new ArgumentNullException($"{nameof(MetroDisplay)}.{nameof(GenerateStatisticsValues)} {nameof(dataobj)} is not of type {nameof(MetroResult)}");

            return metroResult.GenerateStatisticsValues();
        }

        public void UpdateInternalDisplaySettingsPrm(params object[] inprm)
        {
            throw new NotImplementedException();
        }

        public Color GetColorCategory(IResultDataObject dataobj, string sCategoryName)
        {
            throw new NotImplementedException();
        }

        #endregion IResultDisplay Members

        #region Private Methods

        private static Length ComputeBowValue(MetroResult dataMetro)
        {
            Length meanBow = 0.Micrometers();
            if (dataMetro.MeasureResult.Points.Count > 0)
            {
                int cnt = 0;
                foreach (BowPointResult i in dataMetro.MeasureResult.Points)
                {
                    foreach (BowTotalPointData measure in i.Datas)
                    {
                        if (measure?.Bow != null)
                        {
                            meanBow = meanBow + measure.Bow;
                            cnt++;
                        }
                    }
                }
                if (cnt > 0)
                {
                    meanBow = meanBow / cnt;
                }
            }
            return meanBow;
        }

        private static Length ComputeWarpValue(MetroResult dataMetro)
        {
            Length meanWarp = 0.Micrometers();

            if ((dataMetro.MeasureResult as WarpResult).WarpWaferResults.Count > 0)
            {
                int cnt = 0;
                foreach (var measure in (dataMetro.MeasureResult as WarpResult).WarpWaferResults)
                {
                    if (measure != null)
                    {
                        meanWarp = meanWarp + measure;
                        cnt++;

                    }
                }
                if (cnt > 0)
                {
                    meanWarp = meanWarp / cnt;
                }
            }
            return meanWarp;
        }

        private static void ProcessBitmapFromDataLength(MetroResult dataMetro, Length measureValue, Bitmap bitmap, int fontSize, bool isWarpProcess, int margin = 0)
        {
            dataMetro.MeasureResult.FillNonSerializedProperties(false, false);

            int drawableWidth = bitmap.Width - 2 * margin;
            int drawableHeight = bitmap.Height - 2 * margin;

            int halfImageWidth = drawableWidth / 2;
            int halfImageHeight = drawableHeight / 2;

            var greenSquare = Properties.Resources.square;
            var orangeCross = Properties.Resources.cross;
            var redDiagonalCross = Properties.Resources.diagonalCross;

            int halfStickerSize = greenSquare.Width / 2;

            int symbolPointX = -halfStickerSize + margin + halfImageWidth;
            int symbolPointY = -halfStickerSize + margin + 2 * halfImageHeight / 3;

            var graphics = Graphics.FromImage(bitmap);
            if (!isWarpProcess)
            {
                graphics.FillEllipse(Brushes.Black, margin, margin, drawableWidth, drawableHeight);
            }

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Font thumbnailTextFont = new Font(FontFamily.GenericSansSerif, (fontSize * drawableWidth) / 256, FontStyle.Bold);

            String thumbnailValueText = String.Format("{0:0.00}", measureValue);
            var stringSize = graphics.MeasureString(thumbnailValueText, thumbnailTextFont);

            int textPointX = (int)(halfImageWidth - stringSize.Width / 2);
            int textPointY = (int)(4 * halfImageHeight / 3 - stringSize.Height / 2);

            if (isWarpProcess)
            {
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 240, 248, 255)), textPointX, textPointY, stringSize.Width, stringSize.Height);
            }

            switch (dataMetro.MeasureResult.State)
            {
                case GlobalState.Success:
                    graphics.DrawString(thumbnailValueText, thumbnailTextFont,
                        new SolidBrush(Color.Green), textPointX, textPointY);
                    if (!isWarpProcess)
                        graphics.DrawImage(greenSquare, symbolPointX, symbolPointY);
                    break;

                case GlobalState.Partial:
                    graphics.DrawString(thumbnailValueText, thumbnailTextFont,
                       new SolidBrush(Color.Orange), textPointX, textPointY);
                    if (!isWarpProcess)
                        graphics.DrawImage(orangeCross, symbolPointX, symbolPointY);
                    break;

                case GlobalState.Error:
                    graphics.DrawString(thumbnailValueText, thumbnailTextFont,
                        new SolidBrush(Color.Red), textPointX, textPointY);
                    if (!isWarpProcess)
                        graphics.DrawImage(redDiagonalCross, symbolPointX, symbolPointY);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ProcessBitmapFromData(MetroResult dataMetro, Bitmap bitmap, int margin = 0)
        {
            dataMetro.MeasureResult.FillNonSerializedProperties(false, false);

            double waferMaxDiameter = dataMetro.MeasureResult.Wafer.Diameter.Millimeters;
            int drawableWidth = bitmap.Width - 2 * margin;
            int drawableHeight = bitmap.Height - 2 * margin;
            int halfImageWidth = drawableWidth / 2;
            int halfImageHeight = drawableHeight / 2;
            double widthRatio = drawableWidth / waferMaxDiameter;
            double heightRatio = drawableHeight / waferMaxDiameter * -1; // Y axis is reverted for Bitmap

            var greenSquare = Properties.Resources.square;
            var orangeCross = Properties.Resources.cross;
            var redDiagonalCross = Properties.Resources.diagonalCross;
            var purpleTriangle = Properties.Resources.triangle;

            int halfStickerSize = greenSquare.Width / 2;
            int xRescaleTranslation = -halfStickerSize + margin + halfImageWidth;
            int yRescaleTranslation = -halfStickerSize + margin + halfImageHeight;

            var graphics = Graphics.FromImage(bitmap);
            graphics.FillEllipse(Brushes.Black, margin, margin, drawableWidth, drawableHeight);

            var points = dataMetro.MeasureResult.GetAllPoints();

            foreach (var measurePointResult in points)
            {
                // Coordinates of the top-left corner of the sticker that will represent them
                int rescaledPointX = (int)(measurePointResult.WaferRelativeXPosition * widthRatio + xRescaleTranslation);
                int rescaledPointY = (int)(measurePointResult.WaferRelativeYPosition * heightRatio + yRescaleTranslation);

                switch (measurePointResult.State)
                {
                    case MeasureState.Success:
                        graphics.DrawImage(greenSquare, rescaledPointX, rescaledPointY);
                        break;

                    case MeasureState.Partial:
                        graphics.DrawImage(orangeCross, rescaledPointX, rescaledPointY);
                        break;

                    case MeasureState.Error:
                        graphics.DrawImage(redDiagonalCross, rescaledPointX, rescaledPointY);
                        break;

                    case MeasureState.NotMeasured:
                        graphics.DrawImage(purpleTriangle, rescaledPointX, rescaledPointY);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }



        #endregion Private Methods
    }
}
