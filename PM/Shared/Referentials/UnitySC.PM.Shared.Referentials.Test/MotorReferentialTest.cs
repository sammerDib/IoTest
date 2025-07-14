using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Referentials.Test
{
    [TestClass]
    public class MotorReferentialTest
    {
        [TestMethod]
        public void Expect_GetHashCode_to_be_always_the_same()
        {
            var ref1 = new MotorReferential();
            var ref2 = new MotorReferential();

            Assert.AreEqual(ref1.GetHashCode(), ref2.GetHashCode());
        }
    }
}
