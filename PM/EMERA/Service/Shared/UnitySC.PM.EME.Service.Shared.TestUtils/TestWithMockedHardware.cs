using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.EME.Hardware;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    /// <summary>
    /// <para>This is using the Curiously Recurring Template Pattern (CRTP).</para>
    ///
    /// <para>
    /// A child class that wants to mock some hardware needs to have this class as a parent, and
    /// also implement all relevant hardware mock interfaces. Then, the Init function will generically
    /// initialize all mocks for each hardware interface. The test class can add some mocks setups
    /// using the virtual functions.
    /// </para>
    ///
    /// <para>
    /// The following article breaks it down in a very understandable way: https://zpbappi.com/curiously-recurring-template-pattern-in-csharp/
    /// </para>
    /// </summary>
    ///
    /// <typeparam name="TDerived">Refers to the test class that we are creating (CRTP in aciton)</typeparam>
    ///
    /// <example>
    /// This shows a quick example for a test class that needs mocked lights and axes.
    ///
    /// <code>
    /// [TestClass]
    /// public class MyTestClass : TestWithMockedHardware<MyTestClass>, ITestWithLight, ITestWithAxes
    /// {
    ///     // Common property of both interfaces
    ///     AnaHardwareManager HardwareManager { get; set; }
    ///
    ///     // ITestWithAxes property
    ///     Mock<IAxes> SimulatedAxes { get; set; }
    ///
    ///     // ITestWithLight properties
    ///     string LightId { get; set; }
    ///     Mock<Light> SimulatedLight { get; set; }
    ///
    ///     [TestCase]
    ///     public void MyFirstTest() {
    ///         // This test case automatically has the axes and lights mocked
    ///         // ...
    ///     }
    /// }
    /// </code>
    /// </example>
    [TestClass]
    public class TestWithMockedHardware<TDerived> where TDerived : TestWithMockedHardware<TDerived>
    {
        public EmeHardwareManager HardwareManager { get; set; }
        protected virtual bool FlowsAreSimulated => false;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            // Make sure to allow registration overriding for SpecializeRegister
            container.Options.AllowOverridingRegistrations = true;
            Bootstrapper.Register(container, FlowsAreSimulated);

            SpecializeRegister();

            HardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();

            PreGenericSetup();

            if (this is ITestWithPhotoLumAxes testWithPhotoLumAxes)
                TestWithPhotoLumHelper.Setup(testWithPhotoLumAxes);
            if (this is ITestWithFilterWheel testWithFilterWheel)
                TestWithFilterWheelHelper.Setup(testWithFilterWheel);
            if (this is ITestWithChuck testWithChuck)
                TestWithChuckHelper.Setup(testWithChuck);
            if (this is ITestWithCamera testWithCamera)
                TestWithCameraHelper.Setup(testWithCamera);
            if (this is ITestWithLights testWithLight)
                TestWithLightHelper.Setup(testWithLight);
            if (this is ITestWithDistanceSensor testWithDistanceSensor)
                TestWithDistanceSensorHelper.Setup(testWithDistanceSensor);
            
            PostGenericSetup();
        }

        /// <summary>
        /// Called before the generic setup. May be used to manually set up interfaces members (constants or even mocks).
        /// </summary>
        protected virtual void PreGenericSetup()
        {
            // When setting up interfaces manually, make sure that the different helpers
            // do take it into account and do not override the manually set value.
        }

        /// <summary>
        /// Called after the generic setup. May be used to add mock functions setups (e.g. MyMock.Setup(...))
        /// </summary>
        protected virtual void PostGenericSetup()
        {
        }

        /// <summary>
        /// Needed for tests that need to register new elements to the Bootstrapper, as registering
        /// must be performed before any call to GetInstance.
        /// </summary>
        protected virtual void SpecializeRegister()
        {
        }
    }
}
