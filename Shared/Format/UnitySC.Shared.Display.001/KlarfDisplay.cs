using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using UnitySC.Shared.Data;
using UnitySC.Shared.Format._001;
using UnitySC.Shared.Format.Base;

namespace UnitySC.Shared.Display._001
{
    public sealed class KlarfDisplay : IResultDisplay
    {
        #region Fields

        private readonly int _thumbnailmarginpx = 1;
        private readonly int _thumbnailtargetimgsize = 256;
        private readonly float _thumbnaildefaultDisplayFactor = 1.05f;
        private readonly float _thumbnaildefaultDisplayMinSize = 1.0f;

        private readonly int _marginpx = 12;
        private readonly int _targetWaferSizepx = 3000;
        private readonly float _fViewPenSizepx = 5.0F;

        private bool _sizeBinUpdated = true;

        #endregion

        #region Properties

        public IExportResult ExportResult { get; } = new KlarfExportResult();

        public int NViewSizepx => _targetWaferSizepx + 2 * _marginpx;

        // KlarfSettingsData
        private DefectBins _table_defectBins;
        public DefectBins TableBinDefect => (DefectBins)_table_defectBins.Clone();

        private SizeBins _table_sizeBins;
        public SizeBins TableBinSize => (SizeBins)_table_sizeBins.Clone();

        #endregion
        
        public void UpdateInternalDisplaySettingsPrm(params object[] inprm)
        {
            if (inprm == null)
                throw new ArgumentNullException("KlarfDisplay.UpdateInternalDisplaySettingsPrm");

            // Arg in Prm expected : 0 DefectBins, 1 SizeBins
            if (inprm.Length == 0)
                throw new ArgumentException("KlarfDisplay.UpdateInternalDisplaySettingsPrm Empty inprm");

            if (inprm.Length > 2)
                throw new ArgumentException("KlarfDisplay.UpdateInternalDisplaySettingsPrm too much inprm");

            if (inprm.Length == 2)
            {
                // Arg in Prm expected : 0 DefectBins, 1 SizeBins
                _table_defectBins = (DefectBins)inprm[0];
                _table_sizeBins = (SizeBins)inprm[1];
                _sizeBinUpdated = true;
            }
            else
            {
                // only one arg
                if (inprm[0] is DefectBins)
                {
                    _table_defectBins = (DefectBins)inprm[0];
                }

                if (inprm[0] is SizeBins)
                {
                    _table_sizeBins = (SizeBins)inprm[0];
                    _sizeBinUpdated = true;
                }
            }
        }

        public Color GetColorCategory(IResultDataObject dataobj, string sCategoryName)
        {
            if (int.TryParse(sCategoryName, out int nRoughBinCode))
            {
                return GetColorCategory(nRoughBinCode);
            }
            return Color.Transparent;
        }
        
        public Color GetColorCategory(int roughBinCode)
        {
            if (_table_defectBins.RoughBinList.Contains(roughBinCode))
                return _table_defectBins.GetDefectBinColor(roughBinCode);
            return Color.Transparent;
        }

        public string GetCategoryName(int roughBinCode)
        {
            if (_table_defectBins.RoughBinList.Contains(roughBinCode))
                return _table_defectBins.GetDefectBinLabel(roughBinCode);
            return string.Format("Unknown_{0}", roughBinCode);
        }

        public Bitmap DrawImage(IResultDataObject dataobj, params object[] inprm)
        {
            if (inprm == null)
                throw new ArgumentNullException("KlarfDisplay.DrawImage");

            // Arg in Prm expected :  2 - 0 DisplayGrid (bool), 3 - 1 LayerToShow(list of rougbin code)
            if (inprm.Length != 2)
                throw new ArgumentException("KlarfDisplay.DrawImage");

            var KlarfData = dataobj as DataKlarf;
            var bLayerDisplayed = new Dictionary<int, bool>();

            bool bDisplayGrid = (bool)inprm[0];
            var LayerByRoughBin = (List<int>)inprm[1];
            if (LayerByRoughBin == null)
            {
                // Specific case - show all layer
                foreach (int key in KlarfData.RBinKeys)
                {
                    bLayerDisplayed.Add(key, true);
                }
            }
            else
            {
                foreach (int key in KlarfData.RBinKeys)
                {
                    bLayerDisplayed.Add(key, LayerByRoughBin.Contains(key));
                }
            }

            int nMarginpx = _marginpx;
            int nBmpSizepx = NViewSizepx;
            Graphics gImage = null;
            bool bIsDieModeUsed = KlarfData.SampleTestPlan.NbDies > 1;
            if (!bIsDieModeUsed)
                bDisplayGrid = false; //we cancel display grid if theis is not a die to die klarf result

            Bitmap ViewBmp;
            double lXConversionMicronToPixelAdjusted;
            double lYConversionMicronToPixelAdjusted;
            float Cx;
            float Cy;
            if (!KlarfData.IsSquaredWafer)
            {
                ViewBmp = new Bitmap(nBmpSizepx, nBmpSizepx);
                gImage = Graphics.FromImage(ViewBmp);
                //gImage.FillRectangle(Brushes.Black, 0, 0, ViewBmp.Width, ViewBmp.Height);
                //gImage.Clear(Color.Transparent);
                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, _targetWaferSizepx, _targetWaferSizepx); // pixel margin
                Cx = nMarginpx + WaferRectImage.Width * 0.5F;
                Cy = nMarginpx + WaferRectImage.Height * 0.5F;

                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / KlarfData.SampleSize.WaferDiameter_mm / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / KlarfData.SampleSize.WaferDiameter_mm / 1000;

                float dDiamNotchpx = 2500.0F * (float)lYConversionMicronToPixelAdjusted;
                DrawWaferShape(KlarfData, ref gImage, nMarginpx, nBmpSizepx, dDiamNotchpx, _fViewPenSizepx, bDisplayGrid);
            }
            else
            {
                nMarginpx = 60;
                int nSizepx = nBmpSizepx - 2 * nMarginpx;

                // on calcul le ratio X/Y
                float RectRatio = KlarfData.SquareSizemm.X / KlarfData.SquareSizemm.Y;
                // le plus grand coté sera de la taille choisi
                bool bXBiggerSize = KlarfData.SquareSizemm.X >= KlarfData.SquareSizemm.Y;
                int nSizepx_X = bXBiggerSize ? nSizepx : ((int)(nSizepx * RectRatio));
                int nSizepx_Y = bXBiggerSize ? ((int)(nSizepx / RectRatio)) : nSizepx;

                ViewBmp = new Bitmap(nSizepx_X + 2 * nMarginpx, nSizepx_Y + 2 * nMarginpx);
                gImage = Graphics.FromImage(ViewBmp);
                //gImage.FillRectangle(Brushes.Black, 0, 0, ViewBmp.Width, ViewBmp.Height);
                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nSizepx_X, nSizepx_Y); // pixel margin
                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / KlarfData.SquareSizemm.X / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / KlarfData.SquareSizemm.Y / 1000;

                Cx = nMarginpx + WaferRectImage.Width * 0.5F;
                Cy = nMarginpx + WaferRectImage.Height * 0.5F;

                gImage.FillRectangle(new Pen(Color.FromArgb(1, 1, 1)).Brush, WaferRectImage);
                var MyWhitePen = new Pen(Color.White, 2.0F);
                gImage.DrawRectangle(MyWhitePen, WaferRectImage);
                if (bIsDieModeUsed && bDisplayGrid)
                {
                    // draw DIE GRID
                    double XPos = 0.0;
                    double i = 0.0;
                    double dPitchX = KlarfData.DiePitch.X;
                    double dPitchY = KlarfData.DiePitch.Y;
                    var GridPen = new Pen(Color.FromArgb(240, 240, 240), _fViewPenSizepx * 0.5f)
                    {
                        DashStyle = System.Drawing.Drawing2D.DashStyle.Dot //Dash //DashDotDot //DashDot //Dot //Solid
                    };

                    gImage.DrawRectangle(new Pen(Color.FromArgb(1, 255, 1), _fViewPenSizepx * 0.5f),
                      (int)(KlarfData.DieOrigin.X * lXConversionMicronToPixelAdjusted + Cx), (int)((-KlarfData.DieOrigin.Y + (-1) * dPitchY) * lYConversionMicronToPixelAdjusted + (nSizepx_Y + 2 * nMarginpx) - Cy),
                     (int)(dPitchX * lXConversionMicronToPixelAdjusted), (int)(dPitchY * lYConversionMicronToPixelAdjusted));

                    if (dPitchX > 0.0 && dPitchY > 0.0)
                    {
                        while (XPos < WaferRectImage.Width) // X sens positif
                        {
                            XPos = (KlarfData.DieOrigin.X + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                            gImage.DrawLine(GridPen, new PointF((float)XPos, nMarginpx), new PointF((float)XPos, nMarginpx + nSizepx_Y));
                            i++;
                        }

                        i = -1.0;
                        XPos = (KlarfData.DieOrigin.X + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                        while (XPos >= 0.0) // X sens negatif
                        {
                            XPos = (KlarfData.DieOrigin.X + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                            gImage.DrawLine(GridPen, new PointF((float)XPos, nMarginpx), new PointF((float)XPos, nMarginpx + nSizepx_Y));
                            i--;
                        }

                        double YPos = 0.0; i = 0.0;
                        while (YPos < WaferRectImage.Height) // Y sens positif
                        {
                            YPos = (-KlarfData.DieOrigin.Y + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + (nSizepx_Y + 2 * nMarginpx) - Cy;
                            gImage.DrawLine(GridPen, new PointF(nMarginpx, (float)YPos), new PointF(nMarginpx + nSizepx_X, (float)YPos));
                            i++;
                        }

                        YPos = WaferRectImage.Height - nMarginpx;
                        i = -1.0;
                        while (YPos >= 0.0) // Y sens negatif
                        {
                            YPos = (-KlarfData.DieOrigin.Y + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + (nSizepx_Y + 2 * nMarginpx) - Cy;
                            gImage.DrawLine(GridPen, new PointF(nMarginpx, (float)YPos), new PointF(nMarginpx + nSizepx_X, (float)YPos));
                            i--;
                        }
                    }
                }
            }

            double dWaferRadius_um = KlarfData.SampleSize.WaferDiameter_mm * 500.0;
            var BinPens = new Dictionary<int, Pen>();
            if (KlarfData.DefectViewItemList == null || _sizeBinUpdated)
            {
                ComputeRcpxItemsList(KlarfData, _table_sizeBins, ViewBmp.Height, lXConversionMicronToPixelAdjusted, lYConversionMicronToPixelAdjusted, Cx, Cy);
                _sizeBinUpdated = false;
            }

            // Draw List Item
            if (KlarfData.DefectViewItemList != null && KlarfData.DefectViewItemList.ItemList != null)
            {
                foreach (var defItem in KlarfData.DefectViewItemList.ItemList)
                {
                    if (bLayerDisplayed[defItem.RoughBinKey])
                    {
                        if (!BinPens.ContainsKey(defItem.RoughBinKey))
                        {
                            BinPens.Add(defItem.RoughBinKey, new Pen(GetColorCategory(defItem.RoughBinKey)));
                        }
                        gImage.FillRectangle(BinPens[defItem.RoughBinKey].Brush, Rectangle.Round(defItem.Rectpx));
                    }
                }
            }

            ViewBmp.MakeTransparent(Color.Black);

            foreach (var pen in BinPens.Values)
                pen.Dispose();

            return ViewBmp;
        }

        public bool GenerateThumbnailFile(IResultDataObject p_Data, params object[] p_Inprm)
        {
            if (p_Inprm == null)
                throw new ArgumentNullException("KlarfDisplay.GenerateThumbnailFile");

            if (p_Inprm.Length != 2)
                throw new ArgumentException("KlarfDisplay.GenerateThumbnailFile");

            var Table_Defectbins = (DefectBins)p_Inprm[0];
            var Table_sizebins = (SizeBins)p_Inprm[1];

            int nMarginpx = _thumbnailmarginpx;
            int nThumbSizepx = _thumbnailtargetimgsize;
            Graphics gImage = null;
            var KlarfData = p_Data as DataKlarf;

            Bitmap ThumbBmp;
            double lXConversionMicronToPixelAdjusted;
            double lYConversionMicronToPixelAdjusted;
            float Cx;
            float Cy;
            if (!KlarfData.IsSquaredWafer)
            {
                ThumbBmp = new Bitmap(nThumbSizepx, nThumbSizepx);
                gImage = Graphics.FromImage(ThumbBmp);
                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nThumbSizepx - 2 * nMarginpx, nThumbSizepx - 2 * nMarginpx); // pixel margin
                Cx = nMarginpx + WaferRectImage.Width * 0.5F;
                Cy = nMarginpx + WaferRectImage.Height * 0.5F;

                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / KlarfData.SampleSize.WaferDiameter_mm / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / KlarfData.SampleSize.WaferDiameter_mm / 1000;

                float dDiamNotchpx = 3.0F;
                DrawWaferShape(KlarfData, ref gImage, nMarginpx, nThumbSizepx, dDiamNotchpx, 2.0F, false);
            }
            else
            {
                nMarginpx = 3;
                int nSizepx = nThumbSizepx - 2 * nMarginpx;

                // on calcul le ratio X/Y
                float RectRatio = KlarfData.SquareSizemm.X / KlarfData.SquareSizemm.Y;
                // le plus grand coté sera de la taille choisi
                bool bXBiggerSize = KlarfData.SquareSizemm.X >= KlarfData.SquareSizemm.Y;
                int nSizepx_X = bXBiggerSize ? nSizepx : ((int)(nSizepx * RectRatio));
                int nSizepx_Y = bXBiggerSize ? ((int)(nSizepx / RectRatio)) : nSizepx;

                ThumbBmp = new Bitmap(nSizepx_X + 2 * nMarginpx, nSizepx_Y + 2 * nMarginpx);
                gImage = Graphics.FromImage(ThumbBmp);
                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nSizepx_X, nSizepx_Y); // pixel margin
                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / KlarfData.SquareSizemm.X / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / KlarfData.SquareSizemm.Y / 1000;

                Cx = nMarginpx + WaferRectImage.Width * 0.5F;
                Cy = nMarginpx + WaferRectImage.Height * 0.5F;

                gImage.FillRectangle(new Pen(Color.FromArgb(1, 1, 1)).Brush, WaferRectImage);
                var MyWhitePen = new Pen(Color.White, 2.0F);
                gImage.DrawRectangle(MyWhitePen, WaferRectImage);
            }

            if (KlarfData.DefectViewItemList == null)
            {
                ComputeRcpxItemsList(KlarfData, Table_sizebins, ThumbBmp.Height, lXConversionMicronToPixelAdjusted, lYConversionMicronToPixelAdjusted, Cx, Cy);
            }

            var BinPens = new Dictionary<int, Pen>();
            // Draw List Item
            foreach (var defItem in KlarfData.DefectViewItemList.ItemList)
            {
                if (!BinPens.ContainsKey(defItem.RoughBinKey))
                {
                    BinPens.Add(defItem.RoughBinKey, new Pen(Table_Defectbins.GetDefectBinColorOrDefault(defItem.RoughBinKey)));
                }
                gImage.FillRectangle(BinPens[defItem.RoughBinKey].Brush, defItem.ApplyModifiers(_thumbnaildefaultDisplayFactor, _thumbnaildefaultDisplayMinSize));
            }

            ThumbBmp.MakeTransparent(Color.Black);

            string sThumbFilePath = FormatHelper.ThumbnailPathOf(KlarfData);
            Directory.CreateDirectory(Path.GetDirectoryName(sThumbFilePath));
            ThumbBmp.Save(sThumbFilePath, ImageFormat.Png);

            foreach (var pen in BinPens.Values)
                pen.Dispose();

            // clear thumbnails rects
            KlarfData.DefectViewItemList = null;

            return true;
        }

        public List<ResultDataStats> GenerateStatisticsValues(IResultDataObject p_Data, params object[] p_Inprm)
        {
            var KlarfData = p_Data as DataKlarf;
            return KlarfData.GetStats();
        }
        
        private void DrawWaferShape(DataKlarf p_KlarfData, ref Graphics p_gImage, int p_nMarginpx, int p_nSizepx, float p_dDiamNotchpx, float p_fViewPenSizepx, bool bDisplayGrid)
        {
            var WaferRectImage = new Rectangle(p_nMarginpx, p_nMarginpx, p_nSizepx - 2 * p_nMarginpx, p_nSizepx - 2 * p_nMarginpx); // pixel margin
            var MyWhitePen = new Pen(Color.White, p_fViewPenSizepx);
            float Cx = p_nMarginpx + WaferRectImage.Width * 0.5F;
            float Cy = p_nMarginpx + WaferRectImage.Height * 0.5F;

            bool bIsDieModeUsed = p_KlarfData.SampleTestPlan.NbDies > 1;
            int nWaferSize_mm = p_KlarfData.SampleSize.WaferDiameter_mm;

            p_gImage.FillEllipse(new Pen(Color.FromArgb(1, 1, 1)).Brush, WaferRectImage);

            if (bIsDieModeUsed && bDisplayGrid)
            {
                // draw DIE GRID
                double lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / nWaferSize_mm / 1000;
                double lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / nWaferSize_mm / 1000;

                double XPos = 0.0;
                double i = 0.0;
                double dPitchX = p_KlarfData.DiePitch.X;
                double dPitchY = p_KlarfData.DiePitch.Y;
                var GridPen = new Pen(Color.FromArgb(240, 240, 240), p_fViewPenSizepx * 0.5f)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Dot //Dash //DashDotDot //DashDot //Dot //Solid
                };

                p_gImage.DrawRectangle(new Pen(Color.FromArgb(1, 255, 1), p_fViewPenSizepx * 0.5f),
                    (int)(p_KlarfData.DieOrigin.X * lXConversionMicronToPixelAdjusted + Cx), (int)((-p_KlarfData.DieOrigin.Y + (-1) * dPitchY) * lYConversionMicronToPixelAdjusted + NViewSizepx - Cy),
                    (int)(dPitchX * lXConversionMicronToPixelAdjusted), (int)(dPitchY * lYConversionMicronToPixelAdjusted));

                if (dPitchX > 0.0 && dPitchY > 0.0)
                {
                    while (XPos < WaferRectImage.Width) // X sens positif
                    {
                        XPos = (p_KlarfData.DieOrigin.X + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                        p_gImage.DrawLine(GridPen, new PointF((float)XPos, p_nMarginpx), new PointF((float)XPos, p_nMarginpx + p_nSizepx));
                        i++;
                    }

                    XPos = WaferRectImage.Width - p_nMarginpx;
                    i = -1.0;
                    XPos = (p_KlarfData.DieOrigin.X + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                    while (XPos >= 0.0) // X sens negatif
                    {
                        XPos = (p_KlarfData.DieOrigin.X + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                        p_gImage.DrawLine(GridPen, new PointF((float)XPos, p_nMarginpx), new PointF((float)XPos, p_nMarginpx + p_nSizepx));
                        i--;
                    }

                    double YPos = 0.0; i = 0.0;
                    while (YPos < WaferRectImage.Height) // Y sens positif
                    {
                        YPos = (-p_KlarfData.DieOrigin.Y + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + NViewSizepx - Cy;
                        p_gImage.DrawLine(GridPen, new PointF(p_nMarginpx, (float)YPos), new PointF(p_nMarginpx + p_nSizepx, (float)YPos));
                        i++;
                    }

                    YPos = WaferRectImage.Height - p_nMarginpx;
                    i = -1.0;
                    while (YPos >= 0.0) // Y sens negatif
                    {
                        YPos = (-p_KlarfData.DieOrigin.Y + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + NViewSizepx - Cy;
                        p_gImage.DrawLine(GridPen, new PointF(p_nMarginpx, (float)YPos), new PointF(p_nMarginpx + p_nSizepx, (float)YPos));
                        i--;
                    }

                    //on supprime les elment exterieur à l'ellipse en les coloriant en noir
                    var rgn = new Region(new Rectangle(0, 0, p_nSizepx, p_nSizepx));
                    var EllispePath = new System.Drawing.Drawing2D.GraphicsPath();
                    EllispePath.AddEllipse(WaferRectImage);
                    rgn.Exclude(EllispePath);
                    p_gImage.FillRegion(Brushes.Black, rgn);
                }
            }

            p_gImage.DrawEllipse(MyWhitePen, WaferRectImage);
#if DEBUG
            //            // Debug draw Center of wafer
            //             Pen MyDBGPen = new Pen(Color.Green, p_fViewPenSizepx);
            //             p_gImage.DrawLine(MyDBGPen, new PointF(Cx-20, Cy), new PointF(Cx+20, Cy));
            //             p_gImage.DrawLine(MyDBGPen, new PointF(Cx, Cy-20), new PointF(Cx, Cy+20));
#endif

            switch (p_KlarfData.SampleOrientationMarkType.Value)
            {
                default:
                case PrmSampleOrientationMarkType.SomtType.NOTCH: // notch
                    {
                        var rectNotch = new RectangleF(Cx - p_dDiamNotchpx * 0.5F, p_nSizepx - p_dDiamNotchpx * 0.5F - p_nMarginpx, p_dDiamNotchpx, p_dDiamNotchpx);
                        p_gImage.FillEllipse(Brushes.Black, rectNotch);
                        float startAngle = 180.0F;
                        float sweepAngle = 180.0F;
                        p_gImage.DrawArc(MyWhitePen, rectNotch, startAngle, sweepAngle);
                    }
                    break;

                case PrmSampleOrientationMarkType.SomtType.FLAT: // Flat
                    {  // draw "meplat" -- grosso merdo 2.6% du diametre du wafer bouffer par le méplat.
                        float p = 2.6F / 100.0F;
                        float g = p_nMarginpx + (WaferRectImage.Height * (1.0F - p));
                        float r = WaferRectImage.Width * 0.5F;
                        float zeta = r * r - (Cx * Cx) - (Cy * Cy) + g * (2.0F * Cy - g); // intersection droite et cercle (x-Cx)² + (y-Cy)² = r² (où y=g=(1 - p)2r)
                        float Discriminant = 4.0F * (Cx * Cx + zeta);
                        float x1 = (2.0F * Cx - (float)Math.Sqrt(Discriminant)) / 2.0F;
                        float x2 = (2.0F * Cx + (float)Math.Sqrt(Discriminant)) / 2.0F;

                        // point intersection meplat et cercle wafer
                        var TLm = new PointF(x1, g);
                        var TRm = new PointF(x2, g);
                        var BLm = new PointF(x1, p_nSizepx - 1.0F);
                        var BRm = new PointF(x2, p_nSizepx - 1.0F);
                        p_gImage.FillRectangle(Brushes.Black, x1, g, p_nSizepx, p_nSizepx);
                        p_gImage.DrawLine(MyWhitePen, TLm, TRm);
                    }
                    break;

                case PrmSampleOrientationMarkType.SomtType.DFLAT: // double flat
                    {
                        // draw "meplat" Bas -- grosso merdo 2.6% du diametre du wafer bouffer par le méplat.
                        //
                        float p = 2.6F / 100.0F;
                        float g = p_nMarginpx + (WaferRectImage.Height * (1.0F - p));
                        float r = WaferRectImage.Width * 0.5F;
                        float zeta = r * r - (Cx * Cx) - (Cy * Cy) + g * (2.0F * Cy - g); // intersection droite et cercle (x-Cx)² + (y-Cy)² = r² (où y=g=(1 - p)2r)
                        float Discriminant = 4.0F * (Cx * Cx + zeta);
                        float x1 = (2.0F * Cx - (float)Math.Sqrt(Discriminant)) / 2.0F;
                        float x2 = (2.0F * Cx + (float)Math.Sqrt(Discriminant)) / 2.0F;

                        // point intersection meplat et cercle wafer
                        var TLmx = new PointF(x1, g);
                        var TRmx = new PointF(x2, g);
                        var BLmx = new PointF(x1, p_nSizepx - 1.0F);
                        var BRmx = new PointF(x2, p_nSizepx - 1.0F);
                        p_gImage.FillRectangle(Brushes.Black, x1, g, p_nSizepx, p_nSizepx);
                        p_gImage.DrawLine(MyWhitePen, TLmx, TRmx);

                        // draw "meplat" Gauche -- grosso merdo 2.6% du diametre du wafer bouffer par le méplat.
                        //
                        g = p_nMarginpx + (WaferRectImage.Width * p);
                        r = WaferRectImage.Height * 0.5F;
                        zeta = r * r - (Cy * Cy) - (Cx * Cx) + g * (2.0F * Cx - g); // intersection droite et cercle (x-Cx)² + (y-Cy)² = r² (où x=g=p2r)
                        Discriminant = 4.0F * (Cy * Cy + zeta);
                        float y1 = (2.0F * Cy - (float)Math.Sqrt(Discriminant)) / 2.0F;
                        float y2 = (2.0F * Cy + (float)Math.Sqrt(Discriminant)) / 2.0F;

                        // point intersection meplat et cercle wafer
                        var TLmy = new PointF(1.0F, y1);
                        var TRmy = new PointF(g, y1);
                        var BLmy = new PointF(1.0F, y2);
                        var BRmy = new PointF(g, y2);
                        p_gImage.FillRectangle(Brushes.Black, 1.0F, y1, (float)g, y2 - y1);
                        p_gImage.DrawLine(MyWhitePen, TRmy, BRmy);
                    }
                    break;
            }
        }

        private void ComputeRcpxItemsList(DataKlarf klarfData, SizeBins table_sizebins, double bmpHeight, double lXConversionMicronToPixelAdjusted, double lYConversionMicronToPixelAdjusted, double cx, double cy)
        {
            bool bIsDieModeUsed = klarfData.SampleTestPlan.NbDies > 1;
            double dWaferRadius_um = klarfData.SampleSize.WaferDiameter_mm * 500.0;

            klarfData.DefectViewItemList = new Format.Helper.RcpxItemList<KlarfDefect>();
            foreach (var oDef in klarfData.DefectList)
            {
                double lXRelatifFromWaferCenter;
                double lYRelatifFromWaferCenter;

                if (bIsDieModeUsed)
                {
                    // Wafer with Dies
                    int nDieIndexX = (int)oDef.Get("XINDEX");
                    int nDieIndexY = (int)oDef.Get("YINDEX");

                    // die pos relative to die origine of current die (X,Y)
                    lXRelatifFromWaferCenter = klarfData.DieOrigin.X + nDieIndexX * klarfData.DiePitch.X + (double)oDef.Get("XREL");
                    lYRelatifFromWaferCenter = klarfData.DieOrigin.Y + nDieIndexX * klarfData.DiePitch.Y + (double)oDef.Get("YREL");
                }
                else
                {
                    if (!klarfData.IsSquaredWafer)
                    {
                        // raw wafer
                        lXRelatifFromWaferCenter = (double)oDef.Get("XREL") - dWaferRadius_um;
                        lYRelatifFromWaferCenter = (double)oDef.Get("YREL") - dWaferRadius_um;
                    }
                    else
                    {
                        // rectangular raw wafer
                        lXRelatifFromWaferCenter = (double)oDef.Get("XREL") - klarfData.SquareSizemm.X * 500.0;
                        lYRelatifFromWaferCenter = (double)oDef.Get("YREL") - klarfData.SquareSizemm.Y * 500.0;
                    }
                }

                double XPos = lXRelatifFromWaferCenter * lXConversionMicronToPixelAdjusted + cx;
                double YPos = lYRelatifFromWaferCenter * -1.0 * lYConversionMicronToPixelAdjusted + bmpHeight - cy;

                double defectSizeX_um = (double)oDef.Get("XSIZE");
                double defectSizeY_um = (double)oDef.Get("YSIZE");
                int RoughBinCode = (int)oDef.Get("ROUGHBINNUMBER");
                double dDefectArea_um2 = (double)oDef.Get("DEFECTAREA");
                int SquareWidth_um = table_sizebins.GetSquareWidth((long)dDefectArea_um2);

                double dSizeX = SquareWidth_um * lXConversionMicronToPixelAdjusted;
                if (dSizeX < 1.0)
                    dSizeX = 1.0;
                double dSizeY = SquareWidth_um * lYConversionMicronToPixelAdjusted;
                if (dSizeY < 1.0)
                    dSizeY = 1.0;

                if (klarfData.CoordinatesCentered.Value)
                {
                    XPos -= dSizeX * 0.5;
                    YPos -= dSizeY * 0.5;
                }
                else
                {
                    // origin defined as bottom left of rectangle
                    YPos += defectSizeY_um * lYConversionMicronToPixelAdjusted;
                    YPos -= dSizeY;
                }

                klarfData.DefectViewItemList.AddItem(RoughBinCode.ToString(), new KlarfDefect(oDef, new RectangleF((float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY)));
            }
            klarfData.DefectViewItemList.NotifyAddCompleted();
        }
    }
}
