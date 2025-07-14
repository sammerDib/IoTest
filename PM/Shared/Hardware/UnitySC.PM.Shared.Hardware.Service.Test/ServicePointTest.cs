using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Service.Test
{
    [TestClass]
    public class ServicePointTest
    {
        [TestMethod]
        public void Expect_coordinate_equality_to_work()
        {
            var left = new ServicePoint(1, 1);
            var right = new ServicePoint(1, 1);

            Assert.IsTrue(left.Equals(right), $"Content equality should be true");
            Assert.AreEqual(left, right, $"Content equality should be true");
            Assert.IsFalse(left == right, $"Reference equality should be false");
        }
    }
}
