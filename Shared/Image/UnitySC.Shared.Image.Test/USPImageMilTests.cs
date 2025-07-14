using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.Shared.Configuration;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.Image.Test
{
    [TestClass]
    public class USPImageMilTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var container = new Container();
            var configuration = new ServiceConfigurationManager { MilIsSimulated = true, };
            container.Register<IServiceConfigurationManager>(() => configuration);
            ClassLocator.ExternalInit(container, true);
        }
        
        [TestMethod]
        public void ShouldCreateImageMil()
        {
            //Arrange
            byte[,] pixelArray = { 
                { 1, 2, 3}, 
                { 4, 5, 6}, 
                { 7, 8, 9}
            };
            
            var image = new USPImageMil();
            
            //Act
            
            image.PutCSharpArray(pixelArray);
            
            //Assert
            Assert.AreEqual(3, image.GetCSharpImage().width);
            Assert.AreEqual(3, image.GetCSharpImage().height);
            
            Assert.AreEqual(1, image.GetCSharpImage().uint8(0, 0));
            Assert.AreEqual(2, image.GetCSharpImage().uint8(0, 1));
            Assert.AreEqual(3, image.GetCSharpImage().uint8(0, 2));
            Assert.AreEqual(4, image.GetCSharpImage().uint8(1, 0));
            Assert.AreEqual(5, image.GetCSharpImage().uint8(1, 1));
            Assert.AreEqual(6, image.GetCSharpImage().uint8(1, 2));
            Assert.AreEqual(7, image.GetCSharpImage().uint8(2, 0));
            Assert.AreEqual(8, image.GetCSharpImage().uint8(2, 1));
            Assert.AreEqual(9, image.GetCSharpImage().uint8(2, 2));
        }
        
        private class ServiceConfigurationManager : IServiceConfigurationManager
        {
            public string ConfigurationFolderPath { get; }
            public string ConfigurationName { get;  }
            public string LogConfigurationFilePath { get; }
            public bool MilIsSimulated { get; set;}
            public string GetStatus()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
