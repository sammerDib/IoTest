using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Service.Core.Recipe.AcquisitionPath;
using UnitySC.PM.EME.Service.Core.Recipe.Save;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class AcquisitionImageResultBuilderTests
    {
        [TestMethod]
        public void ShouldCreateAcquisitionImageResult()
        {
            //Arrange
            var acquisitionPath = new SerpentinePath(0, 0, 0, 8, 7);
            var imageFolderAndBaseName = new ImageFolderAndBaseName("C:/", "test.tiff");
            var filter = EMEFilter.NoFilter;
            var type = EMELightType.DirectionalDarkField0Degree;
            
            //Act
            var result = new AcquisitionImageResultBuilder()
                .AddFolderAndBaseName(imageFolderAndBaseName)
                .AddFilter(filter, 2.0175.Micrometers(), false, 1)
                .AddLightType(type)
                .AddWaferDiameter(200.Millimeters())
                .BuildVignetteImageResult(acquisitionPath.NbImagesY, acquisitionPath.NbImagesX);

            //Assert
            Assert.AreEqual(acquisitionPath.NbImagesX, result.NbColumns);
            Assert.AreEqual(acquisitionPath.NbImagesY, result.NbLines);

            Assert.AreEqual(imageFolderAndBaseName.Folder, result.FolderName);
            Assert.AreEqual(imageFolderAndBaseName.BaseName, result.BaseName);

            Assert.AreEqual(ResultType.EME_Visible0, result.ResultType);
            
            Assert.AreEqual(2.0175.Micrometers(), result.PixelSize);
            Assert.AreEqual(49566, result.WaferCenter);
        }
    }
}
