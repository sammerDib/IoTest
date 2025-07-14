using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestImageConverter
    {
        [TestMethod]
        public void Image_convertor_allows_to_convert_between_32FC1_and_8UC1()
        {
            // Given : Grayscale 8-bit images (Gray8)
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var convertedImg = Converter.convertTo32FC1(img);
            var reconvertedImg = Converter.convertTo8UC1(convertedImg);

            Assert.AreEqual(ImageType.GRAYSCALE_Unsigned8bits, img.Type);
            Assert.AreEqual(ImageType.GRAYSCALE_Float32bits, convertedImg.Type);
            Assert.AreEqual(ImageType.GRAYSCALE_Unsigned8bits, reconvertedImg.Type);
        }
    }
}
