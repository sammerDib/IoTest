using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;



namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class DeflectometryCalculations
    {
        public static PSDResult ComputePhaseMap(List<ServiceImage> images, int fringeImageNumber, FringesDisplacement fringesDisplacement)
        {
            var psdParameters = new PSDParams(fringeImageNumber, fringesDisplacement);
            var imageDatas = images.Select(image => new ImageData(image.Data, image.DataWidth, image.DataHeight, ImageType.GRAYSCALE_Unsigned8bits))
                .ToArray();
            return PhaseShiftingDeflectometry.ComputePhaseMap(imageDatas, psdParameters);
        }

        public static ImageData ComputeBaseDark(ImageData darkXImage, ImageData darkYImage, ImageData mask)
        {
            return PhaseShiftingDeflectometry.ComputeDark(darkXImage, darkYImage, mask, FitSurface.PolynomeOrder2);
        }

        public static ImageData ApplyDarkDynamicsCoefficient(ImageData darkImage, ImageData mask, float darkDynamicsCoefficient = 2, float percentageOfLowSaturation = 0.03f)
        {
            return PhaseShiftingDeflectometry.ApplyDynamicCoefficient(darkImage, mask, darkDynamicsCoefficient, percentageOfLowSaturation);
        }

        public static USPImageMil ComputeDark(ImageData darkXImage, ImageData darkYImage, ImageData mask, float darkDynamicsCoefficient = 2, float percentageOfLowSaturation = 0.03f)
        {
            var dark = PhaseShiftingDeflectometry.ComputeDark(darkXImage, darkYImage, mask, FitSurface.PolynomeOrder2);
            dark = PhaseShiftingDeflectometry.ApplyDynamicCoefficient(dark, mask, darkDynamicsCoefficient, percentageOfLowSaturation);
            return dark.ConvertToUSPImageMil();
        }

        public static ImageData ComputeCurvatureMap(PSDResult result, ImageData mask, int fringeImageNumber, FringesDisplacement fringesDisplacement)
        {
            var psdParameters = new PSDParams(fringeImageNumber, fringesDisplacement);
            return PhaseShiftingDeflectometry.ComputeCurvature(result, mask, psdParameters);
        }

        public static ImageData MultiPeriodUnwrap(List<PSDResult> psdResults, ImageData mask, List<int> periods)
        {
            List<ImageData> wrappedPhaseMaps = psdResults.Select(result => result.WrappedPhaseMap).ToList();
            return PhaseShiftingDeflectometry.MultiperiodUnwrap(wrappedPhaseMaps.ToArray(), mask, periods.ToArray(), periods.Count());
        }

        public static ImageData SubstractPlaneFromUnwrapped(ImageData unwrappedPhaseMap, ImageData mask)
        {
            return PhaseShiftingDeflectometry.SubstractPlaneFromUnwrapped(unwrappedPhaseMap, mask);
        }
    }
}
