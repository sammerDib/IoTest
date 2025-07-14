using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.ANA.Service.Interface.Referential;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Referential
{
    [TestClass]
    public class DieWaferConverterTest
    {
        private readonly int _defaultDieGridTopLeftX = -150;
        private readonly int _defaultDieGridTopLeftY = 150;

        private readonly Matrix<bool> _defaultPresenceGrid = new Matrix<bool>(new bool[5, 5]{
                                                    {true,true,true,true, true},
                                                    { true,true,true,true, true},
                                                    { true,true,true,true, true},
                                                    { true,true,true,true, true},
                                                    { true,true,true,true, true}});

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            ClassLocator.ExternalInit(container, true);

            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
        }   


        [TestMethod]
        public void Expect_wafer_referential_when_convert_from_die_to_wafer()
        {
            // Arrange :
            // Position (0,0) into die referential & Die position (line:0, column:0) into die grid

            var initialPosition = new XYZTopZBottomPosition(new DieReferential(line: 0, column: 0), 0, 0, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: 0.Millimeters(),
                                                dieHeight: 0.Millimeters(),
                                                streetWidth: 0.Millimeters(),
                                                streetHeight: 0.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var dieToWaferConverter = new DieToWaferConverter(dieRefSettings);

            // Act
            var resultPositionWafer = dieToWaferConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(ReferentialTag.Wafer, resultPositionWafer.Referential.Tag);
        }

        [TestMethod]
        public void Expect_die_referential_when_convert_from_wafer_to_die()
        {
            // Arrange :
            // Position (0,0) into die referential & Die position (line:0, column:0) into die grid

            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: 0.Millimeters(),
                                                dieHeight: 0.Millimeters(),
                                                streetWidth: 0.Millimeters(),
                                                streetHeight: 0.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionDie = waferToDieConverter.Convert(initialPosition);

            // Assert
            Assert.AreEqual(ReferentialTag.Die, resultPositionDie.Referential.Tag);
        }

        [TestMethod]
        public void Die_to_wafer_conversion_considers_origin_of_the_die_referential_at_the_bottom_left_corner_of_the_die()
        {
            // Arrange :
            // Position (0,0) into die referential & Die position (line:0, column:0) into die grid

            var initialPosition = new XYZTopZBottomPosition(new DieReferential(line: 0, column: 0), 0, 0, 0, 0);

            int dieHeight = 10;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var dieToWaferConverter = new DieToWaferConverter(dieRefSettings);

            // Act
            var resultPositionWafer = dieToWaferConverter.Convert(initialPosition);

            // Assert
            int theoricalPositionWaferX = _defaultDieGridTopLeftX;
            int theoricalPositionWaferY = _defaultDieGridTopLeftY - dieHeight;
            Assert.AreEqual(theoricalPositionWaferX, resultPositionWafer.X);
            Assert.AreEqual(theoricalPositionWaferY, resultPositionWafer.Y);
        }

        [TestMethod]
        public void Wafer_to_die_conversion_considers_origin_of_the_die_referential_at_the_bottom_left_corner_of_the_die()
        {
            // Arrange :

            int dieHeight = 10;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;

            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY - dieHeight, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionDie = waferToDieConverter.Convert(initialPosition);

            // Assert
            // Position (0,0) into die referential & Die position (line:0, column:0) into die grid
            Assert.AreEqual(0, (resultPositionDie.Referential as DieReferential).DieLine);
            Assert.AreEqual(0, (resultPositionDie.Referential as DieReferential).DieColumn);
            Assert.AreEqual(0, resultPositionDie.X);
            Assert.AreEqual(0, resultPositionDie.Y);
        }

        [TestMethod]
        public void Die_to_wafer_conversion_considers_origin_of_die_grid_is_at_the_top_left_of_the_grid()
        {
            // Arrange
            // Position (0,0) into die referential & Die position (line:1, column:2) into die grid

            int dieLine = 1;
            int dieColumn = 2;
            var initialPosition = new XYZTopZBottomPosition(new DieReferential(line: dieLine, column: dieColumn), 0, 0, 0, 0);

            int dieHeight = 10;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var dieToWaferConverter = new DieToWaferConverter(dieRefSettings);

            // Act
            var resultPositionWafer = dieToWaferConverter.Convert(initialPosition);

            // Assert
            int theoricalPositionWaferX = _defaultDieGridTopLeftX + dieColumn * (dieWidth + streetWidth);
            int theoricalPositionWaferY = _defaultDieGridTopLeftY - dieLine * (dieHeight + streetHeight) - dieHeight;
            Assert.AreEqual(theoricalPositionWaferX, resultPositionWafer.X);
            Assert.AreEqual(theoricalPositionWaferY, resultPositionWafer.Y);
        }

        [TestMethod]
        public void Wafer_to_die_conversion_considers_origin_of_die_grid_is_at_the_top_left_of_the_grid()
        {
            // Arrange
            int dieLine = 1;
            int dieColumn = 2;
            int dieHeight = 10;
            int streetHeight = 0;
            int dieWidth = 30;
            int streetWidth = 0;

            var initialPosition = new XYZTopZBottomPosition(
                new WaferReferential(),
                _defaultDieGridTopLeftX + dieColumn * (dieWidth + streetWidth),
                _defaultDieGridTopLeftY - dieLine * (dieHeight + streetHeight) - dieHeight,
                0,
                0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionDie = waferToDieConverter.Convert(initialPosition);

            // Assert
            // Position (0,0) into die referential & Die position (line:1, column:2) into die grid
            Assert.AreEqual(dieLine, (resultPositionDie.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionDie.Referential as DieReferential).DieColumn);
            Assert.AreEqual(0, resultPositionDie.X);
            Assert.AreEqual(0, resultPositionDie.Y);
        }

        [TestMethod]
        public void Position_places_outside_die_belongs_to_the_nearest_die_in_die_referential()
        {
            // Arrange
            int dieLine = 2;
            int dieColumn = 2;
            int dieHeight = 10;
            int streetHeight = 5;
            int dieWidth = 30;
            int streetWidth = 5;

            var dieOriginX = _defaultDieGridTopLeftX + dieColumn * (dieWidth + streetWidth);
            var dieOriginY = _defaultDieGridTopLeftY - dieLine * (dieHeight + streetHeight) - dieHeight;

            var positionInNearLeftStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX - 2, dieOriginY, 0, 0);
            var positionInNearRigthStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX + dieWidth + 2, dieOriginY, 0, 0);
            var positionInNearTopStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX, dieOriginY + dieHeight + 2, 0, 0);
            var positionInNearBottomStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX, dieOriginY - 2, 0, 0);
            var positionInFarLeftStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX - 3, dieOriginY, 0, 0);
            var positionInFarRigthStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX + dieWidth + 3, dieOriginY, 0, 0);
            var positionInFarTopStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX, dieOriginY + dieHeight + 3, 0, 0);
            var positionInFarBottomStreet = new XYZTopZBottomPosition(new WaferReferential(), dieOriginX, dieOriginY - 3, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionInNearLeftStreet = waferToDieConverter.Convert(positionInNearLeftStreet);
            var resultPositionInNearRigthStreet = waferToDieConverter.Convert(positionInNearRigthStreet);
            var resultPositionInNearTopStreet = waferToDieConverter.Convert(positionInNearTopStreet);
            var resultPositionInNearBottomStreet = waferToDieConverter.Convert(positionInNearBottomStreet);
            var resultPositionInFarLeftStreet = waferToDieConverter.Convert(positionInFarLeftStreet);
            var resultPositionInFarRigthStreet = waferToDieConverter.Convert(positionInFarRigthStreet);
            var resultPositionInFarTopStreet = waferToDieConverter.Convert(positionInFarTopStreet);
            var resultPositionInFarBottomStreet = waferToDieConverter.Convert(positionInFarBottomStreet);

            // Assert
            Assert.AreEqual(dieLine, (resultPositionInNearLeftStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionInNearLeftStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(-2, resultPositionInNearLeftStreet.X);
            Assert.AreEqual(0, resultPositionInNearLeftStreet.Y);
            Assert.AreEqual(dieLine, (resultPositionInNearRigthStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionInNearRigthStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(dieWidth + 2, resultPositionInNearRigthStreet.X);
            Assert.AreEqual(0, resultPositionInNearRigthStreet.Y);
            Assert.AreEqual(dieLine, (resultPositionInNearTopStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionInNearTopStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(0, resultPositionInNearTopStreet.X);
            Assert.AreEqual(dieHeight + 2, resultPositionInNearTopStreet.Y);
            Assert.AreEqual(dieLine, (resultPositionInNearBottomStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionInNearBottomStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(0, resultPositionInNearBottomStreet.X);
            Assert.AreEqual(-2, resultPositionInNearBottomStreet.Y);
            Assert.AreEqual(dieLine, (resultPositionInFarLeftStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn - 1, (resultPositionInFarLeftStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(dieWidth + 2, resultPositionInFarLeftStreet.X);
            Assert.AreEqual(0, resultPositionInFarLeftStreet.Y);
            Assert.AreEqual(dieLine, (resultPositionInFarRigthStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn + 1, (resultPositionInFarRigthStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(-2, resultPositionInFarRigthStreet.X);
            Assert.AreEqual(0, resultPositionInFarRigthStreet.Y);
            Assert.AreEqual(dieLine - 1, (resultPositionInFarTopStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionInFarTopStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(0, resultPositionInFarTopStreet.X);
            Assert.AreEqual(-2, resultPositionInFarTopStreet.Y);
            Assert.AreEqual(dieLine + 1, (resultPositionInFarBottomStreet.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionInFarBottomStreet.Referential as DieReferential).DieColumn);
            Assert.AreEqual(0, resultPositionInFarBottomStreet.X);
            Assert.AreEqual(dieHeight + 2, resultPositionInFarBottomStreet.Y);
        }

        [TestMethod]
        public void Expect_correct_rotation_when_converting_from_die_to_wafer()
        {
            // Arrange
            // Position (0,0) into die referential & Die position (line:0, column:0) into die grid
            // Rotation of 90° between die grid and wafer, with rotation center = dieGridTopLeft position.

            var initialPosition = new XYZTopZBottomPosition(new DieReferential(line: 0, column: 0), 0, 0, 0, 0);

            int dieHeight = 10;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 90.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var dieToWaferConverter = new DieToWaferConverter(dieRefSettings);

            // Act
            var resultPositionWafer = dieToWaferConverter.Convert(initialPosition);

            // Assert
            int theoricalPositionWaferX = _defaultDieGridTopLeftX + dieHeight;
            int theoricalPositionWaferY = _defaultDieGridTopLeftY;
            Assert.AreEqual(theoricalPositionWaferX, resultPositionWafer.X);
            Assert.AreEqual(theoricalPositionWaferY, resultPositionWafer.Y);
        }

        [TestMethod]
        public void Expect_correct_rotation_when_converting_from_wafer_to_die()
        {
            // Arrange
            // Rotation of 90° between die grid and wafer, with rotation center = dieGridTopLeft position.

            int dieHeight = 10;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;

            var initialPosition = new XYZTopZBottomPosition(
                new WaferReferential(),
                _defaultDieGridTopLeftX + dieHeight,
                _defaultDieGridTopLeftY,
                0,
                0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 90.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionDie = waferToDieConverter.Convert(initialPosition);

            // Assert
            // Position (0,0) into die referential & Die position (line:0, column:0) into die grid

            Assert.AreEqual(0, (resultPositionDie.Referential as DieReferential).DieLine);
            Assert.AreEqual(0, (resultPositionDie.Referential as DieReferential).DieColumn);
            Assert.AreEqual(0, resultPositionDie.X);
            Assert.AreEqual(0, resultPositionDie.Y);
        }

        [TestMethod]
        public void Expect_correct_shift_when_converting_from_die_to_wafer()
        {
            // Arrange
            // Position (4,6) into die referential & Die position (line:1, column:2) into die grid
            // No rotation between die grid and wafer.

            int posXInDie = 4;
            int posYInDie = 6;
            int dieLine = 1;
            int dieColumn = 2;
            var initialPosition = new XYZTopZBottomPosition(new DieReferential(line: dieLine, column: dieColumn), posXInDie, posYInDie, 0, 0);

            int dieHeight = 10;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var dieToWaferConverter = new DieToWaferConverter(dieRefSettings);

            // Act
            var resultPositionWafer = dieToWaferConverter.Convert(initialPosition);

            // Assert
            int theoricalPositionWaferX = _defaultDieGridTopLeftX + posXInDie + dieColumn * (dieWidth + streetWidth);
            int theoricalPositionWaferY = _defaultDieGridTopLeftY + posYInDie - dieHeight - dieLine * (dieHeight + streetHeight);
            Assert.AreEqual(theoricalPositionWaferX, resultPositionWafer.X);
            Assert.AreEqual(theoricalPositionWaferY, resultPositionWafer.Y);
        }

        [TestMethod]
        public void Expect_correct_shift_when_converting_from_wafer_to_die()
        {
            // Arrange

            int posXInDie = 4;
            int posYInDie = 6;
            int dieLine = 1;
            int dieColumn = 2;
            int dieHeight = 10;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;
            var initialPosition = new XYZTopZBottomPosition(
                new WaferReferential(),
                _defaultDieGridTopLeftX + posXInDie + dieColumn * (dieWidth + streetWidth),
                _defaultDieGridTopLeftY + posYInDie - dieHeight - dieLine * (dieHeight + streetHeight),
                0,
                0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 0.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionDie = waferToDieConverter.Convert(initialPosition);

            // Assert
            // Position (4,6) into die referential & Die position (line:1, column:2) into die grid
            Assert.AreEqual(dieLine, (resultPositionDie.Referential as DieReferential).DieLine);
            Assert.AreEqual(dieColumn, (resultPositionDie.Referential as DieReferential).DieColumn);
            Assert.AreEqual(posXInDie, resultPositionDie.X);
            Assert.AreEqual(posYInDie, resultPositionDie.Y);
        }

        [TestMethod]
        public void Expect_conversion_loop_success_when_position_is_inside_die()
        {
            // Arrange
            // Position (4,6) into die referential & Die position (line:1, column:2) into die grid
            // Rotation of 12° between die grid and wafer, with rotation center = dieGridTopLeft position.

            int posXInDie = 14;
            int posYInDie = 16;
            int dieLine = 3;
            int dieColumn = 2;
            var initialPosition = new XYZTopZBottomPosition(new DieReferential(line: dieLine, column: dieColumn), posXInDie, posYInDie, 0, 0);

            int dieHeight = 40;
            int streetHeight = 1;
            int dieWidth = 30;
            int streetWidth = 3;

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 12.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var dieToWaferConverter = new DieToWaferConverter(dieRefSettings);
            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionWafer = dieToWaferConverter.Convert(initialPosition);
            var resultPositionDie = waferToDieConverter.Convert(resultPositionWafer);

            // Assert
            // Position (14,16) into die referential & Die position (line:3, column:2) into die grid
            Assert.AreEqual(dieLine, (resultPositionDie.Referential as DieReferential).DieLine, 10e-10);
            Assert.AreEqual(dieColumn, (resultPositionDie.Referential as DieReferential).DieColumn, 10e-10);
            Assert.AreEqual(posXInDie, resultPositionDie.X, 10e-10);
            Assert.AreEqual(posYInDie, resultPositionDie.Y, 10e-10);
        }

        [TestMethod]
        public void Expect_conversion_loop_success_when_position_is_between_dies()
        {
            // Arrange
            int dieHeight = 40;
            int streetHeight = 5;
            int dieWidth = 30;
            int streetWidth = 5;

            int posXInDie = -2;
            int posYInDie = -2;
            int dieLine = 3;
            int dieColumn = 2;
            var initialPosition = new XYZTopZBottomPosition(new DieReferential(line: dieLine, column: dieColumn), posXInDie, posYInDie, 0, 0);

            var dieRefSettings = new DieReferentialSettings(
                dieGridAngle: 12.Degrees(),
                dieGridTopLeft: new XYPosition(new WaferReferential(), _defaultDieGridTopLeftX, _defaultDieGridTopLeftY),
                presenceGrid: _defaultPresenceGrid,
                dieDimensionalCharacteristic: new UnitySC.Shared.Data.DieDimensionalCharacteristic(
                                                dieWidth: dieWidth.Millimeters(),
                                                dieHeight: dieHeight.Millimeters(),
                                                streetWidth: streetWidth.Millimeters(),
                                                streetHeight: streetHeight.Millimeters(),
                                                dieAngle: 0.Degrees()));

            var dieToWaferConverter = new DieToWaferConverter(dieRefSettings);
            var waferToDieConverter = new WaferToDieConverter(dieRefSettings);

            // Act
            var resultPositionWafer = dieToWaferConverter.Convert(initialPosition);
            var resultPositionDie = waferToDieConverter.Convert(resultPositionWafer);

            // Assert
            Assert.AreEqual(dieLine, (resultPositionDie.Referential as DieReferential).DieLine, 10e-10);
            Assert.AreEqual(dieColumn, (resultPositionDie.Referential as DieReferential).DieColumn, 10e-10);
            Assert.AreEqual(posXInDie, resultPositionDie.X, 10e-10);
            Assert.AreEqual(posYInDie, resultPositionDie.Y, 10e-10);
        }
    }
}
