using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Referentials.Test
{
    [TestClass]
    public class XYZTopZBottomMoveTest
    {
        [TestMethod]
        public void Expect_same_move_data_to_be_equal()
        {
            var left = new XYZTopZBottomMove(1, 1, 1, 1);
            var right = new XYZTopZBottomMove(1, 1, 1, 1);

            Assert.AreEqual(left, right);
            Assert.IsTrue(left == right);
            Assert.IsFalse(left != right);
        }

        [TestMethod]
        public void Expect_equality_test_to_manage_null()
        {
            var left = new XYZTopZBottomMove(1, 1, 1, 1);
            XYZTopZBottomMove nullMovement = null;

            Assert.AreNotEqual(left, nullMovement);
            Assert.AreNotEqual(nullMovement, left);
            Assert.IsFalse(left == nullMovement);
            Assert.IsTrue(left != nullMovement);
        }
    }
}
