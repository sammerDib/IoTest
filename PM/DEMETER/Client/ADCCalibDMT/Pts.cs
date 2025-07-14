using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace ADCCalibDMT
{
    // Class for Mire Pts
    public class Pts
    {

        static public Rectangle g_rcImage; // limit zone image

        public int nxx;   // X coord - Pts Localization in Wafer Grid where Origin is the center of the wafer
        public int nyy;   // Y coord - Pts Localization in Wafer Grid where Origin is the center of the wafer

        public double dPosX_um; // X coord - Theorical Physical Pts position in Wafer (µm - origin Center of wafer) 
        public double dPosY_um; // Y coord - Theorical Physical Pts position in Wafer (µm - origin Center of wafer) 

        public double dPosX_px; // X coord - Pts position in image (pixel - origin TopLeft of image)
        public double dPosY_px; // Y coord - Pts position in image (pixel - origin TopLeft of image)

        public int nPatternID; // pattern ID loaded used to find mire points
 
        public bool bUsedGridPts = false;   // @true if this point is in grid and associate with a true physical wafer points   
        public bool bFound = false;         // @true if this point has been found in image

        public Rectangle SearchAreaInImage;

        public Pts()
        {
            bUsedGridPts = false;
            bFound = false;
        }

        public void SetSearchArea(int X_px, int Y_px, int nSize)
        {
            SearchAreaInImage = Rectangle.Intersect(Pts.g_rcImage, new Rectangle(X_px - nSize, Y_px - nSize, nSize * 2 + 1, nSize * 2 + 1));
        }

        public bool IsGoodToCalibrate()
        {
            return bUsedGridPts & bFound;
        }
    }
}
