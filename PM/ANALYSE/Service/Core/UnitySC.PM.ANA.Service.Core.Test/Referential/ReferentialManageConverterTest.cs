using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Referential
{
    [TestClass]
    public class ReferentialManageConverterTest : TestWithMockedHardware<ReferentialManageConverterTest>, ITestWithCamera
    {
        #region Interfaces properties

        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            CalibManager.UpdateCalibration(new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>()
                {
                    new ObjectiveCalibration()
                    {
                        DeviceId = ObjectiveUpId,
                        ZOffsetWithMainObjective = 0.Millimeters(),
                        Image = new ImageParameters()
                        {
                            XOffset = 10.Millimeters(),
                            YOffset = 5.Millimeters(),
                        }
                    }
                }
            });
        }

        [TestMethod]
        public void Delete_wafer_referential_settings()
        {
            // Arrange
            var initialPositionInStageReferential = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 0, 0);
            var initialPositionInWaferReferential = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);

            var waferRefSettings = new WaferReferentialSettings();
            waferRefSettings.ShiftX = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.ShiftY = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.WaferAngle = new Angle(0, AngleUnit.Degree);

            // Act
            var referentialManager = new AnaReferentialManager();
            referentialManager.SetSettings(waferRefSettings);
            referentialManager.DeleteSettings(waferRefSettings.Tag);
            var resultPositionInWaferReferential = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPositionInStageReferential, ReferentialTag.Wafer);
            var resultPositionInStageReferential = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPositionInWaferReferential, ReferentialTag.Stage);

            // Assert : only referentials changes, the x, y, ztop and zbottom values ​​do not change
            Assert.AreEqual(initialPositionInWaferReferential, resultPositionInWaferReferential);
            Assert.AreEqual(initialPositionInStageReferential, resultPositionInStageReferential);
        }

        [TestMethod]
        public void Delete_die_referential_settings()
        {
            // Arrange
            var initialPositionInDieRef = new XYZTopZBottomPosition(new DieReferential(0, 0), 0, 0, 0, 0);
            var initialPositionInWaferRef = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 45.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), -150, 150),
                presenceGrid: new Matrix<bool>(new bool[1, 5] { { true, true, true, true, true } }),
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: 1.Millimeters(),
                                                dieHeight: 2.Millimeters(),
                                                streetWidth: 3.Millimeters(),
                                                streetHeight: 4.Millimeters(),
                                                dieAngle: 0.Degrees()));

            // Act
            var referentialManager = new AnaReferentialManager();
            referentialManager.SetSettings(dieRefSettings);
            referentialManager.DeleteSettings(dieRefSettings.Tag);

            // Assert : convert from die to wafer or from wafer to die thrown an exception
            Assert.ThrowsException<Exception>(() => referentialManager.ConvertTo(initialPositionInDieRef, ReferentialTag.Wafer));
            Assert.ThrowsException<Exception>(() => referentialManager.ConvertTo(initialPositionInWaferRef, ReferentialTag.Die));
        }

        [TestMethod]
        public void Motor_to_stage_and_stage_to_motor_conversion()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new MotorReferential(), 0, 0, 0, 0);

            // Act
            var referentialManager = new AnaReferentialManager();
            var resultPositionInStageRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPosition, ReferentialTag.Stage);
            var resultPositionInMotorRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(resultPositionInStageRef, ReferentialTag.Motor);

            // Assert
            Assert.AreNotEqual(new XYZTopZBottomPosition(new StageReferential(), initialPosition.X, initialPosition.Y, initialPosition.ZTop, initialPosition.ZBottom), resultPositionInStageRef);
            Assert.AreEqual(initialPosition, resultPositionInMotorRef);
        }

        [TestMethod]
        public void Stage_to_wafer_and_wafer_to_stage_conversion()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new StageReferential(), 1, 2, 3, 4);

            var waferRefSettings = new WaferReferentialSettings
            {
                ShiftX = 20.Millimeters(),
                ShiftY = 10.Millimeters(),
                WaferAngle = 0.Degrees(),
                ZTopFocus = 1.Millimeters()
            };

            // Act
            var referentialManager = new AnaReferentialManager();
            referentialManager.SetSettings(waferRefSettings);
            var resultPositionInWaferRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPosition, ReferentialTag.Wafer);
            var resultPositionInStageRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(resultPositionInWaferRef, ReferentialTag.Stage);

            // Assert
            Assert.AreNotEqual(new XYZTopZBottomPosition(new WaferReferential(), initialPosition.X, initialPosition.Y, initialPosition.ZTop, initialPosition.ZBottom), resultPositionInWaferRef);
            Assert.AreEqual(initialPosition, resultPositionInStageRef);
        }

        [TestMethod]
        public void Wafer_to_die_and_die_to_wafer_conversion()
        {
            // Arrange
            var initialPosition = new XYZTopZBottomPosition(new DieReferential(0, 0), 0, 0, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 10.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), -150, 150),
                presenceGrid: new Matrix<bool>(new bool[5, 5] {  { true, true, true, true, true },
                                                { true, true, true, true, true },
                                                { true, true, true, true, true },
                                                { true, true, true, true, true },
                                                { true, true, true, true, true }}),
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: 10.Millimeters(),
                                                dieHeight: 20.Millimeters(),
                                                streetWidth: 3.Millimeters(),
                                                streetHeight: 4.Millimeters(),
                                                dieAngle: 0.Degrees()));

            // Act
            var referentialManager = new AnaReferentialManager();
            referentialManager.SetSettings(dieRefSettings);
            var resultPositionInWaferRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPosition, ReferentialTag.Wafer);
            var resultPositionInDieRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(resultPositionInWaferRef, ReferentialTag.Die);

            // Assert
            Assert.AreNotEqual(new XYZTopZBottomPosition(new WaferReferential(), initialPosition.X, initialPosition.Y, initialPosition.ZTop, initialPosition.ZBottom), resultPositionInWaferRef);
            Assert.AreEqual(initialPosition, resultPositionInDieRef);
        }

        [TestMethod]
        public void Wafer_to_motor_and_motor_to_wafer_conversion()
        {
            // Arrange
            var initialPositionInWaferRef = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);

            var waferRefSettings = new WaferReferentialSettings();
            waferRefSettings.ShiftX = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.ShiftY = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.WaferAngle = new Angle(0, AngleUnit.Degree);

            // Act
            var referentialManager = new AnaReferentialManager();
            referentialManager.SetSettings(waferRefSettings);
            var resultPositionInMotorRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPositionInWaferRef, ReferentialTag.Motor);
            var resultPositionInWaferRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(resultPositionInMotorRef, ReferentialTag.Wafer);

            // Assert
            Assert.AreNotEqual(new XYZTopZBottomPosition(new MotorReferential(), initialPositionInWaferRef.X, initialPositionInWaferRef.Y, initialPositionInWaferRef.ZTop, initialPositionInWaferRef.ZBottom), resultPositionInMotorRef);
            Assert.AreEqual(initialPositionInWaferRef.Referential.Tag, resultPositionInWaferRef.Referential.Tag);
            Assert.AreEqual(initialPositionInWaferRef.X, resultPositionInWaferRef.X, 10e10);
            Assert.AreEqual(initialPositionInWaferRef.Y, resultPositionInWaferRef.Y, 10e10);
            Assert.AreEqual(initialPositionInWaferRef.ZTop, resultPositionInWaferRef.ZTop, 10e10);
            Assert.AreEqual(initialPositionInWaferRef.ZBottom, resultPositionInWaferRef.ZBottom, 10e10);
        }

        [TestMethod]
        public void Die_to_stage_and_stage_to_die_conversion()
        {
            // Arrange
            var initialPositionInDieRef = new XYZTopZBottomPosition(new DieReferential(0, 0), 0, 0, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 45.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), -150, 150),
                presenceGrid: new Matrix<bool>(new bool[1, 5] { { true, true, true, true, true } }),
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: 1.Millimeters(),
                                                dieHeight: 2.Millimeters(),
                                                streetWidth: 3.Millimeters(),
                                                streetHeight: 4.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferRefSettings = new WaferReferentialSettings();
            waferRefSettings.ShiftX = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.ShiftY = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.WaferAngle = new Angle(0, AngleUnit.Degree);

            // Act
            var referentialManager = new AnaReferentialManager();
            referentialManager.SetSettings(waferRefSettings);
            referentialManager.SetSettings(dieRefSettings);
            var resultPositionInStageRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPositionInDieRef, ReferentialTag.Stage);
            var resultPositionInDieRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(resultPositionInStageRef, ReferentialTag.Die);

            // Assert
            Assert.AreNotEqual(new XYZTopZBottomPosition(new StageReferential(), initialPositionInDieRef.X, initialPositionInDieRef.Y, initialPositionInDieRef.ZTop, initialPositionInDieRef.ZBottom), resultPositionInStageRef);
            Assert.AreEqual(initialPositionInDieRef.Referential.Tag, resultPositionInDieRef.Referential.Tag);
            Assert.AreEqual(initialPositionInDieRef.X, resultPositionInDieRef.X, 10e10);
            Assert.AreEqual(initialPositionInDieRef.Y, resultPositionInDieRef.Y, 10e10);
            Assert.AreEqual(initialPositionInDieRef.ZTop, resultPositionInDieRef.ZTop, 10e10);
            Assert.AreEqual(initialPositionInDieRef.ZBottom, resultPositionInDieRef.ZBottom, 10e10);
        }

        [TestMethod]
        public void Die_to_motor_and_motor_to_die_conversion()
        {
            // Arrange
            var initialPositionInDieRef = new XYZTopZBottomPosition(new DieReferential(0, 0), 0, 0, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 45.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), -150, 150),
                presenceGrid: new Matrix<bool>(new bool[1, 5] { { true, true, true, true, true } }),
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: 1.Millimeters(),
                                                dieHeight: 2.Millimeters(),
                                                streetWidth: 3.Millimeters(),
                                                streetHeight: 4.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferRefSettings = new WaferReferentialSettings();
            waferRefSettings.ShiftX = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.ShiftY = new Length(10, LengthUnit.Micrometer);
            waferRefSettings.WaferAngle = new Angle(0, AngleUnit.Degree);

            // Act
            var referentialManager = new AnaReferentialManager();
            referentialManager.SetSettings(waferRefSettings);
            referentialManager.SetSettings(dieRefSettings);
            var resultPositionInMotorRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(initialPositionInDieRef, ReferentialTag.Motor);
            var resultPositionInDieRef = (XYZTopZBottomPosition)referentialManager.ConvertTo(resultPositionInMotorRef, ReferentialTag.Die);

            // Assert
            Assert.AreNotEqual(new XYZTopZBottomPosition(new MotorReferential(), initialPositionInDieRef.X, initialPositionInDieRef.Y, initialPositionInDieRef.ZTop, initialPositionInDieRef.ZBottom), resultPositionInMotorRef);
            Assert.AreEqual(initialPositionInDieRef.Referential.Tag, resultPositionInDieRef.Referential.Tag);
            Assert.AreEqual(initialPositionInDieRef.X, resultPositionInDieRef.X, 10e10);
            Assert.AreEqual(initialPositionInDieRef.Y, resultPositionInDieRef.Y, 10e10);
            Assert.AreEqual(initialPositionInDieRef.ZTop, resultPositionInDieRef.ZTop, 10e10);
            Assert.AreEqual(initialPositionInDieRef.ZBottom, resultPositionInDieRef.ZBottom, 10e10);
        }
    }
}
