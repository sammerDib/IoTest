using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using UnitySC.Shared.Format.ASO;
using UnitySC.Shared.Format.Base;

namespace UnitySC.Shared.Display.ASO
{
    public sealed class AsoDisplay : IResultDisplay
    {
        #region Fields

        private readonly int _thumbnailmarginpx = 1;
        private readonly int _thumbnailtargetimgsize = 256;
        private readonly float _thumbnaildefaultDisplayFactor = 1.05f;
        private readonly float _thumbnaildefaultDisplayMinSize = 2.0f;

        private readonly int _marginpx = 50;
        private readonly int _targetWaferSizepx = 3000;
        private readonly float _fViewPenSizepx = 5.0F;

        #endregion

        #region Properties

        public int NViewSizepx { get => _targetWaferSizepx + 2 * _marginpx; }

        public float DisplayFactor { get; private set; } = 1.0f;

        public float DisplayMinSize { get; private set; } = 5.0f;

        public IExportResult ExportResult { get; } = new AsoExportResult();

        #endregion

        #region WAFER DISPLAY 
        private void DrawWaferShape(DataAso asoData, ref Graphics p_gImage, int p_nMarginpx, int p_nSizepx, float p_dDiamNotchpx, float p_fViewPenSizepx, bool bDisplayGrid)
        {
            var WaferRectImage = new Rectangle(p_nMarginpx, p_nMarginpx, p_nSizepx - 2 * p_nMarginpx, p_nSizepx - 2 * p_nMarginpx); // pixel margin
            var MyWhitePen = new Pen(Color.White, p_fViewPenSizepx);
            float Cx = p_nMarginpx + WaferRectImage.Width * 0.5F;
            float Cy = p_nMarginpx + WaferRectImage.Height * 0.5F;

            bool bIsDieModeUsed = asoData.UseDieGridDisplay;
            int nWaferSize_mm = asoData.WaferSizeX_mm;

            p_gImage.FillEllipse(new Pen(Color.FromArgb(80, 80, 80)).Brush, WaferRectImage);

            if (bIsDieModeUsed && bDisplayGrid)
            {
                // draw DIE GRID
                double lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / nWaferSize_mm / 1000;
                double lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / nWaferSize_mm / 1000;

                double XPos = 0.0;
                double i = 0.0;
                double dPitchX = (double)asoData.DiePitchX;
                double dPitchY = (double)asoData.DiePitchY;
                var GridPen = new Pen(Color.FromArgb(240, 240, 240), p_fViewPenSizepx * 0.5f)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Dot //Dash //DashDotDot //DashDot //Dot //Solid
                };

                p_gImage.DrawRectangle(new Pen(Color.FromArgb(1, 255, 1), p_fViewPenSizepx * 0.5f),
                    (int)(asoData.DieOriginX * lXConversionMicronToPixelAdjusted + Cx), (int)((-asoData.DieOriginY + (-1) * dPitchY) * lYConversionMicronToPixelAdjusted + NViewSizepx - Cy),
                    (int)(dPitchX * lXConversionMicronToPixelAdjusted), (int)(dPitchY * lYConversionMicronToPixelAdjusted));

                if (dPitchX > 0.0 && dPitchY > 0.0)
                {
                    while (XPos < WaferRectImage.Width) // X sens positif
                    {
                        XPos = (asoData.DieOriginX + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                        p_gImage.DrawLine(GridPen, new PointF((float)XPos, p_nMarginpx), new PointF((float)XPos, p_nMarginpx + p_nSizepx));
                        i++;
                    }

                    XPos = WaferRectImage.Width - p_nMarginpx;
                    i = -1.0;
                    XPos = (asoData.DieOriginX + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                    while (XPos >= 0.0) // X sens negatif
                    {
                        XPos = (asoData.DieOriginX + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                        p_gImage.DrawLine(GridPen, new PointF((float)XPos, p_nMarginpx), new PointF((float)XPos, p_nMarginpx + p_nSizepx));
                        i--;
                    }

                    double YPos = 0.0; i = 0.0;
                    while (YPos < WaferRectImage.Height) // Y sens positif
                    {
                        YPos = (-asoData.DieOriginY + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + NViewSizepx - Cy;
                        p_gImage.DrawLine(GridPen, new PointF(p_nMarginpx, (float)YPos), new PointF(p_nMarginpx + p_nSizepx, (float)YPos));
                        i++;
                    }

                    YPos = WaferRectImage.Height - p_nMarginpx;
                    i = -1.0;
                    while (YPos >= 0.0) // Y sens negatif
                    {
                        YPos = (-asoData.DieOriginY + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + NViewSizepx - Cy;
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

            switch (asoData.OrientationMarkType)
            {
                default:
                case DataAso.OrientationMark.en_NOTCH: // notch
                    {
                        var rectNotch = new RectangleF(Cx - p_dDiamNotchpx * 0.5F, p_nSizepx - p_dDiamNotchpx * 0.5F - p_nMarginpx, p_dDiamNotchpx, p_dDiamNotchpx);
                        p_gImage.FillEllipse(Brushes.Black, rectNotch);
                        float startAngle = 180.0F;
                        float sweepAngle = 180.0F;
                        p_gImage.DrawArc(MyWhitePen, rectNotch, startAngle, sweepAngle);
                    }
                    break;

                case DataAso.OrientationMark.en_FLAT: // Flat
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

                case DataAso.OrientationMark.en_DOUBLE_FLAT: // double flat
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
        
        public Bitmap DrawImage(IResultDataObject dataobj, params object[] inprm)
        {
            if (inprm == null)
                throw new ArgumentNullException("AsoDisplay.DrawImage");

            // Arg in Prm expected :  2 - 0 DisplayGrid (bool), 3 - 1 LayerToShow(list of category labels)
            if (inprm.Length != 2)
                throw new ArgumentException("AsoDisplay.DrawImage");

            var asoData = dataobj as DataAso;
            var bLayerDisplayed = new Dictionary<string, bool>();

            float fDisplayFactor = DisplayFactor;
            float fDisplayMinSize = DisplayMinSize; // note de rti à voir si on le passe en param de drawimage

            bool bDisplayGrid = (bool)inprm[0];
            var LayerByCategorylabel = (List<string>)inprm[1];
            if (LayerByCategorylabel == null)
            {
                // Specific case - show all layer
                if (asoData != null)
                    foreach (var cat in asoData.ReportDetailList)
                    {
                        bLayerDisplayed.Add(cat.Label, true);
                    }
            }
            else
            {
                if (asoData != null)
                    foreach (var cat in asoData.ReportDetailList)
                    {
                        bLayerDisplayed.Add(cat.Label, LayerByCategorylabel.Contains(cat.Label));
                    }
            }

            int nMarginpx = _marginpx;
            int nBmpSizepx = NViewSizepx;
            
            Bitmap ViewBmp = null;
            Graphics gImage = null;
            if (asoData == null) return ViewBmp;// A Valider
            bool bIsDieModeUsed = asoData.UseDieGridDisplay;
            if (!bIsDieModeUsed)
                bDisplayGrid = false; //we cancel display grid if theis is not a die to die klarf result

            double lXConversionMicronToPixelAdjusted, lYConversionMicronToPixelAdjusted;
            float Cx, Cy, Ox, Oy;
            
            if (!asoData.IsSquareWafer)
            {
                ViewBmp = new Bitmap(nBmpSizepx, nBmpSizepx);

                gImage = Graphics.FromImage(ViewBmp);
                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, _targetWaferSizepx, _targetWaferSizepx); // pixel margin
                Cx = nMarginpx + WaferRectImage.Width * 0.5F;
                Cy = nMarginpx + WaferRectImage.Height * 0.5F;
                // wafer referential origin => bottom left
                Ox = nMarginpx;
                Oy = nMarginpx + (float)WaferRectImage.Height;

                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / asoData.WaferSizeX_mm / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / asoData.WaferSizeY_mm / 1000;

                float dDiamNotchpx = 2500.0F * (float)lYConversionMicronToPixelAdjusted;
                DrawWaferShape(asoData, ref gImage, nMarginpx, nBmpSizepx, dDiamNotchpx, _fViewPenSizepx, bDisplayGrid);
            }
            else
            {
                nMarginpx = 60;
                int nSizepx = nBmpSizepx - 2 * nMarginpx;

                // on calcul le ratio X/Y
                float RectRatio = asoData.WaferSizeX_mm / asoData.WaferSizeY_mm;
                // le plus grand coté sera de la taille choisi
                bool bXBiggerSize = asoData.WaferSizeX_mm >= asoData.WaferSizeY_mm;
                int nSizepx_X = bXBiggerSize ? nSizepx : ((int)(nSizepx * RectRatio));
                int nSizepx_Y = bXBiggerSize ? ((int)(nSizepx / RectRatio)) : nSizepx;

                ViewBmp = new Bitmap(nSizepx_X + 2 * nMarginpx, nSizepx_Y + 2 * nMarginpx);
                gImage = Graphics.FromImage(ViewBmp);

                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nSizepx_X, nSizepx_Y); // pixel margin
                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / asoData.WaferSizeX_mm / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / asoData.WaferSizeY_mm / 1000;

                Cx = nMarginpx + WaferRectImage.Width * 0.5F;
                Cy = nMarginpx + WaferRectImage.Height * 0.5F;
                // wafer referential origin => bottom left
                Ox = nMarginpx;
                Oy = nMarginpx + nSizepx_Y;

                gImage.FillRectangle(new Pen(Color.FromArgb(1, 1, 1)).Brush, WaferRectImage);
                var MyWhitePen = new Pen(Color.White, 2.0F);
                gImage.DrawRectangle(MyWhitePen, WaferRectImage);
                if (bIsDieModeUsed && bDisplayGrid)
                {
                    // draw DIE GRID
                    double XPos = 0.0;
                    double i = 0.0;
                    double dPitchX = (double)asoData.DiePitchX;
                    double dPitchY = (double)asoData.DiePitchY;
                    var GridPen = new Pen(Color.FromArgb(240, 240, 240), _fViewPenSizepx * 0.5f)
                    {
                        DashStyle = System.Drawing.Drawing2D.DashStyle.Dot //Dash //DashDotDot //DashDot //Dot //Solid
                    };

                    gImage.DrawRectangle(new Pen(Color.FromArgb(1, 255, 1), _fViewPenSizepx * 0.5f),
                      (int)(asoData.DieOriginX * lXConversionMicronToPixelAdjusted + Cx), (int)((-asoData.DieOriginY + (-1) * dPitchY) * lYConversionMicronToPixelAdjusted + (nSizepx_Y + 2 * nMarginpx) - Cy),
                     (int)(dPitchX * lXConversionMicronToPixelAdjusted), (int)(dPitchY * lYConversionMicronToPixelAdjusted));

                    if (dPitchX > 0.0 && dPitchY > 0.0)
                    {
                        while (XPos < WaferRectImage.Width) // X sens positif
                        {
                            XPos = (asoData.DieOriginX + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                            gImage.DrawLine(GridPen, new PointF((float)XPos, nMarginpx), new PointF((float)XPos, nMarginpx + nSizepx_Y));
                            i++;
                        }

                        i = -1.0;
                        XPos = (asoData.DieOriginX + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                        while (XPos >= 0.0) // X sens negatif
                        {
                            XPos = (asoData.DieOriginX + i * dPitchX) * lXConversionMicronToPixelAdjusted + Cx;
                            gImage.DrawLine(GridPen, new PointF((float)XPos, nMarginpx), new PointF((float)XPos, nMarginpx + nSizepx_Y));
                            i--;
                        }

                        double YPos = 0.0; i = 0.0;
                        while (YPos < WaferRectImage.Height) // Y sens positif
                        {
                            YPos = (-asoData.DieOriginY + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + (nSizepx_Y + 2 * nMarginpx) - Cy;
                            gImage.DrawLine(GridPen, new PointF(nMarginpx, (float)YPos), new PointF(nMarginpx + nSizepx_X, (float)YPos));
                            i++;
                        }

                        YPos = WaferRectImage.Height - nMarginpx;
                        i = -1.0;
                        while (YPos >= 0.0) // Y sens negatif
                        {
                            YPos = (-asoData.DieOriginY + (i - 1) * dPitchY) * lYConversionMicronToPixelAdjusted + (nSizepx_Y + 2 * nMarginpx) - Cy;
                            gImage.DrawLine(GridPen, new PointF(nMarginpx, (float)YPos), new PointF(nMarginpx + nSizepx_X, (float)YPos));
                            i--;
                        }
                    }
                }
            }

            if (asoData.DefectViewItemList == null)
            {
                ComputeRcpxItemsList(asoData, lXConversionMicronToPixelAdjusted, lYConversionMicronToPixelAdjusted, Ox, Oy);
            }

            var BinPens = new Dictionary<string, Pen>();
            if (asoData.DefectViewItemList != null && asoData.DefectViewItemList.ItemList != null)
            {
                // Draw List Item
                foreach (var defItem in asoData.DefectViewItemList.ItemList)
                {
                    if (bLayerDisplayed[defItem.DefectCategory])
                    {
                        if (!BinPens.ContainsKey(defItem.DefectCategory))
                        {
                            BinPens.Add(defItem.DefectCategory, new Pen(defItem.Color));
                        }
                        gImage.FillRectangle(BinPens[defItem.DefectCategory].Brush, Rectangle.Round(defItem.ApplyModifiers(fDisplayFactor, fDisplayMinSize)));
                    }
                }
            }

            ViewBmp.MakeTransparent(Color.Black);

            foreach (var pen in BinPens.Values)
                pen.Dispose();
 
            gImage.Dispose();

            return ViewBmp;
        }
        
        private void ComputeRcpxItemsList(DataAso asoData, double lXConversionMicronToPixelAdjusted, double lYConversionMicronToPixelAdjusted, double ox, double oy)
        {
            asoData.DefectViewItemList = new Format.Helper.RcpxItemList<AsoDefect>();

            foreach (var oDef in asoData.ClusterList)
            {
                // compute & store rect without DisplayFactor and DisplayMinSize
                double dSizeX = oDef.MicronSizeX * lXConversionMicronToPixelAdjusted; // width
                double dSizeY = oDef.MicronSizeY * lYConversionMicronToPixelAdjusted; // height

                // Micron position are express in wafer refereential where orgin is bottomleft of wafer, X+ toward right and Y+ toward Top
                double XPos = ox + oDef.MicronPositionX * lXConversionMicronToPixelAdjusted - dSizeX * 0.5; // Left pos
                double YPos = oy - oDef.MicronPositionY * lYConversionMicronToPixelAdjusted - dSizeY * 0.5; // top pos

                asoData.DefectViewItemList.AddItem(oDef.UserLabel, new AsoDefect(oDef, new RectangleF((float)XPos, (float)YPos, (float)dSizeX, (float)dSizeY)));
            }
            asoData.DefectViewItemList.NotifyAddCompleted();
        }
        #endregion

        public List<ResultDataStats> GenerateStatisticsValues(IResultDataObject dataobj, params object[] inprm)
        {
            var asodata = dataobj as DataAso;
            return asodata.GetStats();
        }

        public bool GenerateThumbnailFile(IResultDataObject dataobj, params object[] inprm)
        {
            // no use of inprm

            int nMarginpx = _thumbnailmarginpx;
            int nThumbSizepx = _thumbnailtargetimgsize;
            Graphics gImage = null;
            var asoData = dataobj as DataAso;

            Bitmap ThumbBmp;
            double lXConversionMicronToPixelAdjusted;
            double lYConversionMicronToPixelAdjusted;
            float Ox;
            float Oy;
            if (!asoData.IsSquareWafer)
            {
                ThumbBmp = new Bitmap(nThumbSizepx, nThumbSizepx);
                gImage = Graphics.FromImage(ThumbBmp);
                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nThumbSizepx - 2 * nMarginpx, nThumbSizepx - 2 * nMarginpx); // pixel margin
                // wafer referential origin => bottom left
                Ox = nMarginpx;
                Oy = nMarginpx + (float)WaferRectImage.Height;

                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / asoData.WaferSizeX_mm / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / asoData.WaferSizeY_mm / 1000;

                float dDiamNotchpx = 3.0F;
                DrawWaferShape(asoData, ref gImage, nMarginpx, nThumbSizepx, dDiamNotchpx, 2.0F, false);
            }
            else
            {
                nMarginpx = 3;
                int nSizepx = nThumbSizepx - 2 * nMarginpx;

                // on calcul le ratio X/Y
                float RectRatio = asoData.WaferSizeX_mm / asoData.WaferSizeY_mm;
                // le plus grand coté sera de la taille choisi
                bool bXBiggerSize = asoData.WaferSizeX_mm >= asoData.WaferSizeY_mm;
                int nSizepx_X = bXBiggerSize ? nSizepx : ((int)(nSizepx * RectRatio));
                int nSizepx_Y = bXBiggerSize ? ((int)(nSizepx / RectRatio)) : nSizepx;

                ThumbBmp = new Bitmap(nSizepx_X + 2 * nMarginpx, nSizepx_Y + 2 * nMarginpx);
                gImage = Graphics.FromImage(ThumbBmp);
                gImage.Clear(Color.Black);

                var WaferRectImage = new Rectangle(nMarginpx, nMarginpx, nSizepx_X, nSizepx_Y); // pixel margin
                lXConversionMicronToPixelAdjusted = (double)WaferRectImage.Width / asoData.WaferSizeX_mm / 1000;
                lYConversionMicronToPixelAdjusted = (double)WaferRectImage.Height / asoData.WaferSizeY_mm / 1000;

                // wafer referential origin => bottom left
                Ox = nMarginpx;
                Oy = nMarginpx + nSizepx_Y;

                gImage.FillRectangle(new Pen(Color.FromArgb(1, 1, 1)).Brush, WaferRectImage);
                var MyWhitePen = new Pen(Color.White, 2.0F);
                gImage.DrawRectangle(MyWhitePen, WaferRectImage);
            }

            if (asoData.DefectViewItemList == null)
            {
                ComputeRcpxItemsList(asoData, lXConversionMicronToPixelAdjusted, lYConversionMicronToPixelAdjusted, Ox, Oy);
            }

            var BinPens = new Dictionary<string, Pen>();
            // Draw List Item
            foreach (var defItem in asoData.DefectViewItemList.ItemList)
            {
                if (!BinPens.ContainsKey(defItem.DefectCategory))
                {
                    BinPens.Add(defItem.DefectCategory, new Pen(defItem.Color));
                }
                gImage.FillRectangle(BinPens[defItem.DefectCategory].Brush, defItem.ApplyModifiers(_thumbnaildefaultDisplayFactor, _thumbnaildefaultDisplayMinSize));
            }

            ThumbBmp.MakeTransparent(Color.Black);

            string sThumbFilePath = FormatHelper.ThumbnailPathOf(asoData);
            Directory.CreateDirectory(Path.GetDirectoryName(sThumbFilePath));
            ThumbBmp.Save(sThumbFilePath, ImageFormat.Png);

            foreach (var pen in BinPens.Values)
                pen.Dispose();

            asoData.DefectViewItemList = null;
            return true;
        }

        public Color GetColorCategory(IResultDataObject dataobj, string sCategoryName)
        {
            if (dataobj == null)
                return Color.Transparent;

            var asodata = dataobj as DataAso;
            return asodata.GetColorCategory(sCategoryName);
        }

        public void UpdateInternalDisplaySettingsPrm(params object[] inprm)
        {
            if (inprm == null)
                throw new ArgumentNullException("AsofDisplay.UpdateInternalDisplaySettingsPrm");

            // Arg in Prm expected : 0 DisplayFactor (float), 1 DisplayMinSize (float)
            if (inprm.Length == 0)
                throw new ArgumentException("AsofDisplay.UpdateInternalDisplaySettingsPrm Empty inprm");

            if (inprm.Length > 2)
                throw new ArgumentException("AsofDisplay.UpdateInternalDisplaySettingsPrm too much inprm");

            if (inprm.Length == 2)
            {
                // Arg in Prm expected : 0 DisplayFactor (double), 1 DisplayMinSize (int)
                DisplayFactor = (float)(double)inprm[0];
                DisplayMinSize = (int)inprm[1];
            }
        }
    }
}
