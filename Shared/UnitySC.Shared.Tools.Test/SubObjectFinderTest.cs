using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitySC.Shared.Tools.Test
{
    [TestClass]
    public class SubObjectFinderTest
    {
        internal class TestClass
        {
            public string Name { get; set; }
            public List<SubClassTest> SubClassesA { get; set; }
            public List<SubClassTest> SubClassesB { get; set; }
            public ClassToFind ToFind { get; set; }
        }

        internal class SubClassTest
        {
            public string Test { get; set; }
            public SubSubClassTest SubSubClass { get; set; }
            public ClassToFind ToFind { get; set; }
        }

        internal class SubSubClassTest
        {
            public ClassToFind ToFind { get; set; }
        }

        internal class ClassToFind
        {
            public string Test { get; set; }
        }

        private static TestClass InitTestClass()
        {
            var testClass = new TestClass();
            testClass.Name = "Test";
            testClass.SubClassesA = new List<SubClassTest>()
            {
                new SubClassTest(){Test  ="ASubTest1" , ToFind = new ClassToFind(){Test = "ASubToFind1" }, SubSubClass = new SubSubClassTest(){ ToFind = new ClassToFind() { Test = "ASubSubToFind1" } } },
                new SubClassTest(){Test  ="ASubTest2" , ToFind = new ClassToFind(){Test = "ASubToFind2" }, SubSubClass = new SubSubClassTest(){ ToFind = new ClassToFind() { Test = "ASubSubToFind2" } } },
            };
            testClass.SubClassesB = new List<SubClassTest>()
            {
                new SubClassTest(){Test  ="BSubTest1" , ToFind = new ClassToFind(){Test = "BSubToFind1" }, SubSubClass = new SubSubClassTest(){ ToFind = new ClassToFind() { Test = "BSubSubToFind1" } } },
                new SubClassTest(){Test  ="BSubTest2" , ToFind = new ClassToFind(){Test = "BSubToFind2" }, SubSubClass = new SubSubClassTest(){ ToFind = new ClassToFind() { Test = "BSubSubToFind2" } } },
            };
            testClass.ToFind = new ClassToFind() { Test = "ToFind" };
            return testClass;
        }

        [TestMethod]
        public void FindSuBobjectInComplexObjectTest()
        {
            var testClass = InitTestClass();
            var res = SubObjectFinder.GetAllSubObjectOfTypeT<ClassToFind>(testClass);
            Assert.IsTrue(res.Count() == 9);
        }

        [TestMethod]
        public void FindSubObjectWithMaxDepthTest()
        {
            var testClass = InitTestClass();
            var res = SubObjectFinder.GetAllSubObjectOfTypeT<ClassToFind>(testClass, 1);
            Assert.IsTrue(res.Count() == 1);
        }

        [TestMethod]
        public void FindBasicTypeErrorTest()
        {
            var testClass = InitTestClass();
            try
            {
                var res = SubObjectFinder.GetAllSubObjectOfTypeT<string>(testClass);
                Assert.Fail();
            }
            catch
            {
            }
        }
    }
}
