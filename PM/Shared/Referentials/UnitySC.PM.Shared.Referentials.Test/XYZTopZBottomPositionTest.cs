using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Referentials.Test
{
    [TestClass]
    public class XYZTopZBottomPositionTest
    {
        [TestMethod]
        public void Expect_XYZTopZBottomPosition_to_be_creatable()
        {
            _ = new XYZTopZBottomPosition(new MotorReferential(), 0, 0, 0, 0);
        }

        [TestMethod]
        public void Expect_same_location_data_to_be_equal()
        {
            var left = new XYZTopZBottomPosition(new MotorReferential(), 1, 1, 1, 1);
            var right = new XYZTopZBottomPosition(new MotorReferential(), 1, 1, 1, 1);

            Assert.AreEqual(left, right);
            Assert.IsTrue(left == right);
            Assert.IsFalse(left != right);
        }

        [TestMethod]
        public void Expect_equality_test_to_manage_null()
        {
            var left = new XYZTopZBottomPosition(new MotorReferential(), 1, 1, 1, 1);
            XYZTopZBottomPosition nullLocation = null;

            Assert.AreNotEqual(left, nullLocation);
            Assert.AreNotEqual(nullLocation, left);
            Assert.IsFalse(left == nullLocation);
            Assert.IsTrue(left != nullLocation);
        }

        [TestMethod]
        public void Expect_same_coordinates_on_different_referential_to_be_different()
        {
            var left = new XYZTopZBottomPosition(new MotorReferential(), 1, 1, 1, 1);
            var right = new XYZTopZBottomPosition(new WaferReferential(), 1, 1, 1, 1);

            Assert.AreNotEqual(left, right);
            Assert.AreNotEqual(right, left);
            Assert.IsFalse(left == right);
            Assert.IsTrue(left != right);
        }

        [TestMethod]
        public void Expect_toString_to_be_correct()
        {
            var position = new XYZTopZBottomPosition(new MotorReferential(), 1, 1, 1, 1);
            string expectedStringPosition = new StringBuilder()
                .AppendLine("\tReferential = Motor")
                .AppendLine("\tX = 1")
                .AppendLine("\tY = 1")
                .AppendLine("\tZTop = 1")
                .AppendLine("\tZBottom = 1")
                .ToString();

            Assert.AreEqual(expectedStringPosition, position.ToString());
        }
    }
}
