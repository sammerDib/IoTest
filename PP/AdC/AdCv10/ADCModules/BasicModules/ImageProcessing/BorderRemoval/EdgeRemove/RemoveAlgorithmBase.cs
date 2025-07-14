using System.Drawing;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    public abstract class RemoveAlgorithmBase
    {
        private const int _step = 40;   // minimum size to divide and recurse

        protected abstract eCompare IsQuadInside(QuadF micronQuad);
        protected abstract bool IsPointInside(PointF micronPoint);

        //=================================================================
        // Paramètres
        //=================================================================
        public bool RemoveInterior;

        /// <summary>
        ///Dessine les zones "removées" dans l'image 
        /// </summary>
        public bool Debug;

        /// <summary>
        // Crée une map dans une autre image. La map contient les zones dedans/dehors/sur le bord.
        /// </summary>
        public bool DrawDebugMap;

        private MilGraphicsContext gc;

        private static object mutex = new object();
        public static MilImage map;    // Pour le debug :
                                       // Crée une map dans une autre image. La map 
                                       // contient les zones dedans/dehors/sur le bord.

        //=================================================================
        // 
        //=================================================================
        public virtual void PerformRemoval(ImageBase image)
        {
            if (DrawDebugMap)
            {
                lock (mutex)
                {
                    if (map == null)
                        CreateMap(image);
                }
            }

            using (gc = new MilGraphicsContext())
            {
                gc.Alloc(Mil.Instance.HostSystem);
                gc.Image = image.CurrentProcessingImage.GetMilImage();
                gc.Color = 0;
                Remove(image, image.imageRect);
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void Remove(ImageBase image, Rectangle rect)
        {
            MatrixBase matrix = image.Layer.Matrix;
            QuadF micronQuad = matrix.pixelToMicron(rect);

            var res = IsQuadInside(micronQuad);
            if (MustBeKept(res))
            {
                if (DrawDebugMap)
                    DrawMap(rect, 80);

                if (Debug)
                    AndRect(image, rect, 255, 0);
                else
                { /*Keep all pixels*/ }
            }
            else if (MustBeRemoved(res))
            {
                if (DrawDebugMap)
                    DrawMap(rect, 160);

                Rectangle r = rect.NegativeOffset(image.imageRect.TopLeft());
                if (Debug)
                    AndRect(image, rect, 255, 80);
                else
                    gc.RectFill(r);
            }
            else if (rect.Width > _step || rect.Height > _step)
            {
                DivideRect(image, rect);
            }
            else
            {
                if (DrawDebugMap)
                    DrawMap(rect, 255);

                RemovePixels(image, rect);
            }
        }


        //=================================================================
        // 
        //=================================================================
        private void DivideRect(ImageBase image, Rectangle rect)
        {
            Rectangle quarter;
            bool bHalfWidth = true;
            bool bHalfHeight = true;

            // TopLeft
            //........
            quarter = rect.TopLeftQuarter();
            if (rect.Width < _step)
            {
                quarter.Width = rect.Width;
                bHalfWidth = false;
            }
            if (rect.Height < _step)
            {
                quarter.Height = rect.Height;
                bHalfHeight = false;
            }
            Remove(image, quarter);

            // TopRight
            //.........
            if (bHalfWidth)
            {
                quarter = rect.TopRightQuarter();
                if (quarter.Height <= _step / 2)
                    quarter.Height = rect.Height;
                Remove(image, quarter);
            }

            // BottomLeft
            //...........
            if (bHalfHeight)
            {
                quarter = rect.BottomLeftQuarter();
                if (quarter.Width <= _step / 2)
                    quarter.Width = rect.Width;
                Remove(image, quarter);
            }

            // BottomRight
            //............
            if (bHalfWidth && bHalfHeight)
            {
                quarter = rect.BottomRightQuarter();
                Remove(image, quarter);
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void RemovePixels(ImageBase image, Rectangle rect)
        {
            MatrixBase matrix = image.Layer.Matrix;

            // Get pixels
            //...........
            byte[,] data = new byte[rect.Height, rect.Width];
            Rectangle subImageRect = rect.NegativeOffset(image.imageRect.TopLeft());
            image.CurrentProcessingImage.GetMilImage().Get2d(subImageRect.ToWinRect(), data);

            // Check if pixels are in the wafer
            //.................................
            Point p = new Point();
            for (p.X = rect.X; p.X < rect.Right; p.X++)
            {
                for (p.Y = rect.Y; p.Y < rect.Bottom; p.Y++)
                {
                    int x = p.X - rect.X;
                    int y = p.Y - rect.Y;

                    if (Debug)
                    {
                        const byte fillcolor = 60;

                        PointF micronPos = matrix.pixelToMicron(p);

                        if (IsPointInside(micronPos) == RemoveInterior)
                        {
                            if (data[y, x] < 255 - fillcolor)
                                data[y, x] += fillcolor;
                        }
                    }
                    else
                    {
                        if (data[y, x] != 0)    // we remove only white pixels :-)
                        {
                            PointF micronPos = matrix.pixelToMicron(p);

                            if (IsPointInside(micronPos) == RemoveInterior)
                                data[y, x] = 0;
                        }
                    }
                }
            }

            // Put back pixels
            //................
            image.CurrentProcessingImage.GetMilImage().Put2d(subImageRect.ToWinRect(), data);
        }

        //=================================================================
        // 
        //=================================================================
        public void AndRect(ImageBase image, Rectangle rect, byte bordercolor, byte fillcolor)
        {
            int w = rect.Width;
            int h = rect.Height;
            Rectangle subImageRect = rect.NegativeOffset(image.imageRect.TopLeft());
            MilImage milImage = image.CurrentProcessingImage.GetMilImage();

            byte[,] data = new byte[h, w];
            milImage.Get2d(subImageRect.ToWinRect(), data);

            if (fillcolor > 0)
            {
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                    {
                        if (data[i, j] < 255 - fillcolor)
                            data[i, j] += fillcolor;
                    }
            }

            // Dessin du pourtour pour debug
            //for (int i = 0; i < h; i++)
            //{
            //    data[i, 0] = bordercolor;
            //    data[i, w - 1] = bordercolor;
            //}
            //for (int j = 0; j < w; j++)
            //{
            //    data[0, j] = bordercolor;
            //    data[h - 1, j] = bordercolor;
            //}

            milImage.Put2d(subImageRect.ToWinRect(), data);
        }

        //=================================================================
        // 
        //=================================================================
        private bool MustBeRemoved(eCompare cmp)
        {
            if (RemoveInterior)
                return cmp == eCompare.QuadIsInside;
            else
                return cmp == eCompare.QuadIsOutside;
        }

        private bool MustBeKept(eCompare cmp)
        {
            if (RemoveInterior)
                return cmp == eCompare.QuadIsOutside;
            else
                return cmp == eCompare.QuadIsInside;
        }

        //=================================================================
        // 
        //=================================================================
        private void CreateMap(ImageBase image)
        {
            map = new MilImage();
            WaferBase wafer = image.Layer.Wafer;

            RectangleF rf = wafer.SurroundingRectangleWithFlats;
            Rectangle r = image.Layer.Matrix.micronToPixel(rf);
            r = r.DividedBy(_step / 2);
            map.Alloc2d(r.Right + 100, r.Bottom + 100, 8 + MIL.M_UNSIGNED, MIL.M_PROC + MIL.M_IMAGE);
            map.Clear();
        }

        //=================================================================
        // 
        //=================================================================
        private void DrawMap(Rectangle rect, byte color)
        {
            Point p = rect.TopLeft().DividedBy(_step / 2);
            map.PutPixel(p.X, p.Y, color);
        }

        //=================================================================
        // 
        //=================================================================
#if DRAW_DEBUG_OLD
        public void DrawWafer(ImageBase image, MilGraphicsContext gc)
        {
			RectangleF r = Wafer.SurroundingRectangleWithFlats;
			DrawArc(image, gc, r);

			r.Inflate(-(float)paramMargin, -(float)paramMargin);
			DrawArc(image, gc, r);
		}

		protected void DrawArc(ImageBase image, MilGraphicsContext gc, RectangleF r)
		{
			MatrixBase matrix = image.layer.matrix;
			Rectangle pixelRect = matrix.micronToPixel(r);
			Point p = pixelRect.Middle();
			Size radius = pixelRect.Size.DividedBy(2);
			gc.Color = 150;
			gc.Arc(p.X, p.Y, radius.Width, radius.Height);
		}
#endif

        //=================================================================
        // 
        //=================================================================
        private void Test(ImageBase image)
        {
            int step = 50;
            for (int i = image.imageRect.Left; i < image.imageRect.Right; i += step)
            {
                for (int j = image.imageRect.Top; j < image.imageRect.Bottom; j += step)
                {
                    Rectangle r = new Rectangle(i, j, step, step);

                    WaferBase wafer = image.Layer.Wafer;
                    MatrixBase matrix = image.Layer.Matrix;
                    QuadF micronQuad = matrix.pixelToMicron(r);

                    var res = IsQuadInside(micronQuad);
                    switch (res)
                    {
                        case eCompare.QuadIsInside:
                            gc.Color = 255;
                            break;
                        case eCompare.QuadIsOutside:
                            gc.Color = 80;
                            break;
                        case eCompare.QuadIntersects:
                            gc.Color = 150;
                            break;
                    }
                    gc.RectFill(r);
                }
            }
        }

    }
}
