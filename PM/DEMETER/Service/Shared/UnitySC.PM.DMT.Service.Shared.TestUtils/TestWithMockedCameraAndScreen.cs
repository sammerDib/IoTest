using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Implementation;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using Container = SimpleInjector.Container;

namespace UnitySC.PM.DMT.Service.Shared.TestUtils
{
    [TestClass]
    public class TestWithMockedCameraAndScreen<TDerived> where TDerived : TestWithMockedCameraAndScreen<TDerived>
    {
        protected Mock<DMTCameraManager> SimulatedCameraManager;
        protected DMTHardwareManager HardwareManager { get; set; }
        protected string CameraFrontId { get; set; }
        protected string CameraBackId { get; set; }
        protected string ScreenFrontId { get; set; }
        protected string ScreenBackId { get; set; }

        protected Mock<CameraBase> SimulatedCameraFront { get; set; }
        protected Mock<CameraBase> SimulatedCameraBack { get; set; }

        protected Mock<ScreenBase> SimulatedScreenFront { get; set; }
        protected Mock<ScreenBase> SimulatedScreenBack { get; set; }

        protected virtual bool FlowsAreSimulated => false;

        protected virtual bool MilIsSimulated => true;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;
            
            SpecializeRegister(container);
            Bootstrapper.Register(container, FlowsAreSimulated, MilIsSimulated);
            
            HardwareManager = ClassLocator.Default.GetInstance<DMTHardwareManager>();
            
            // Needed yto make sure Bootstrapper.SimulatedCameraManager is initialized
            ClassLocator.Default.GetInstance<DMTCameraManager>();
            SimulatedCameraManager = Bootstrapper.SimulatedCameraManager;

            ClassLocator.Default.GetInstance<AlgorithmManager>().Init();

            PreGenericSetup();

            Setup();

            PostGenericSetup();
        }

        private void Setup()
        {
            CameraFrontId = "FSCamera";
            CameraBackId = "BSCamera";
            ScreenFrontId = "FSScreen";
            ScreenBackId = "BSScreen";

            var globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var logger = ClassLocator.Default.GetInstance<ILogger>();

            // Setup mocked camera
            SimulatedCameraFront =
                new Mock<CameraBase>(
                    new CameraConfigBase { Name = CameraFrontId, DeviceID = CameraFrontId, IsMainCamera = true },
                    globalStatus, logger);
            SimulatedCameraBack = new Mock<CameraBase>(
                new CameraConfigBase { Name = CameraFrontId, DeviceID = CameraFrontId },
                globalStatus, logger);
            SimulatedCameraFront.Setup(camera => camera.IsAcquiring).Returns(true);
            SimulatedCameraFront.Setup(camera => camera.GetFrameRate()).Returns(6);
            SimulatedCameraFront.Setup(camera => camera.GetExposureTimeMs()).Returns(1000.0 / 6);
            SimulatedCameraBack.Setup(camera => camera.IsAcquiring).Returns(true);
            SimulatedCameraBack.Setup(camera => camera.GetFrameRate()).Returns(6);
            SimulatedCameraBack.Setup(camera => camera.GetExposureTimeMs()).Returns(1000.0 / 6);
            HardwareManager.CamerasBySide.Add(Side.Front, SimulatedCameraFront.Object);
            HardwareManager.CamerasBySide.Add(Side.Back, SimulatedCameraBack.Object);
            HardwareManager.Cameras.Add(CameraFrontId, SimulatedCameraFront.Object);
            HardwareManager.Cameras.Add(CameraBackId, SimulatedCameraBack.Object);

            // Setup mocked screens
            SimulatedScreenFront = new Mock<ScreenBase>();
            SimulatedScreenBack = new Mock<ScreenBase>();
            HardwareManager.ScreensBySide.Add(Side.Front, SimulatedScreenFront.Object);
            HardwareManager.ScreensBySide.Add(Side.Back, SimulatedScreenBack.Object);
            HardwareManager.Screens.Add(ScreenFrontId, SimulatedScreenFront.Object);
            HardwareManager.Screens.Add(ScreenBackId, SimulatedScreenBack.Object);
        }

        protected MockSequence SetupCameraWithImages(Side side, List<USPImageMil> images)
        {
            var camera = GetCameraForSide(side);

            var mockSequence = new MockSequence();
            images.ForEach(img => SimulatedCameraManager.InSequence(mockSequence)
                .Setup(cameraManager =>
                    cameraManager.GetLastCameraImage(It.Is<CameraBase>(cameraParam => camera == cameraParam)))
                .Returns(img));
            return mockSequence;
        }

        private CameraBase GetCameraForSide(Side side)
        {
            switch (side)
            {
                case Side.Front:
                    return SimulatedCameraFront.Object;

                case Side.Back:
                    return SimulatedCameraBack.Object;

                default:
                    throw new InvalidEnumArgumentException(nameof(side), (int)side, typeof(Side));
            }
        }

        protected void SetupCameraWithSameImageContinuously(Side side, DummyUSPImageMil image)
        {
            var camera = GetCameraForSide(side);
            Bootstrapper.SimulatedCameraManager.Setup(cameraManager =>
                    cameraManager.GetNextCameraImage(It.Is<CameraBase>(cameraParam => camera == cameraParam)))
                .Returns(image);
            Bootstrapper.SimulatedCameraManager.Setup(cameraManager =>
                    cameraManager.GetLastCameraImage(It.Is<CameraBase>(cameraParam => camera == cameraParam)))
                .Returns(image);
        }

        protected virtual void PreGenericSetup()
        {
        }

        protected virtual void SpecializeRegister(Container container)
        {
        }

        protected virtual void PostGenericSetup()
        {
        }
    }
}
