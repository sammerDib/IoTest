using System.Collections.Generic;
using System.ServiceModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Service.Test
{
    [TestClass]
    public class AxesServiceTest
    {
        private AxesService _serviceUnderTest;

        protected Mock<IReferentialManager> SimulatedReferentialManager;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            Bootstrapper.Register(container);
            SimulatedReferentialManager = Bootstrapper.SimulatedReferentialManager;

            _serviceUnderTest = ClassLocator.Default.GetInstance<AxesService>();
        }

        [TestMethod]
        public void PositionChanged_notifies_subscribers_with_given_position_converted_in_wafer_referential()
        {
            // Given
            var listener = BuildListenerMock();
            _serviceUnderTest.Subscribe(listener.Object);

            var position = BuildPosition(new MotorReferential());
            var convertedPosition = BuildPosition(new WaferReferential());
            SimulatedReferentialManager.Setup(rm => rm.ConvertTo(position, ReferentialTag.Wafer))
                .Returns(convertedPosition);

            // When
            _serviceUnderTest.PositionChanged(position);

            // Then
            listener.Verify(l => l.PositionChangedCallback(convertedPosition));
        }

        private static Mock<IAxesServiceCallback> BuildListenerMock()
        {
            var listenerAsComm = new Mock<ICommunicationObject>();
            listenerAsComm.SetupGet(l => l.State).Returns(CommunicationState.Opened);
            return listenerAsComm.As<IAxesServiceCallback>();
        }

        private static PositionBase BuildPosition(ReferentialBase referential)
        {
            return new XYZTopZBottomPosition(referential, 0, 1, 2, 3);
        }
    }
}
