using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

namespace UnitySC.Shared.Tools.Test
{
    [TestClass]
    public class ClassLocatorTest
    {
        [TestInitialize]
        public void ResetContainer()
        {
            ClassLocator.ExternalInit(new Container(), true);
        }

        [TestMethod]
        public void Test_Locating_Same_Instance_For_Multiple_Interfaces()
        {
            ClassLocator.Default.Register(typeof(TestAbstract),
                () => TestClass.Instanciate(), Lifestyle.Singleton);
            ClassLocator.Default.Register(typeof(TestInterface),
                () => ClassLocator.Default.GetInstance<TestAbstract>(), Lifestyle.Singleton);

            Assert.AreSame<TestClass>(ClassLocator.Default.GetInstance<TestAbstract>() as TestClass,
                                      ClassLocator.Default.GetInstance<TestInterface>() as TestClass, 
                                      "Same_Instance_For_Multiple_Interfaces fail");
        }

        [TestMethod]
        public void Test_Locating_Multiplie_Instances_For_One_Interface()
        {
            ClassLocator.Default.AppendToCollection<TestAbstract>(
                () => TestClass.Instanciate(), Lifestyle.Singleton);
            ClassLocator.Default.AppendToCollection<TestAbstract>(
                () => TestClass.Instanciate(), Lifestyle.Singleton);

            var collection = ClassLocator.Default.GetCollection<TestAbstract>();
            Assert.IsTrue(collection.Count() == 2);
            Assert.IsInstanceOfType(collection.First(), typeof(TestClass), "");
        }
    }

    public class TestClass : TestAbstract, TestInterface
    {
        public static TestClass Instanciate()
        { return new TestClass(); }
    }

    public abstract class TestAbstract
    { }

    public interface TestInterface
    { }
}
