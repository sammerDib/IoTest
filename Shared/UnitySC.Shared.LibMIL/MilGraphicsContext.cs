using System;
using System.Drawing;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    public class MilGraphicsContextList : AMilId
    {
        public int NumberOfGraphics { get { return (int)MIL.MgraInquireList(MilId, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_NUMBER_OF_GRAPHICS, MIL.M_NULL); } }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MgraFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Allocate a Graphic Context
        //=================================================================
        public void Alloc(MIL_ID systemId)
        {
#if DEBUG
            MIL.MgraAllocList(systemId, MIL.M_DEFAULT, ref _milId);
            AMilId.checkMilError("Failed to allocate Graphic context list");
#endif
        }
    }

    public class MilGraphicsContext : AMilId
    {
        //=================================================================
        // Properties
        //=================================================================
        public double Color
        {
            get
            {
                double color = 0.0;
                MIL.MgraInquire(MilId, MIL.M_COLOR, ref color);
                return color;
            }
            set { MIL.MgraColor(MilId, value); }
        }

        private MilImage _milImage;
        public MilImage Image { get => _milImage; set { _milImage = value; } }
        private MilGraphicsContextList _gcList;
        public MilGraphicsContextList GCList { get => _gcList; set { _milImage = null; _gcList = value; } }
        private MIL_ID _destID => _milImage == null ? _gcList.MilId : _milImage.MilId;

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_milId != MIL.M_NULL)
                {
                    MIL.MgraFree(_milId);
                    _milId = MIL.M_NULL;
                }

                base.Dispose(disposing);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Dispose MilGraphicsContext : {e.Message}");
                Console.WriteLine($"Dispose MilGraphicsContext : {e.StackTrace}");

                System.Diagnostics.Debugger.Break();
            }
        }

        //=================================================================
        // Allocate a Graphic Context
        //=================================================================
        public void Alloc(MIL_ID systemId)
        {
            MIL.MgraAlloc(systemId, ref _milId);
            AMilId.checkMilError("Failed to allocate Graphic context");
        }

        public void Alloc()
        {
            Alloc(Mil.Instance.HostSystem);
        }

        //=================================================================
        // Draw a point
        //=================================================================
        public void Dot(int xPos, int yPos)
        {
            MIL.MgraDot(_milId, _destID, xPos, yPos);
            AMilId.checkMilError("Failed to draw dot");
        }

        public void Dot(Point point)
        {
            Dot(point.X, point.Y);
        }

        //=================================================================
        // Draw a line
        //=================================================================
        public void Line(int xStart, int yStart, int xEnd, int yEnd)
        {
            MIL.MgraLine(_milId, _destID, xStart, yStart, xEnd, yEnd);
            AMilId.checkMilError("Failed to draw line");
        }

        //=================================================================
        // Draw a cross
        //=================================================================
        public void Cross(Point point, int size)
        {
            Cross(point.X, point.Y, size);
        }

        public void Cross(int xPos, int yPos, int size)
        {
            Line(xPos - size, yPos, xPos + size, yPos);
            Line(xPos, yPos - size, xPos, yPos + size);
        }

        //=================================================================
        // Draw a rectangle
        //=================================================================
        public void Rect(int xStart, int yStart, int xEnd, int yEnd)
        {
            MIL.MgraRect(_milId, _destID, xStart, yStart, xEnd, yEnd);
            AMilId.checkMilError("Failed to draw rectangle");
        }

        public void AddText(int posX, int posY, string text)
        {
            MIL.MgraText(_milId, _destID, posX, posY, text);
        }

        public void Rect(Rectangle rect)
        {
            Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public void ThickRect(Rectangle rect, int thickness)
        {
            for (int i = 0; i < thickness; i++)
            {
                Rect(rect);
                rect.Inflate(+1, +1);
            }
        }

        public void RectFill(int xStart, int yStart, int xEnd, int yEnd)
        {
            MIL.MgraRectFill(_milId, _destID, xStart, yStart, xEnd, yEnd);
            AMilId.checkMilError("Failed to fill rectangle");
        }

        public void RectFill(Rectangle rect)
        {
            RectFill(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        //=================================================================
        // Draw/Fill an arc
        //=================================================================
        public void ArcFill(double xCenter, double yCenter, double xRad, double yRad, double startAngle = 0, double endAngle = 360)
        {
            MIL.MgraArcFill(_milId, _destID, xCenter, yCenter, xRad, yRad, startAngle, endAngle);
            AMilId.checkMilError("Failed to fill arc");
        }

        public void Arc(double xCenter, double yCenter, double xRad, double yRad, double startAngle = 0, double endAngle = 360)
        {
            MIL.MgraArc(_milId, _destID, xCenter, yCenter, xRad, yRad, startAngle, endAngle);
            AMilId.checkMilError("Failed to fill arc");
        }

        //=================================================================
        // Clear
        //=================================================================
        public void Clear()
        {
            try
            {
                MIL.MgraClear(MilId, _destID);
                AMilId.checkMilError("Failed to clear graphic context");
            }
            catch (ApplicationException)
            {
                System.Diagnostics.Debugger.Break();
                //TODO does this happen with MIL-10 ?
                // When using a large image on a (rather) slow PC, a "Transfer Error"
                // exception is raised by Clear(). Anyway the image is cleared.
                // It looks like a MIL internal problem, so we ignore this exception.
            }
        }

        //=================================================================
        // Draw a string into MilImage
        //=================================================================
        public void DrawString(double x, double y, string text)
        {
            MIL.MgraText(_milId, _destID, x, y, text);
            checkMilError("Failed to draw string");
        }
    }
}
