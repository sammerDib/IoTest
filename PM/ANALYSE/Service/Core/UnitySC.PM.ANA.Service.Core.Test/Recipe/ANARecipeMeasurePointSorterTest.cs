using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.Recipe.WaferMap;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Recipe
{
    [TestClass]
    public class ANARecipeMeasurePointSorterTest : TestWithMockedHardware<ANARecipeMeasurePointSorterTest>, ITestWithAxes, ITestWithChuck
    {
        public Mock<IAxes> SimulatedAxes { get; set; }
        public Mock<ITestChuck> SimulatedChuck { get; set; }

        private readonly List<DieIndex> _unsortedDiesIndex = new List<DieIndex>()
        {
                new DieIndex(1, 1),
                new DieIndex(0, 0),
                new DieIndex(1, 0),
                new DieIndex(0, 1)
        };

        protected override void SpecializeRegister()
        {
            ClassLocator.Default.Register(() =>
               new MeasuresConfiguration()
               {
                   AuthorizedMeasures = new List<MeasureType>
                   {
                        MeasureType.TSV,
                        MeasureType.Thickness,
                        MeasureType.Bow,
                   },
                   Measures = new List<MeasureConfigurationBase>()
                   {
                       new MeasureTSVConfiguration(),
                       new MeasureThicknessConfiguration(),
                       new MeasureBowConfiguration(),
                   }
               });
        }

        protected override void PostGenericSetup()
        {
            /*Bootstrapper.SimulatedReferentialManager.SetupSequence(_ => _.ConvertTo(It.IsAny<PositionBase>(), ReferentialTag.Die))
                .Returns(new XYPosition(new DieReferential(_unsortedDiesIndex[0].Column, _unsortedDiesIndex[0].Row), 0, 0))
                .Returns(new XYPosition(new DieReferential(_unsortedDiesIndex[1].Column, _unsortedDiesIndex[1].Row), 0, 0))
                .Returns(new XYPosition(new DieReferential(_unsortedDiesIndex[2].Column, _unsortedDiesIndex[2].Row), 0, 0))
                .Returns(new XYPosition(new DieReferential(_unsortedDiesIndex[3].Column, _unsortedDiesIndex[3].Row), 0, 0));*/

            Bootstrapper.SimulatedReferentialManager.Setup(_ => _.ConvertTo(It.IsAny<PositionBase>(), ReferentialTag.Stage)).Returns<PositionBase, ReferentialTag>((x, y) => x);
            Bootstrapper.SimulatedReferentialManager.Setup(_ => _.ConvertTo(It.IsAny<PositionBase>(), ReferentialTag.Wafer)).Returns<PositionBase, ReferentialTag>((x, y) => x);
            Bootstrapper.SimulatedReferentialManager.Setup(_ => _.ConvertTo(It.IsAny<PositionBase>(), ReferentialTag.Motor)).Returns<PositionBase, ReferentialTag>((x, y) => x);

            SimulatedChuck.Object.Configuration.IsOpenChuck = false;
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present); 
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_without_dies_without_measure()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateEmptyRecipe();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = new List<RecipeMeasure>();

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            Assert.IsNotNull(sortedMeasurePoint);
            Assert.IsTrue(sortedMeasurePoint.Count == 0);
        }

        [TestMethod]
        public void Sort_recipe_per_point_without_dies_without_measure()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateEmptyRecipe();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = new List<RecipeMeasure>();

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            Assert.IsNotNull(sortedMeasurePoint);
            Assert.IsTrue(sortedMeasurePoint.Count == 0);
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_without_dies_with_one_measure()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithOneMeasureWithoutDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(1)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_without_dies_with_one_measure()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithOneMeasureWithoutDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(1)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_with_one_measure()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithOneMeasureWithDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(4)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_with_one_measure()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithOneMeasureWithDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(4)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_without_dies_with_multiple_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresWithoutDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(3)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV 2" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_without_dies_with_multiple_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresWithoutDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(3)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV 2" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_without_dies_with_multiple_measures_with_multiple_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithoutDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(6)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = null, PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "TSV 2" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_without_dies_with_multiple_measures_with_multiple_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithoutDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(6)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = null, PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "TSV 2" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_with_multiple_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresWithDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(8)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_with_multiple_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresWithDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(8)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_with_multiple_measures_with_muliple_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(12)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_with_multiple_measures_with_muliple_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDies();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(12)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWafer();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(10)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "TSV 2" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWafer();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(10)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "TSV 2" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_duplicate_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndDuplicatePointsWithDiesAndWafer();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(21)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "TSV 2" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = null, PointId = 8, MeasureName = "TSV 3" })
                .And.HaveElementAt(19, new RecipeSortedPoints() { DieIndex = null, PointId = 6, MeasureName = "TSV 4" })
                .And.HaveElementAt(20, new RecipeSortedPoints() { DieIndex = null, PointId = 7, MeasureName = "TSV 4" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_duplicate_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndDuplicatePointsWithDiesAndWafer();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(21)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = null, PointId = 6, MeasureName = "TSV 4" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "TSV 2" })
                .And.HaveElementAt(19, new RecipeSortedPoints() { DieIndex = null, PointId = 7, MeasureName = "TSV 4" })
                .And.HaveElementAt(20, new RecipeSortedPoints() { DieIndex = null, PointId = 8, MeasureName = "TSV 3" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_clamp_and_unclamp_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithClampAndUnclamp();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(10)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "Bow" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_clamp_and_unclamp_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithClampAndUnclamp();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(10)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 1, MeasureName = "TSV" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "TSV" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = null, PointId = 4, MeasureName = "TSV 2" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = null, PointId = 5, MeasureName = "Bow" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMove();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(11)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMove();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(11)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_measures_with_duplicate_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMoveWithDuplicatesPoints();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(19)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_measures_with_duplicate_points()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMoveWithDuplicatesPoints();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(19)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_measures_with_same_z_positions()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMoveWithSameZPositions();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(19)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_measures_with_same_z_positions()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMoveWithSameZPositions();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(19)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 9, MeasureName = "Thickness 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" });
        }

        [TestMethod]
        public void Sort_recipe_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_with_clamp_and_unclamp_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferAndDuplicatePointsWithCantAndCanZMoveWithClampAndUnclamp();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(53)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(19, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 7, MeasureName = "Thickness 1" })

                .And.HaveElementAt(20, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(21, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(22, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(23, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(24, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(25, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(26, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(27, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(28, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(29, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(30, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(31, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(32, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(33, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(34, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(35, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(36, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(37, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(38, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(39, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(40, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(41, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(42, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(43, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(44, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(45, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(46, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(47, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 18, MeasureName = "Thickness 2" })

                .And.HaveElementAt(48, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(49, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(50, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(51, new RecipeSortedPoints() { DieIndex = null, PointId = 14, MeasureName = "Bow 1" })
                .And.HaveElementAt(52, new RecipeSortedPoints() { DieIndex = null, PointId = 9, MeasureName = "Bow" });
        }

        [TestMethod]
        public void Sort_recipe_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_with_clamp_and_unclamp_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferAndDuplicatePointsWithCantAndCanZMoveWithClampAndUnclamp();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(53)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(19, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 5, MeasureName = "Thickness 1" })

                .And.HaveElementAt(20, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(21, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(22, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(23, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(24, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(25, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(26, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 17, MeasureName = "Thickness 2" })

                .And.HaveElementAt(27, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(28, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(29, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(30, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(31, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(32, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(33, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 17, MeasureName = "Thickness 2" })

                .And.HaveElementAt(34, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(35, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(36, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(37, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(38, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(39, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(40, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 17, MeasureName = "Thickness 2" })

                .And.HaveElementAt(41, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(42, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(43, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(44, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(45, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(46, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(47, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 17, MeasureName = "Thickness 2" })

                .And.HaveElementAt(48, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(49, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(50, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(51, new RecipeSortedPoints() { DieIndex = null, PointId = 9, MeasureName = "Bow" })
                .And.HaveElementAt(52, new RecipeSortedPoints() { DieIndex = null, PointId = 14, MeasureName = "Bow 1" });
        }

        [TestMethod]
        public void Sort_recipe_route_not_per_die_per_measurement_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_with_clamp_and_unclamp_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferAndDuplicatePointsWithCantAndCanZMoveWithClampAndUnclamp();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures, false, false, false);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(53)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(19, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 7, MeasureName = "Thickness 1" })

                .And.HaveElementAt(20, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(21, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(22, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(23, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(24, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(25, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(26, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(27, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(28, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(29, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(30, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(31, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(32, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(33, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(34, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(35, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(36, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(37, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(38, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(39, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(40, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(41, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(42, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(43, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(44, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(45, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(46, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(47, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 18, MeasureName = "Thickness 2" })

                .And.HaveElementAt(48, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(49, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(50, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(51, new RecipeSortedPoints() { DieIndex = null, PointId = 14, MeasureName = "Bow 1" })
                .And.HaveElementAt(52, new RecipeSortedPoints() { DieIndex = null, PointId = 9, MeasureName = "Bow" });
        }

        [TestMethod]
        public void Sort_recipe_route_not_per_die_per_point_with_dies_and_wafer_with_multiple_measures_with_muliple_points_with_can_and_cant_z_move_with_clamp_and_unclamp_measures()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferAndDuplicatePointsWithCantAndCanZMoveWithClampAndUnclamp();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures, false, false, false);

            // Then
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(53)
                .And.HaveElementAt(0, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(1, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(2, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(3, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 3, MeasureName = "Thickness" })
                .And.HaveElementAt(4, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(5, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(6, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(7, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 4, MeasureName = "Thickness" })
                .And.HaveElementAt(8, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(9, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(10, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(11, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 6, MeasureName = "Thickness 1" })
                .And.HaveElementAt(12, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(13, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(14, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(15, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 7, MeasureName = "Thickness 1" })
                .And.HaveElementAt(16, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(17, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(18, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 5, MeasureName = "Thickness 1" })
                .And.HaveElementAt(19, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 5, MeasureName = "Thickness 1" })

                .And.HaveElementAt(20, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(21, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(22, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(23, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 16, MeasureName = "Thickness 2" })
                .And.HaveElementAt(24, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(25, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(26, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(27, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 10, MeasureName = "TSV 2" })
                .And.HaveElementAt(28, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(29, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(30, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(31, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 12, MeasureName = "TSV 3" })
                .And.HaveElementAt(32, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(33, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(34, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(35, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 18, MeasureName = "Thickness 2" })
                .And.HaveElementAt(36, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(37, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(38, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(39, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 11, MeasureName = "TSV 2" })
                .And.HaveElementAt(40, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(41, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(42, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(43, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 13, MeasureName = "TSV 3" })
                .And.HaveElementAt(44, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[0], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(45, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[1], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(46, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[2], PointId = 17, MeasureName = "Thickness 2" })
                .And.HaveElementAt(47, new RecipeSortedPoints() { DieIndex = _unsortedDiesIndex[3], PointId = 17, MeasureName = "Thickness 2" })

                .And.HaveElementAt(48, new RecipeSortedPoints() { DieIndex = null, PointId = 0, MeasureName = "TSV" })
                .And.HaveElementAt(49, new RecipeSortedPoints() { DieIndex = null, PointId = 2, MeasureName = "TSV 1" })
                .And.HaveElementAt(50, new RecipeSortedPoints() { DieIndex = null, PointId = 1, MeasureName = "TSV 1" })
                .And.HaveElementAt(51, new RecipeSortedPoints() { DieIndex = null, PointId = 9, MeasureName = "Bow" })
                .And.HaveElementAt(52, new RecipeSortedPoints() { DieIndex = null, PointId = 14, MeasureName = "Bow 1" });
        }

        [TestMethod]
        public void Sort_recipe_exec_time_test_route_per_measurement()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateBigRecipe();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);
            stopwatch.Stop();

            // Then
            // We dont want to check elapsed time with assert since we dont know computing power of the system running the test
            System.Console.WriteLine("Elapsed ms = " + stopwatch.ElapsedMilliseconds);
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(501050);
        }

        [TestMethod]
        public void Sort_recipe_exec_time_test_route_per_point()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateBigRecipe();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures);
            stopwatch.Stop();

            // Then
            // We dont want to check elapsed time with assert since we dont know computing power of the system running the test
            System.Console.WriteLine("Elapsed ms = " + stopwatch.ElapsedMilliseconds);
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(501050);
        }

        [TestMethod]
        public void Sort_recipe_exec_time_test_route_not_per_die_per_measurement()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateBigRecipe();
            recipe.Execution.Strategy = MeasurementStrategy.PerMeasurementType;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures, false, false, false);
            stopwatch.Stop();

            // Then
            // We dont want to check elapsed time with assert since we dont know computing power of the system running the test
            System.Console.WriteLine("Elapsed ms = " + stopwatch.ElapsedMilliseconds);
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(501050);
        }

        [TestMethod]
        public void Sort_recipe_exec_time_test_route_not_per_die_per_point()
        {
            // Given
            var recipePointSorter = new ANARecipeMeasurePointsSorter();
            var recipe = CreateBigRecipe();
            recipe.Execution.Strategy = MeasurementStrategy.PerPoint;
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute(recipe);

            // When
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var sortedMeasurePoint = recipePointSorter.SortRecipeMeasurePoints(recipe, recipeMeasures, false, false, false);
            stopwatch.Stop();

            // Then
            // We dont want to check elapsed time with assert since we dont know computing power of the system running the test
            System.Console.WriteLine("Elapsed ms = " + stopwatch.ElapsedMilliseconds);
            sortedMeasurePoint.Should().NotBeNull()
                .And.HaveCount(501050);
        }

        private List<RecipeMeasure> GetRecipeMeasuresToExecute(ANARecipe recipe)
        {
            var measureLoader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());

            var measuresToExecute = recipe.Measures.Where(m => m.IsActive).ToList();
            var recipeMeasures = new List<RecipeMeasure>();
            foreach (var measureSettings in measuresToExecute)
            {
                IMeasure measure = measureLoader.GetMeasure(measureSettings.MeasureType);
                if (measure is null)
                {
                    continue;
                }

                recipeMeasures.Add(new RecipeMeasure
                {
                    Measure = measure,
                    Settings = measureSettings,
                    MeasurePointIds = new HashSet<int>(measureSettings.MeasurePoints)
                });
            }
            return recipeMeasures;
        }

        private ANARecipe CreateEmptyRecipe()
        {
            var recipe = new ANARecipe
            {
                Points = new List<MeasurePoint>(),
                Measures = new List<MeasureSettingsBase>(),
                Execution = new ExecutionSettings()
            };
            return recipe;
        }

        private ANARecipe CreateRecipeWithOneMeasureWithoutDies()
        {
            var recipe = CreateEmptyRecipe();
            recipe.Points.Add(new MeasurePoint(0, 0, 0, false));
            recipe.Measures.Add(new TSVSettings()
            {
                IsActive = true,
                MeasurePoints = new List<int> { 0 },
                Name = "TSV",
            });

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresWithoutDies()
        {
            var recipe = CreateEmptyRecipe();
            recipe.Points.Add(new MeasurePoint(0, 0, 0, false));
            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV 1",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV 2",
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithoutDies()
        {
            var recipe = CreateEmptyRecipe();
            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, false),
                new MeasurePoint(1, 1, 1, false),
                new MeasurePoint(2, 2, 2, false),
                new MeasurePoint(3, 3, 3, false),
                new MeasurePoint(4, 4, 4, false),
                new MeasurePoint(5, 5, 5, false),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0, 1, 2 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 3 },
                    Name = "TSV 1",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 4, 5 },
                    Name = "TSV 2",
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresWithDies()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points.Add(new MeasurePoint(0, 0, 0, true));
            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV 1",
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithDies()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, true),
                new MeasurePoint(1, 1, 1, true),
                new MeasurePoint(2, 2, 2, true),
                new MeasurePoint(3, 3, 3, true),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 3 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0, 1 },
                    Name = "TSV 1",
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWafer()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, true),
                new MeasurePoint(1, 1, 1, true),
                new MeasurePoint(2, 2, 2, true),
                new MeasurePoint(3, 3, 3, true),
                new MeasurePoint(4, 4, 4, false),
                new MeasurePoint(5, 5, 5, false),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 1, 3 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 4, 5 },
                    Name = "TSV 2",
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndDuplicatePointsWithDiesAndWafer()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, true),
                new MeasurePoint(1, 1, 1, true),
                new MeasurePoint(2, 0, 0, true),
                new MeasurePoint(3, 1, 1, true),
                new MeasurePoint(4, 4, 4, false),
                new MeasurePoint(5, 5, 5, false),
                new MeasurePoint(6, 4, 4, false),
                new MeasurePoint(7, 5, 5, false),
                new MeasurePoint(8, 8, 8, false),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0, 1 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 2, 3 },
                    Name = "TSV 1",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 4, 5 },
                    Name = "TSV 2",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 8 },
                    Name = "TSV 3",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 6, 7 },
                    Name = "TSV 4",
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithClampAndUnclamp()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, true),
                new MeasurePoint(1, 1, 1, true),
                new MeasurePoint(2, 2, 2, true),
                new MeasurePoint(3, 3, 3, true),
                new MeasurePoint(4, 4, 4, false),
                new MeasurePoint(5, 5, 5, false),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 1, 3 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 4 },
                    Name = "TSV 2",
                },
                new BowSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 5 },
                    Name = "Bow",
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMove()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, false),
                new MeasurePoint(1, 1, 1, false),
                new MeasurePoint(2, 0, 0, false),
                new MeasurePoint(3, 3, 3, true),
                new MeasurePoint(4, 4, 4, true),
                new MeasurePoint(5, 5, 5, true),
                new MeasurePoint(6, 6, 6, true),
                new MeasurePoint(7, 6, 6, true),
                new MeasurePoint(8, 6, 6, true),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 1, 2 },
                    Name = "TSV 1",
                },
                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 3, 4 },
                    Name = "Thickness",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings()}},
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMoveWithDuplicatesPoints()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, false),
                new MeasurePoint(1, 1, 1, false),
                new MeasurePoint(2, 0, 0, false),
                new MeasurePoint(3, new XYZTopZBottomPosition(null, 3, 3, 3, 3), true),
                new MeasurePoint(4, new XYZTopZBottomPosition(null, 4, 4, 4, 4), true),
                new MeasurePoint(5, 5, 5, true),
                new MeasurePoint(6, 6, 6, true),
                new MeasurePoint(7, 6, 6, true),
                new MeasurePoint(8, 6, 6, true),
                new MeasurePoint(9, new XYZTopZBottomPosition(null, 3, 3, 3, 3), true),
                new MeasurePoint(10, new XYZTopZBottomPosition(null, 4, 4, 4, 4), true),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 1, 2 },
                    Name = "TSV 1",
                },
                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 3, 4 },
                    Name = "Thickness",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings()}},
                },
                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 9, 10 },
                    Name = "Thickness 1",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings()}},
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferWithCantAndCanZMoveWithSameZPositions()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, false),
                new MeasurePoint(1, 1, 1, false),
                new MeasurePoint(2, 0, 0, false),
                new MeasurePoint(3, new XYZTopZBottomPosition(null, 3, 3, 3, 3), true),
                new MeasurePoint(4, new XYZTopZBottomPosition(null, 4, 4, 4, 4), true),
                new MeasurePoint(5, 5, 5, true),
                new MeasurePoint(6, 6, 6, true),
                new MeasurePoint(7, 6, 6, true),
                new MeasurePoint(8, 6, 6, true),
                new MeasurePoint(9, new XYZTopZBottomPosition(null, 9, 9, 3, 3), true),
                new MeasurePoint(10, new XYZTopZBottomPosition(null, 9, 9, 4, 4), true),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 1, 2 },
                    Name = "TSV 1",
                },
                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 3, 4 },
                    Name = "Thickness",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings()}},
                },
                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 9, 10 },
                    Name = "Thickness 1",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings()}},
                },
            };

            return recipe;
        }

        private ANARecipe CreateRecipeWithMultipleMeasuresAndPointsWithDiesAndWaferAndDuplicatePointsWithCantAndCanZMoveWithClampAndUnclamp()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);

            recipe.Points = new List<MeasurePoint>()
            {
                new MeasurePoint(0, 0, 0, false),
                new MeasurePoint(1, 1, 1, false),
                new MeasurePoint(2, 0, 0, false),
                new MeasurePoint(3, 3, 3, true),
                new MeasurePoint(4, 4, 4, true),
                new MeasurePoint(5, 5, 5, true),
                new MeasurePoint(6, 3, 3, true),
                new MeasurePoint(7, 4, 4, true),
                new MeasurePoint(8, 8, 8, true),
                new MeasurePoint(9, 9, 9, false),
                new MeasurePoint(10, 10, 10, true),
                new MeasurePoint(11, 11, 11, true),
                new MeasurePoint(12, 10, 10, true),
                new MeasurePoint(13, 11, 11, true),
                new MeasurePoint(14, 14, 14, false),
                new MeasurePoint(15, 15, 15, true),
                new MeasurePoint(16, 5, 5, true),
                new MeasurePoint(17, 11, 11, true),
                new MeasurePoint(18, 10, 10, true),
            };

            recipe.Measures = new List<MeasureSettingsBase>()
            {
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 0 },
                    Name = "TSV",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 1, 2 },
                    Name = "TSV 1",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 10, 11 },
                    Name = "TSV 2",
                },
                new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 12, 13 },
                    Name = "TSV 3",
                },
                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 3, 4 },
                    Name = "Thickness",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings()}},
                },
                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 5, 6, 7 },
                    Name = "Thickness 1",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings()}},
                },

                new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 16, 17, 18 },
                    Name = "Thickness 2",
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new SingleLiseSettings()}},
                },
                new BowSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 14 },
                    Name = "Bow 1",
                },
                new BowSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { 9 },
                    Name = "Bow",
                },
            };

            return recipe;
        }

        // Create recipe with :
        // 500 Dies
        // 1000 measures points (500 wafer ref + 500 die ref) with duplicates
        // 50 measures ( 10 TSV die ref, 10 TSV wafer ref, 10 Thickness die ref, 10 Thickness wafer ref, 10 Bow wafer ref)
        // 50 measurePoint for each measure
        //  (TSV + Thickness)dieref   (TSV + Thickness)wafer ref    Bow
        // =      500*20*50                  +20*50                 + 50 = 501050 measures
        // if we consider that a measurement takes 10 seconds, this big recipe will last ~58 days
        private ANARecipe CreateBigRecipe()
        {
            var recipe = CreateEmptyRecipe();
            recipe.Dies = new List<DieIndex>();
            recipe.WaferMap = new WaferMapSettings();

            // Create 500 dies
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    recipe.Dies.Add(new DieIndex(i, j));
                }
            }

            // Create 500 measure points in die referential with duplicates
            for (int i = 0; i < 500; i++)
            {
                recipe.Points.Add(new MeasurePoint(i, i % 100, i % 100, true));
            }

            // Create 500 measure points in wafer referential with duplicates
            for (int i = 500; i < 1000; i++)
            {
                recipe.Points.Add(new MeasurePoint(i, i % 100, i % 100, false));
            }

            // Create 10 measures TSV with 50 measure points each in die ref
            for (int i = 0; i < 10; i++)
            {
                var measure = new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int>(),
                    Name = "TSVDieRef " + i,
                };

                for (int j = 0; j < 50; j++)
                {
                    measure.MeasurePoints.Add(i * 50 + j);
                }
                recipe.Measures.Add(measure);
            }

            // Create 10 measures TSV with 50 measure points each in wafer ref
            for (int i = 0; i < 10; i++)
            {
                var measure = new TSVSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int>(),
                    Name = "TSVWaferRef " + i,
                };

                for (int j = 500; j < 550; j++)
                {
                    measure.MeasurePoints.Add(i * 50 + j);
                }
                recipe.Measures.Add(measure);
            }

            // Create 10 measures thickness with dual lise (=cantZMove) with 50 measure points each in die ref
            for (int i = 0; i < 10; i++)
            {
                var measure = new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int>(),
                    Name = "ThicknessDieRef " + i,
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings() } },
                };

                for (int j = 0; j < 50; j++)
                {
                    measure.MeasurePoints.Add(i * 50 + j);
                }
                recipe.Measures.Add(measure);
            }

            // Create 10 measures thickness with dual lise (=cantZMove) with 10 measure points each in wafer ref
            for (int i = 0; i < 10; i++)
            {
                var measure = new ThicknessSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int>(),
                    Name = "ThicknessWaferRef " + i,
                    LayersToMeasure = new List<Layer> { new Layer() { ProbeSettings = new DualLiseSettings() } },
                };

                for (int j = 500; j < 550; j++)
                {
                    measure.MeasurePoints.Add(i * 50 + j);
                }
                recipe.Measures.Add(measure);
            }

            // Create 10 measures bow (=unclamp) in wafer ref
            for (int i = 500; i < 550; i++)
            {
                var measure = new BowSettings()
                {
                    IsActive = true,
                    MeasurePoints = new List<int> { i },
                    Name = "Bow" + i,
                };

                recipe.Measures.Add(measure);
            }

            return recipe;
        }

        private ANARecipe CreateRecipeWithOneMeasureWithDies()
        {
            var recipe = CreateEmptyRecipe();
            AddRecipeWaferMap(recipe);
            recipe.Points.Add(new MeasurePoint(0, 0, 0, true));
            recipe.Measures.Add(new TSVSettings()
            {
                IsActive = true,
                MeasurePoints = new List<int> { 0 },
                Name = "TSV",
            });

            return recipe;
        }

        private void AddRecipeWaferMap(ANARecipe recipe)
        {
            recipe.Dies = _unsortedDiesIndex;
            recipe.WaferMap = new WaferMapSettings();
        }
    }
}
