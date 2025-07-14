using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    public class EdgeDetection
    {
        public virtual ServiceImage Compute(ServiceImage refImg, double gamma, Interface.Algo.RegionOfInterest regionOfInterest, ImageParameters imgParameters, bool removeNoise)
        {
            var refImgData = AlgorithmLibraryUtils.CreateImageData(refImg);
            var roi = AlgorithmLibraryUtils.CreateRegionOfInterest(refImg, regionOfInterest, imgParameters);

            var edgeImg = EdgeDetector.edgeDetection(refImgData, gamma, roi, BlurFilterMethod.Shen, removeNoise);
            return AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(edgeImg);
        }
    }
}
