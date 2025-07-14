using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitySC.Shared.Image.Test
{
    [TestClass]
    public class CSharpImageTests
    {
        private readonly byte[,] _data = { { 1, 2, 3}, 
            { 4, 5, 6},
            { 7, 8, 9}};
        
        [TestMethod]
        public void ShouldCreateImageFrom2dPixelArray()
        {
            //Arrange
            //Act
            var image = new CSharpImage(_data, 8);
            
            //Assert
            Assert.AreEqual(3, image.width);
            Assert.AreEqual(3, image.height);
            CollectionAssert.AreEqual(_data, image.GetDataAsByteArray());
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenGetDataOnImageIs16bitsDepth()
        {
            //Arrange
            var image = new CSharpImage(_data, 8) { depth = 16 };

            //Act
            //Assert
            Assert.ThrowsException<ApplicationException>(() => image.GetDataAsByteArray());
        }
        
        [TestMethod]
        public void ShouldThrowExceptionWhenGetDataOnImageWithInvalidPointer()
        {
            //Arrange
            var image = new CSharpImage(_data, 8);
            image.ptr = 0;
            
            //Act
            //Assert
            Assert.ThrowsException<ApplicationException>(() => image.GetDataAsByteArray());
        }
        
        [DataTestMethod]
        [DataRow(1,1, 8)]
        [DataRow(1,2, 8)]
        [DataRow(1,3, 8)]
        [DataRow(1,125, 128)]
        [DataRow(2,1, 8)]
        [DataRow(2,2, 8)]
        [DataRow(2,3, 8)]
        [DataRow(2,125, 256)]
        [DataRow(4,1, 8)]
        [DataRow(4,2, 8)]
        [DataRow(4,3, 16)]
        [DataRow(4,125, 504)]
        public void ShouldGetPitch(int numberOfBytesPerPixels,int width, int expected)
        {
            //Arrange
            //Act
            int result = CSharpImage.GetPitch(width, numberOfBytesPerPixels);
            
            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
