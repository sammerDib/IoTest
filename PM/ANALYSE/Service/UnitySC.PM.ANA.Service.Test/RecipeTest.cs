using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.TestUtils.Context;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using LayerSettings = UnitySC.PM.ANA.Service.Interface.Recipe.Measure.LayerSettings;

namespace UnitySC.PM.ANA.Service.Test
{
    [TestClass]
    public class RecipeTest
    {
        private const string RecipeComment = "TestComment";
        private const string RecipeName = "TestRecipeName";
        private const int StepId = 10;
        private const int UserId = 20;
        private const string ThicknessConfigurationName = "Thickness";
        private const string TSVConfigurationName = "TSV";
        private const string LiseAFObjectiveId = "Objective";
        private Container _container;

        [TestInitialize]
        public void Init()
        {
            _container = new Container();
            Bootstrapper.Register(_container);

        }

        [TestMethod]
        public void MD5HashTest()
        {
            var fileBuf = File.ReadAllBytes(@"TestExternalFile.png");
            var hashExpected = "2b5f1f267487fc01758bd10b72e37f88";

            var hash = Md5.ComputeHash(fileBuf);
            Assert.AreEqual(hash.Length, 32, "MD5 hash string should be 32 in size");
            Assert.AreEqual(hash, hashExpected, "Not same hash expected");
        }


        [TestMethod]
        public void RecipeMapperTest()
        {
            var recipeService = ClassLocator.Default.GetInstance<ANARecipeService>();
            var _mapper = ClassLocator.Default.GetInstance<Mapper>();
            var newRecipe = recipeService.CreateRecipe(RecipeName, StepId, UserId)?.Result;
            var key = newRecipe.Key;
            newRecipe.Comment = RecipeComment;

            // Alignment settings
            newRecipe.Alignment = new AlignmentSettings();
            newRecipe.Alignment.AutoFocusLise = new AutoFocusLiseParameters();
            newRecipe.Alignment.AutoFocusLise.ZIsDefinedByUser = true;
            newRecipe.Alignment.AutoFocusLise.ZTopFocus = new Length(10, LengthUnit.Millimeter);
            newRecipe.Alignment.AutoFocusLise.LiseObjectiveContext = new ObjectiveContext(LiseAFObjectiveId);

            newRecipe.Alignment.AutoLight = new AutoLightParameters();
            newRecipe.Alignment.AutoLight.LightIntensityIsDefinedByUser = false;
            newRecipe.Alignment.AutoLight.LightIntensity = 5;
            newRecipe.Alignment.AutoLight.AnaPositionContext = AnaPositionContextFactory.Build();
            newRecipe.Alignment.AutoLight.ObjectiveContext = ObjectiveContextFactory.Build();

            newRecipe.Alignment.BareWaferAlignment = new BareWaferAlignmentParameters();
            newRecipe.Alignment.BareWaferAlignment.CustomImagePositions = new List<BareWaferAlignmentImagePosition>()
            {
                new BareWaferAlignmentImagePosition(){EdgePosition = WaferEdgePositions.Bottom, X = new Length(10, LengthUnit.Millimeter), Y = new Length(20, LengthUnit.Millimeter) }
            };

            // Excecution settings
            newRecipe.Execution = new ExecutionSettings();
            newRecipe.Execution.Alignment = new AlignmentParameters();
            newRecipe.Execution.Alignment.RunAutoFocus = false;
            newRecipe.Execution.Alignment.RunAutoLight = true;
            newRecipe.Execution.Alignment.RunBwa = true;
            newRecipe.Execution.Alignment.RunMarkAlignment = false;

            // Measure
            var point = new MeasurePoint(1, 20, 30, false);
            var thicknessConfiguration = new ThicknessSettings();
            thicknessConfiguration.Name = ThicknessConfigurationName;
            thicknessConfiguration.MeasurePoints.Add(point.Id);

            double reflexionIndex = 1.43;
            var layer750 = new LayerSettings()
            {
                Thickness = 750.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
            var layersListFor750 = new List<LayerSettings>() { layer750, };
            var probeSettings = new SingleLiseSettings()
            {
                ProbeId = "ProbeLiseUp",
                LiseGain = 3,
                ProbeObjectiveContext = new ObjectiveContext
                {
                    ObjectiveId = "ObjectiveSelector02"
                }
            };
            var layersToMeasure = new List<Interface.Recipe.Measure.MeasureSettings.Layer>();
            var layerToMeasure = new Interface.Recipe.Measure.MeasureSettings.Layer()
            {
                Name = "MeasurableLayers to measure 1",
                PhysicalLayers = layersListFor750,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
            layersToMeasure.Add(layerToMeasure);
            thicknessConfiguration.LayersToMeasure = layersToMeasure;
            thicknessConfiguration.PhysicalLayers = layersListFor750;
            thicknessConfiguration.Name = "Thickness";

            // TSV
            var tsvConfiguration = new TSVSettings();
            tsvConfiguration.Name = TSVConfigurationName;
            tsvConfiguration.MeasurePoints.Add(point.Id);
            tsvConfiguration.DepthTarget = 100.Micrometers();
            tsvConfiguration.DepthTolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer);
            tsvConfiguration.LengthTarget = 100.Micrometers();
            tsvConfiguration.LengthTolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer);
            tsvConfiguration.Strategy = TSVAcquisitionStrategy.Standard;
            tsvConfiguration.Precision = TSVMeasurePrecision.Fast;
            tsvConfiguration.WidthTarget = 100.Micrometers();
            tsvConfiguration.WidthTolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer);

            newRecipe.Measures.Add(thicknessConfiguration);
            newRecipe.Measures.Add(tsvConfiguration);
            newRecipe.Points.Add(point);

            // Map to Data Access recipe
            var dbrecipe = _mapper.AutoMap.Map<Recipe>(newRecipe);
            //File.WriteAllText(@"c:\temp\AnaRecipe.xml", dbrecipe.XmlContent);

            // Map to ANA Recipe
            var mapRecipe = _mapper.AutoMap.Map<ANARecipe>(dbrecipe);

            // Assert Main recipe fields
            Assert.AreEqual(RecipeName, mapRecipe.Name);
            Assert.AreEqual(RecipeComment, mapRecipe.Comment);
            Assert.AreEqual(StepId, mapRecipe.StepId);
            Assert.AreEqual(UserId, mapRecipe.UserId);
            Assert.AreEqual(key, mapRecipe.Key);

            // Assert Alignment settings
            Assert.AreEqual(true, mapRecipe.Alignment.AutoFocusLise.ZIsDefinedByUser);
            Assert.AreEqual(new Length(10, LengthUnit.Millimeter), newRecipe.Alignment.AutoFocusLise.ZTopFocus);
            Assert.IsNotNull(newRecipe.Alignment.AutoFocusLise.LiseObjectiveContext);
            Assert.AreEqual(LiseAFObjectiveId, newRecipe.Alignment.AutoFocusLise.LiseObjectiveContext.ObjectiveId);
            Assert.AreEqual(false, mapRecipe.Alignment.AutoLight.LightIntensityIsDefinedByUser);
            Assert.AreEqual(5, mapRecipe.Alignment.AutoLight.LightIntensity);
            mapRecipe.Alignment.AutoLight.AnaPositionContext.Should().BeEquivalentTo(newRecipe.Alignment.AutoLight.AnaPositionContext);
            mapRecipe.Alignment.AutoLight.ObjectiveContext.Should().BeEquivalentTo(newRecipe.Alignment.AutoLight.ObjectiveContext);
            Assert.AreEqual(1, mapRecipe.Alignment.BareWaferAlignment.CustomImagePositions.Count);
            var CustomImagePosition = mapRecipe.Alignment.BareWaferAlignment.CustomImagePositions.First();
            Assert.AreEqual(new Length(10, LengthUnit.Millimeter), CustomImagePosition.X);
            Assert.AreEqual(new Length(20, LengthUnit.Millimeter), CustomImagePosition.Y);
            Assert.AreEqual(WaferEdgePositions.Bottom, CustomImagePosition.EdgePosition);

            // Assert Execution settings
            Assert.AreEqual(false, mapRecipe.Execution.Alignment.RunAutoFocus);
            Assert.AreEqual(true, newRecipe.Execution.Alignment.RunAutoLight);
            Assert.AreEqual(true, mapRecipe.Execution.Alignment.RunBwa);
            Assert.AreEqual(false, mapRecipe.Execution.Alignment.RunMarkAlignment);

            // Assert Points
            Assert.AreEqual(1, mapRecipe.Points.Count);
            var mapPoint = mapRecipe.Points.First();
            Assert.AreEqual(point.Id, mapPoint.Id);
            Assert.AreEqual(point.Position.X, mapPoint.Position.X);
            Assert.AreEqual(point.Position.Y, mapPoint.Position.Y);

            // Assert Measures
            Assert.AreEqual(2, mapRecipe.Measures.Count);
            var mapThickness = mapRecipe.Measures.OfType<ThicknessSettings>().FirstOrDefault();
            Assert.IsNotNull(mapThickness);
            Assert.AreEqual(ThicknessConfigurationName, mapThickness.Name);
        }

        [TestMethod]
        public void Go()
        {
            var ctx = ObjectivesContextFactory.Build();
            string x = XML.SerializeToString(ctx);
            var result = XML.DeserializeFromString<ObjectivesContext>(x);
            result.Should().BeEquivalentTo(ctx);
        }
    }
}
