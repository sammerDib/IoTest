using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Interface.Test.Algo
{
    [TestClass]
    public class DieIndexTest
    {
        [TestMethod]
        public void Equality_nominal_case()
        {
            DieIndex die = new DieIndex(1, 2);
            DieIndex equalDie = new DieIndex(1, 2);
            Assert.IsTrue(die == equalDie);
        }

        [TestMethod]
        public void Equality_null()
        {
            DieIndex die = null;
            DieIndex equalDie = null;
            Assert.IsTrue(die == equalDie);
        }

        [TestMethod]
        public void Inequality_different_rows()
        {
            DieIndex die = new DieIndex(1, 2);
            DieIndex differentRow = new DieIndex(1, 3);
            Assert.IsTrue(die != differentRow);
        }

        [TestMethod]
        public void Inequality_different_columns()
        {
            DieIndex die = new DieIndex(1, 2);
            DieIndex differentColumns = new DieIndex(3, 2);
            Assert.IsTrue(die != differentColumns);
        }

        [TestMethod]
        public void Inequality_first_null()
        {
            DieIndex die = new DieIndex(1, 2);
            DieIndex nullDie = null;
            Assert.IsTrue(die != nullDie);
        }

        [TestMethod]
        public void Inequality_second_null()
        {
            DieIndex die = new DieIndex(1, 2);
            DieIndex nullDie = null;
            Assert.IsTrue(nullDie != die);
        }
    }
}
