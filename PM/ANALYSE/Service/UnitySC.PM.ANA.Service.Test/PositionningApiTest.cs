using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface.Positionning;

namespace UnitySC.PM.ANA.Service.Test
{
    //
    // 
    // ---------------- Unit tests
    // 
    //

    [TestClass]
    public class PositionningApiTest
    {

        [TestMethod]
        public void Expect_multi_axis_position_to_be_creatable()
        {
            const int INCREMENT = 10;
            const double X = 0;
            const double Y = 0;
            const double ZTop = 0;
            const double ZBottom = 0;
            var initialPosition = new CompositePosition.Builder(Referential.Image)
            .Add(Axes.X, X)
            .Add(Axes.Y, Y)
            .Add(Axes.ZTop, ZTop)
            .Add(Axes.ZBottom, ZBottom)
            .Build();

            var referentialManager = new ReferentialManagerImpl();
            var imageTransform = new IncrementAllAxesTransformBy(INCREMENT);
            referentialManager.Add(imageTransform).For(Referential.Image);
            var destination = referentialManager.Convert(initialPosition).To(Referential.Stage);

            Assert.AreEqual(X + INCREMENT, destination.Positions[0].SetPoint);
            Assert.AreEqual(Y + INCREMENT, destination.Positions[1].SetPoint);
            Assert.AreEqual(ZTop + INCREMENT, destination.Positions[2].SetPoint);
            Assert.AreEqual(ZBottom + INCREMENT, destination.Positions[3].SetPoint);
        }


        [TestMethod]
        public void Expect_conversion_to_apply_many_transformations()
        {
            ///
            /// In this test we will assume that:
            /// - image is 100x100 pixels, with pixel scale = 1x1mm and current image origin at 200,200
            /// - wafer is shifted 5px to the left and 3px to the top (BWA)
            /// - each coordinates must be "floored" before go to motor (calibration)
            /// 
            ///

            ///
            /// Arrange
            ///    
            var imageTransform = new ImageOriginTransformation(200, 200);
            var bwaTransform = new WaferAlignmentTransformation(-5, 3);
            var calibrationTransform = new ABCCalibrationTransformation();

            var referentialManager = new ReferentialManagerImpl();
            referentialManager.Add(imageTransform).For(Referential.Image);
            referentialManager.Add(bwaTransform).For(Referential.Wafer);
            referentialManager.Add(calibrationTransform).For(Referential.Stage);

            double IMAGE_POS = 40.6;
            double WAFER_POS = IMAGE_POS + 200;
            double STAGE_POS = WAFER_POS + 5;
            double MOTOR_POS = Math.Floor(STAGE_POS);

            var initialPosition = new CompositePosition.Builder(Referential.Image)
                .Add(Axes.X, IMAGE_POS)
                .Build();

            ///
            /// Act
            ///

            // no transform
            var imageXPosition = referentialManager.Convert(initialPosition).To(Referential.Image);

            // IMAGE -> WAFER : imageTransform executed
            var waferXPosition = referentialManager.Convert(imageXPosition).To(Referential.Wafer);

            // WAFER -> STAGE : bwaTransform executed
            var stageXPosition = referentialManager.Convert(waferXPosition).To(Referential.Stage);

            // STAGE -> MOTOR : calibrationTransform executed
            var motorXPosition = referentialManager.Convert(stageXPosition).To(Referential.Motor);

            // IMAGE -> STAGE : imageTransform, bwaTransform, calibrationTransform executed
            var directMotorXPosition = referentialManager.Convert(initialPosition).To(Referential.Motor);

            ///
            /// Assert
            ///
            Assert.AreEqual(IMAGE_POS, imageXPosition.Positions[0].SetPoint, $"Same referential changes nothing");
            Assert.AreEqual(WAFER_POS, waferXPosition.Positions[0].SetPoint, $"Image to Wafer must work");
            Assert.AreEqual(STAGE_POS, stageXPosition.Positions[0].SetPoint, $"Wafer to stage must work");
            Assert.AreEqual(MOTOR_POS, motorXPosition.Positions[0].SetPoint, $"Stage to motor must work");
            Assert.AreEqual(MOTOR_POS, directMotorXPosition.Positions[0].SetPoint, $"Direct transform to motor must work");

        }

        [TestMethod]
        public void Expect_exception_when_adding_position_in_different_referentials_using_builder()
        {
            const Referential FIRST_REF = Referential.Stage;
            const Referential SECOND_REF = Referential.Image;

            // Short API
            var positionBuilder = new CompositePosition.Builder(FIRST_REF);
            try
            {
                positionBuilder.Add(new BasePosition(SECOND_REF, Axes.X, 6.2));
                Assert.Fail($"Adding position in different referential must throw");
            }
            catch (Exception)
            {
                // ok
            }
        }

        /// <summary>
        /// Ensure exception is throw for example when trying to convert
        /// from Wafer to Die referential
        /// </summary>
        [TestMethod]
        public void Expect_conversion_to_upper_referential_to_throw()
        {
            // Arrange
            var referentialManager = new ReferentialManagerImpl();
            var imageXPosition = new CompositePosition.Builder(Referential.Stage)
                .Add(Axes.X, 6.2)
                .Build();

            // Act
            try
            {
                referentialManager.Convert(imageXPosition).To(Referential.Image);
                Assert.Fail($"Converting to a more precise referential must throw");
            }
            catch (ArgumentException)
            {
                // Ok
            }
        }

        [TestMethod]
        public void Expect_conversion_to_apply_one_transformations()
        {
            // Arrange
            var divideByTwo = new DivideValueByTwoTransformation();
            var referentialManager = new ReferentialManagerImpl();
            referentialManager.Add(divideByTwo).For(Referential.Stage);

            const double INPUT = 6.2;
            var imageXPosition = new CompositePosition.Builder(Referential.Stage)
                .Add(Axes.X, INPUT)
                .Build();

            // Act
            var result = referentialManager.Convert(imageXPosition).To(Referential.Motor);

            // Assert
            Assert.AreEqual(INPUT / 2, result.Positions[0].SetPoint);

        }

        //
        // 
        // ---------------- test BaseTransformation
        // 
        //

        private class IncrementAllAxesTransformBy : BaseTransformation
        {
            private int _increment;

            public IncrementAllAxesTransformBy(int increment)
            {
                _increment = increment;
            }
            public override BasePosition Transform(BasePosition from, Referential referentialDest)
            {
                return new BasePosition(referentialDest, from.Axis, from.SetPoint + _increment);
            }
        }
        /// <summary>
        /// Floors coordinate value
        /// </summary>
        private class ABCCalibrationTransformation : BaseTransformation
        {
            public override BasePosition Transform(BasePosition from, Referential referentialDest)
            {
                return new BasePosition(referentialDest, from.Axis, Math.Floor(from.SetPoint));
            }
        }

        /// <summary>
        /// /!\ Naive implementation which do no handle angle misplacement
        /// </summary>
        private class WaferAlignmentTransformation : BaseTransformation
        {
            private readonly double _shiftX;
            private readonly double _shiftY;

            public WaferAlignmentTransformation(double shiftX, double shiftY)
            {
                _shiftX = shiftX;
                _shiftY = shiftY;
            }
            public override BasePosition Transform(BasePosition from, Referential referentialDest)
            {
                BasePosition result = null;
                if (from.Axis == Axes.X)
                {
                    result = new BasePosition(referentialDest, from.Axis, from.SetPoint - _shiftX);
                }
                else if (from.Axis == Axes.Y)
                {
                    result = new BasePosition(referentialDest, from.Axis, from.SetPoint - _shiftY);
                }
                return result;
            }
        }

        private class ImageOriginTransformation : BaseTransformation
        {
            private int _imageOriginX;
            private int _imageOriginY;

            public ImageOriginTransformation(int imageOriginX, int imageOriginY)
            {
                _imageOriginX = imageOriginX;
                _imageOriginY = imageOriginY;
            }

            public override BasePosition Transform(BasePosition from, Referential referentialDest)
            {
                BasePosition result = null;
                if (from.Axis == Axes.X)
                {
                    result = new BasePosition(referentialDest, from.Axis, from.SetPoint + _imageOriginX);
                }
                else if (from.Axis == Axes.Y)
                {
                    result = new BasePosition(referentialDest, from.Axis, from.SetPoint + _imageOriginY);
                }
                return result;
            }
        }

        private class DivideValueByTwoTransformation : BaseTransformation
        {
            public override BasePosition Transform(BasePosition from, Referential referentialDest)
            {
                return new BasePosition(referentialDest, Axes.X, from.SetPoint / 2);
            }
        }
    }
}
