using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Test.Recipe.Measure
{
    [TestClass]
    public class PositionBaseExtensionsTest
    {
        [TestMethod]
        public void PositionInReferential_waferReferential_with_same_referential_ok()
        {
            PositionBase position = new XYPosition(new WaferReferential());
            PositionBase res = position.PositionInReferential(new WaferReferential());
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void PositionInReferential_dieReferential_with_other_dieReferential_ok()
        {
            PositionBase position = new XYPosition(new DieReferential(1, 2));
            PositionBase res = position.PositionInReferential(new DieReferential(2, 3));
            Assert.IsNotNull(res);
            Assert.AreEqual(new DieReferential(2, 3), res.Referential);
        }

        [TestMethod]
        public void PositionInReferential_dieReferential_with_waferReferential_throws()
        {
            PositionBase position = new XYPosition(new DieReferential(1, 2));
            Action functionCall = () => position.PositionInReferential(new WaferReferential());
            Assert.ThrowsException<ArgumentException>(functionCall);
        }

        [TestMethod]
        public void PositionInReferential_waferReferential_with_dieReferential_throws()
        {
            PositionBase position = new XYPosition(new WaferReferential());
            Action functionCall = () => position.PositionInReferential(new DieReferential(1, 2));
            Assert.ThrowsException<ArgumentException>(functionCall);
        }
    }
}
