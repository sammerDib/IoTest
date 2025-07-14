using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.LibMIL;

namespace UnitySC.PM.DMT.Service.Shared.TestUtils
{
    [TestClass]
    [TestCategory("RequiresMIL")]
    public class TestWithMockedCameraAndScreenRequiringMIL<TDerived> : TestWithMockedCameraAndScreen<TDerived> where TDerived : TestWithMockedCameraAndScreenRequiringMIL<TDerived>
    {

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void MilInit(TestContext context)
        {
            Mil.Instance.Allocate();
        }

        [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void MilFree()
        {
            Mil.Instance.Free();
        }
    }
}
