using System.Windows;

using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Shared.Helpers
{
    public static class PatternRecHelpers
    {
        public static PositionWithPatternRec CreatePositionWithPatternRec(ServiceImage patternReference, Length pixelSize, double scale, Rect roiRect, bool isCenteredROI)
        {
            pixelSize /= scale;
            roiRect.X = (long)(roiRect.X * scale);
            roiRect.Y = (long)(roiRect.Y * scale);
            roiRect.Width = (long)(roiRect.Width * scale);
            roiRect.Height = (long)(roiRect.Height * scale);

            var newPatternRecImage = new PositionWithPatternRec();

            newPatternRecImage.PatternRec = CreatePatternRec(patternReference, pixelSize, scale, roiRect, isCenteredROI);

            newPatternRecImage.Position = ClassLocator.Default.GetInstance<EmeraMotionAxesSupervisor>().GetCurrentPosition().Result as XYZPosition;
            return newPatternRecImage;
        }

        public static PatternRecognitionData CreatePatternRec(ServiceImage patternReference, Length pixelSize, double scale, Rect roiRect, bool isCenteredROI)
        {
            var newPatternRec = new PatternRecognitionData();

            newPatternRec.PatternReference = patternReference.ToExternalImage();
            newPatternRec.RegionOfInterest = RoiHelpers.GetRegionOfInterest(pixelSize, scale, roiRect, isCenteredROI);
            newPatternRec.Gamma = 0.3;

            return newPatternRec;

        }
    }
}
