using System;
using System.Drawing;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.ImageViewer
{
    public interface IImgViewerStatus
    {
        float XBmpPixel {get; set;}
        float YBmpPixel {get; set;}
        int XMouse {get; set;}
        int YMouse {get; set;}
        Color BmpPixelColor { get; set; }

        void DisplayCoordStatus();
        void DisplayTextStatus(string sMessage);

    }
}
