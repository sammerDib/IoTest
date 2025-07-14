using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.DataValidation;

namespace UnitySC.Shared.Tools.Test.DataValidation
{
    [TestClass]
    public class DataValidationTest
    {
        [TestInitialize]
        public void InitializeTestClass()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [TestMethod]
        public void SimpleValidation_WithValidData_Returns0Errors()
        {
            // Given
            var simple = new Simple()
            {
                StringProperty = "123456",
                DoubleProperty = 5.4,
            };

            // When
            var errors = DataValidator.TryValidateObject(simple);

            // Then
            errors.Should().BeEmpty();
        }

        [TestMethod]
        public void SimpleTest_WitoutData_Returns2Errors()
        {
            // Given without data
            var simple = new Simple();
            var expectedErrorMessages = new List<string>()
            {
                "The StringProperty field is required.",
                "The field DoubleProperty must be between 1 and 10.",
            };

            // When
            var errors = DataValidator.TryValidateObject(simple);

            // Then
            errors.Select(e => e.ErrorMessage).Should()
                .NotBeEmpty()
                .And.HaveCount(2)
                .And.BeEquivalentTo(expectedErrorMessages);
        }

        [TestMethod]
        public void ComplexValidation_WithValidData_Returns0Errors()
        {
            // Given
            var complex = new Complex()
            {
                ListProperty = new List<Simple>() { new Simple() { StringProperty = "123456", DoubleProperty = 2.3 } },
            };

            // When
            var errors = DataValidator.TryValidateObject(complex);

            // Then
            errors.Should().BeEmpty();
        }

        [TestMethod]
        public void ComplexValidation_WithoutValidData_Returns2Errors()
        {
            // Given
            var complex = new Complex()
            {
                ListProperty = new List<Simple>() { new Simple() },
            };
            var expectedErrorMessagesFromSimple = new List<string>()
            {
                "The StringProperty field is required.",
                "The field DoubleProperty must be between 1 and 10.",
            };
            var expectedErrorMessages = new List<string>()
            {
                $"The property of type List<Simple> is not valid ! 2 error(s) found: {string.Join(" ; ", expectedErrorMessagesFromSimple)}",
            };

            // When
            var errors = DataValidator.TryValidateObject(complex);

            // Then
            errors.Select(e => e.ErrorMessage).Should()
                .NotBeEmpty()
                .And.HaveCount(1)
                .And.BeEquivalentTo(expectedErrorMessages);
        }
    }
}
