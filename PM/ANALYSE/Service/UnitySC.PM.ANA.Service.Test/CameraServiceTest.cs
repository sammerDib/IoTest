using System;
using System.ServiceModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Test
{
    internal class GetSingleGrabImageTest : CameraServiceExTestClient
    {
        public new Response<ServiceImage> GetSingleGrabImage(string cameraId)
        {
            return _remoteService.TryInvokeAndGetMessages(m => m.GetSingleGrabImage(cameraId));
        }
    }

    [TestClass]
    public class CameraServiceTest
    {
        private Container _container;
        private ServiceHost _host;
        private Mock<ILogger<CameraServiceEx>> _logger;
        private ICameraServiceEx _serviceUnderTest;
        private Mock<DummyIDSCamera> _simulatedCamera;

        private const string CameraId = "1002";
        private const string ObjectiveId = "objectiveId";

        [TestInitialize]
        public void SetUp()
        {
            // Class container
            _container = new Container();
            Bootstrapper.Register(_container);
            _logger = new Mock<ILogger<CameraServiceEx>>();

            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var camLogger = new Mock<ILogger<DummyIDSCamera>>();

            _simulatedCamera = new Mock<DummyIDSCamera>(new CameraConfigBase() { Name = "Camera Up" }, globalStatusServer, camLogger.Object);
            _simulatedCamera.Setup(_ => _.IsAcquiring).Returns(true);

            var image = new DummyUSPImage(10, 10, 200);
            _simulatedCamera.Setup(_ => _.SingleGrab()).Returns(image);

            _simulatedCamera.Object.Config.ObjectivesSelectorID = ObjectiveId;
            hardwareManager.Cameras[CameraId] = _simulatedCamera.Object;

            _serviceUnderTest = new CameraServiceEx(_logger.Object);
            ((CameraServiceEx)_serviceUnderTest).Init();

            _host = new ServiceHost(_serviceUnderTest);
            _host.Open();
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                _host?.Close();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Cannot close service: " + e.Message);
            }
        }

        //  Todo Aurelien Carlier : Error in DevOps. Bug 5671
        // [TestMethod]
        //[TestCategory("WCF communication")]
        public void Expect_DummyUSPImageMil_to_be_convertible_to_ServiceImageWithStatistics()
        {
            // Given
            var client = new GetSingleGrabImageTest();

            // When
            Response<ServiceImage> response = null;
            try
            {
                response = client.GetSingleGrabImage(CameraId);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            // Then
            Assert.IsNotNull(response);
            Assert.IsNull(response.Exception);
        }
    }
}
