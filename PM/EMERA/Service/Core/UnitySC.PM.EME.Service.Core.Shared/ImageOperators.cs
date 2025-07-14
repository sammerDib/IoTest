using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public class ImageOperators
    {
        public FocusMeasureMethod MeasureMethod;

        public ImageOperators(FocusMeasureMethod measureMethod = FocusMeasureMethod.TenenbaumGradient)
        {
            MeasureMethod = measureMethod;
        }

        public virtual double ComputeFocusMeasure(ServiceImage img)
        {
            var imgData = AlgorithmLibraryUtils.CreateImageData(img);

            return UnitySCSharedAlgosOpenCVWrapper.ImageOperators.FocusMeasurement(imgData, MeasureMethod);
        }

        public virtual double ComputeFocusMeasure(USPImage img)
        {
            var imgData = AlgorithmLibraryUtils.CreateImageData(img);

            return UnitySCSharedAlgosOpenCVWrapper.ImageOperators.FocusMeasurement(imgData, MeasureMethod);
        }

        public virtual double ComputeFocusMeasure(USPImageMil img)
        {
            var imgData = AlgorithmLibraryUtils.CreateImageData(img);

            return UnitySCSharedAlgosOpenCVWrapper.ImageOperators.FocusMeasurement(imgData, MeasureMethod);
        }
    }
}
