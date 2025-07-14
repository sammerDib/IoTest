using System.Linq;
using System.Runtime.InteropServices;

using CommunityToolkit.Mvvm.Messaging;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;
using SimpleInjector.Advanced;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Referentials;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Hardware.Test
{
    [TestClass]
    public class PhotoLumAxesTests
    {
        IEMEServiceConfigurationManager configManager =null;

        [TestInitialize]
        public void InitializerContext()
        {
            configManager = ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>();
        }

        [TestMethod]
        public void PhotoLumAxes_Should_ContainFourEnabledAxes()
        {          
            // Given
            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();
            var referentialManager = new EmeReferentialManager();
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), logFacto, configManager, new StubGlobalStatus(), referentialManager);
           
            // When
            hardwareManager.Init();

            // Then
            hardwareManager.MotionAxes.Should().BeOfType(typeof(PhotoLumAxes));

            var photoLumAxes = (PhotoLumAxes)hardwareManager.MotionAxes;
            photoLumAxes.Axes.Should().HaveCount(3);
            photoLumAxes.Axes.Select(a => a.AxisID).Should().Contain("X", "Y", "Z");
            photoLumAxes.GetCurrentState().AllAxisEnabled.Should().BeTrue();
        }

        [TestMethod]
        public void PhotoLumAxes_Should_ContainThreeEnabledAxes()
        {
            // Given
            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();
            var referentialManager = new EmeReferentialManager();
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), logFacto, configManager, new StubGlobalStatus(), referentialManager);
            
            // When
            hardwareManager.Init();

            // Then
            hardwareManager.MotionAxes.Should().BeOfType(typeof(PhotoLumAxes));

            var photoLumAxes = (PhotoLumAxes)hardwareManager.MotionAxes;
            photoLumAxes.Axes.Should().HaveCount(3);
            photoLumAxes.Axes.Select(a => a.AxisID).Should().Contain("X", "Y", "Z");
            photoLumAxes.GetCurrentState().AllAxisEnabled.Should().BeTrue();
        }
        
        [TestMethod]
        public void Should_MoveAndGiveCorrectPosition()
        {
            // Given
            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();

            var referentialManager = new EmeReferentialManager();
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), logFacto, configManager, new StubGlobalStatus(), referentialManager);
            hardwareManager.Init();
            var photoLumAxes = (PhotoLumAxes)hardwareManager.MotionAxes;
            double targetX = 1;
            double targetY = 2;
            double targetZ = 0.5;

            // When
            PMAxisMove[] moves =
            {
                new PMAxisMove("X", targetX.Millimeters()),
                new PMAxisMove("Y", targetY.Millimeters()),
                new PMAxisMove("Z", targetZ.Millimeters()),
            };
            photoLumAxes.Move(moves);
            photoLumAxes.WaitMotionEnd(500);
            var position = photoLumAxes.GetPosition();

            // Then
            position.Should().BeOfType(typeof(XYZPosition));
            var xyzPosition = (XYZPosition)position;
            xyzPosition.Referential.Should().Be(new MotorReferential());
            xyzPosition.X.Should().BeApproximately(targetX, 0.001.Millimeters().Value);
            xyzPosition.Y.Should().BeApproximately(targetY, 0.001.Millimeters().Value);
            xyzPosition.Z.Should().BeApproximately(targetZ, 0.001.Millimeters().Value);
        }

        [TestMethod]
        public void Should_NotifyNewPosition()
        {
            // Given
            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();
            var referentialManager = new EmeReferentialManager();
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), logFacto, configManager, new StubGlobalStatus(), referentialManager);           
            hardwareManager.Init();
            var photoLumAxes = (PhotoLumAxes)hardwareManager.MotionAxes;

            PositionBase position = null;
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<AxesPositionChangedMessage>(this, (_, message) => position = message.Position);

            // When
            PMAxisMove[] moves = { new PMAxisMove("X", 1.Millimeters()) };
            photoLumAxes.Move(moves);
            photoLumAxes.WaitMotionEnd(500);

            // Then
            position.Should().NotBeNull().And.BeOfType(typeof(XYZPosition));
        }

        [TestMethod]
        public void Should_GoToCorrectXYPosition_InMotorReferential()
        {
            // Given                   
            var referentialManager = new EmeReferentialManager();
            var waferReferentialSettings = new WaferReferentialSettings { ShiftX = 2.Millimeters(), ShiftY = 1.Millimeters(), WaferAngle = 90.Degrees() };
            referentialManager.SetSettings(waferReferentialSettings);

            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();

            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), logFacto, configManager, new StubGlobalStatus(), referentialManager);     
            hardwareManager.Init();
            var photoLumAxes = (PhotoLumAxes)hardwareManager.MotionAxes;

            // When
            var newPosition = new XYPosition(new WaferReferential(), -0.5, 1);
            photoLumAxes.GoToPosition(newPosition);
            photoLumAxes.WaitMotionEnd(500);
            var position = photoLumAxes.GetPosition();

            // Then
            position.Should().NotBeNull().And.BeOfType(typeof(XYZPosition));
            var xyzPosition = (XYZPosition)position;
            xyzPosition.Referential.Should().Be(new MotorReferential());
            xyzPosition.X.Should().BeApproximately(1, 0.1);
            xyzPosition.Y.Should().BeApproximately(0.5, 0.1);
        }

        [TestMethod]
        public void Should_GoToCorrectXYZPosition_InMotorReferential()
        {            
            // Given                   
            var referentialManager = new EmeReferentialManager();
            var waferReferentialSettings = new WaferReferentialSettings { ShiftX = 2.Millimeters(), ShiftY = 1.Millimeters(), WaferAngle = 90.Degrees() };
            referentialManager.SetSettings(waferReferentialSettings);
            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();

            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(),logFacto, configManager, new StubGlobalStatus(), referentialManager);
            hardwareManager.Init();
            var photoLumAxes = (PhotoLumAxes)hardwareManager.MotionAxes;

            // When
            var newPosition = new XYZPosition(new WaferReferential(), -0.5, 1, 1);
            photoLumAxes.GoToPosition(newPosition);
            photoLumAxes.WaitMotionEnd(500);
            var position = photoLumAxes.GetPosition();

            // Then
            position.Should().NotBeNull().And.BeOfType(typeof(XYZPosition));
            var xyzPosition = (XYZPosition)position;
            xyzPosition.Referential.Should().Be(new MotorReferential());
            xyzPosition.X.Should().BeApproximately(1, 0.1);
            xyzPosition.Y.Should().BeApproximately(0.5, 0.1);
            xyzPosition.Z.Should().BeApproximately(1, 0.1);
        }
    }
}
