using System.Windows;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Shared.Helpers
{
    public static class RoiHelpers
    {
        // roiRect in pixels
        static public RegionOfInterest GetRegionOfInterest(Length pixelSize, double scale, Rect roiRect, bool isCenteredRoi)
        {
            RegionOfInterest newRegionOfInterest = null;

            var camera = ClassLocator.Default.GetInstance<CameraBench>();

            var scaledWidth = (long)(camera.Width * scale);
            var scaledHeight = (long)(camera.Height * scale);

            if ((roiRect.Width != scaledWidth) || (roiRect.Height != scaledHeight))
            {
                var roiWidth = roiRect.Width * pixelSize;
                var roiHeight = roiRect.Height * pixelSize;

                Length roiX;
                Length roiY;
                if (isCenteredRoi)
                {
                    roiX = ((camera.Width - roiRect.Width) * pixelSize) / 2;
                    roiY = ((camera.Height - roiRect.Height) * pixelSize) / 2;
                }
                else
                {
                    roiX = roiRect.Left * pixelSize;
                    roiY = roiRect.Top * pixelSize;
                }

                newRegionOfInterest = new RegionOfInterest(roiX, roiY, roiWidth, roiHeight);
            }
            return newRegionOfInterest;
        }

    }
}
