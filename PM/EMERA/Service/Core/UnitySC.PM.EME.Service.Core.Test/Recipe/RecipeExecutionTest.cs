using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Optional.Unsafe;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class RecipeExecutionTest : TestWithMockedHardware<RecipeExecutionTest>, ITestWithFilterWheel, ITestWithPhotoLumAxes
    {
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }
        public Mock<FilterWheel> SimulatedFilterWheel { get; set; }

        [TestMethod]
        public void ShouldFindZFocus()
        {
            // Arrange
            var recipePreparation =
                new RecipeExecution(null, new FakeConfigurationManager(true), HardwareManager, null);

            // Act
            var filter = new Filter { Type = EMEFilter.BandPass450nm50, DistanceOnFocus = 20 };
            var result = recipePreparation.FindZFocus(filter);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.ValueOrDefault());
        }
    }
}
