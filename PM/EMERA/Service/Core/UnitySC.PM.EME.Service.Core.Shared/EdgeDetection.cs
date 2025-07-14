using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public class EdgeDetection
    {
        public virtual ServiceImage Compute(ServiceImage refImg, double gamma, Interface.Algo.RegionOfInterest regionOfInterest, Length pixelSize, bool removeNoise)
        {
            var refImgData = AlgorithmLibraryUtils.CreateImageData(refImg);
            var roi = AlgorithmLibraryUtils.CreateRegionOfInterest(refImg, regionOfInterest, pixelSize);

            var edgeImg = EdgeDetector.edgeDetection(refImgData, gamma, roi, BlurFilterMethod.Shen, removeNoise);
            return AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(edgeImg);
        }
    }
}
