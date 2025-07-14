using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    /// <summary>
    /// Summary description for ThicknessTest
    /// </summary>
    [TestClass]
    public class ThicknessTest
    {
        private const int NBLayers = 3;

        [TestMethod]
        public void TestThicknessFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.anathick";
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ANALYSE_Thickness, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void ReadWriteThicknessBlankWaferResult()
        {
            var thicknessResult = new ThicknessResult();
            thicknessResult.Name = "ThicknessTest1";
            var rnd = new Random();

            var layersettings = new List<ThicknessLengthSettings>(NBLayers);
            var measureLayers = new List<string>();
            int nk = 0;
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"Layer {++nk}",
                Target = new Length(250, LengthUnit.Micrometer),
                Tolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer),
                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = true
            });
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"Layer {++nk}",
                Target = new Length(500, LengthUnit.Nanometer),
                Tolerance = new LengthTolerance(100, LengthToleranceUnit.Nanometer),
                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = true
            });
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"Layer {++nk}",
                Target = new Length(125, LengthUnit.Micrometer),
                Tolerance = new LengthTolerance(8, LengthToleranceUnit.Micrometer),
                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = true
            });
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"LayerNotMeasured{++nk}",
                Target = new Length(105, LengthUnit.Micrometer),
                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = false
            });

            thicknessResult.Settings.HasWarpMeasure = false;
            thicknessResult.Settings.HasWaferThicknesss = true;
            thicknessResult.Settings.ThicknessLayers = layersettings;
            thicknessResult.Settings.TotalTarget = 480.Micrometers();
            thicknessResult.Settings.TotalTolerance = new LengthTolerance(20, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            thicknessResult.Settings.ComputeNotMeasuredLayers();

            Assert.AreEqual(layersettings.Last().Target.Micrometers, thicknessResult.Settings.TotalNotMeasuredLayers.Value, $"value TotalNotMeasuredLayers");

            thicknessResult.Information = "Test blank wafer result";
            thicknessResult.State = (GlobalState)rnd.Next(0, 5);
            thicknessResult.Wafer = MetroHelper.CreateWafer();
            thicknessResult.Points = new List<MeasurePointResult>();

            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));


            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_Thickness);
            metroResWrite.MeasureResult = thicknessResult;

            // Test WCF Serialize
            XML.DatacontractSerialize(thicknessResult, "ThicknessBlankWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<ThicknessResult>("ThicknessBlankWafer.testwcf");

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"ThicknessBlankWafer.xml", out error), "Bad file name extension for Thickness results");

            bool bRes = metroResWrite.WriteInFile(@"ThicknessBlankWafer.anathick", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_Thickness);
            metroRes.ReadFromFile(@"ThicknessBlankWafer.anathick", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as ThicknessResult;
            AssertThicknessResult(thicknessResult, resXML, "XML Serialize");

            AssertThicknessResult(thicknessResult, resWCF, "WCF Serialize");
        }

        [TestMethod]
        public void ReadWriteThicknessBlankWaferResult_WITH_WarpResult()
        {
            var thicknessResult = new ThicknessResult();
            thicknessResult.Name = "ThicknessTest2";
            var rnd = new Random();

            var layersettings = new List<ThicknessLengthSettings>(NBLayers);
            var measureLayers = new List<string>();
            int nk = 0;
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"Layer {++nk}",
                Target = new Length(250, LengthUnit.Micrometer),
                Tolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer),
                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = true
            }); ; ;
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"Layer {++nk}",
                Target = new Length(500, LengthUnit.Nanometer),
                Tolerance = new LengthTolerance(100, LengthToleranceUnit.Nanometer),
                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = true
            }); ;
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"Layer {++nk}",
                Target = new Length(125, LengthUnit.Micrometer),
                Tolerance = new LengthTolerance(8, LengthToleranceUnit.Micrometer),

                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = true
            });
            layersettings.Add(new ThicknessLengthSettings()
            {
                Name = $"LayerNotMeasured{++nk}",
                Target = new Length(105, LengthUnit.Micrometer),
                LayerColor = Color.FromArgb((byte)rnd.Next(150, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)),
                IsMeasured = false
            });

            thicknessResult.Settings.HasWarpMeasure = true;
            thicknessResult.Settings.WarpTargetMax = 175.Micrometers();
            thicknessResult.Settings.HasWaferThicknesss = true;
            thicknessResult.Settings.ThicknessLayers = layersettings;
            thicknessResult.Settings.TotalTarget = 480.Micrometers();
            thicknessResult.Settings.TotalTolerance = new LengthTolerance(20, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            thicknessResult.Settings.ComputeNotMeasuredLayers();

            Assert.AreEqual(layersettings.Last().Target.Micrometers, thicknessResult.Settings.TotalNotMeasuredLayers.Value, $"value TotalNotMeasuredLayers");

            thicknessResult.Information = "Test blank wafer result with warp";
            thicknessResult.State = (GlobalState)rnd.Next(0, 5);
            thicknessResult.Wafer = MetroHelper.CreateWafer();
            thicknessResult.Points = new List<MeasurePointResult>();

            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));
            thicknessResult.Points.Add(CreateTestThicknessPointResult(thicknessResult.Settings));

            thicknessResult.WarpWaferResults = thicknessResult.ComputeWarpFromRPD();

            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_Thickness);
            metroResWrite.MeasureResult = thicknessResult;

            // Test WCF Serialize
            XML.DatacontractSerialize(thicknessResult, "ThicknessBlankWafer2.testwcf");
            var resWCF = XML.DatacontractDeserialize<ThicknessResult>("ThicknessBlankWafer2.testwcf");

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"ThicknessBlankWafer2.xml", out error), "Bad file name extension for Thickness results");

            bool bRes = metroResWrite.WriteInFile(@"ThicknessBlankWafer2.anathick", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_Thickness);
            metroRes.ReadFromFile(@"ThicknessBlankWafer2.anathick", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as ThicknessResult;
            AssertThicknessResult(thicknessResult, resXML, "XML Serialize");

            AssertThicknessResult(thicknessResult, resWCF, "WCF Serialize");
        }

        private MeasurePointResult CreateTestThicknessPointResult(ThicknessResultSettings settings, bool isInDie = false)
        {
            var point = new ThicknessPointResult();
            if (isInDie)
            {
                // Die 3 * 3 mm
                point.XPosition = (MetroHelper.StaticRandom.Instance.Next(0, 300) * 0.01);
                point.YPosition = (MetroHelper.StaticRandom.Instance.Next(0, 300) * 0.01);
            }
            else
            {
                point.XPosition = (MetroHelper.StaticRandom.Instance.Next(0, 3000) * 0.1) - 150;
                point.YPosition = (MetroHelper.StaticRandom.Instance.Next(0, 3000) * 0.1) - 150;
            }
            point.State = (MeasureState)MetroHelper.StaticRandom.Instance.Next(0, 4);
            var pointData1 = new ThicknessPointData();
            var dt1 = CreateLayersResults(settings);
            pointData1.ThicknessLayerResults = dt1.Item1;
            if (dt1.Item2 != null)
                pointData1.WaferThicknessResult = dt1.Item2;
            pointData1.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;

            if (settings.HasWarpMeasure)
            {
                var warppointData = new WarpPointData();
                warppointData.RPD = (MetroHelper.StaticRandom.Instance.Next(-1500, 1500) * 0.01).Micrometers();
                warppointData.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
                pointData1.WarpResult = warppointData;
            }
            point.Datas.Add(pointData1);

            var pointData2 = new ThicknessPointData();
            var dt2 = CreateLayersResults(settings);
            pointData2.ThicknessLayerResults = dt2.Item1;
            if (dt2.Item2 != null)
                pointData2.WaferThicknessResult = dt2.Item2;
            pointData2.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData2.IndexRepeta = 1;
            if (settings.HasWarpMeasure)
            {
                var warppointData = new WarpPointData();
                warppointData.RPD = (MetroHelper.StaticRandom.Instance.Next(-1500, 1500) * 0.01).Micrometers();
                warppointData.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
                pointData2.WarpResult = warppointData;
            }
            point.Datas.Add(pointData2);

            return point;
        }

        private Tuple<List<ThicknessLengthResult>, ThicknessLengthResult> CreateLayersResults(ThicknessResultSettings settings)
        {
            var res = new List<ThicknessLengthResult>();
            foreach (var layerset in settings.ThicknessLayers)
            {
                if (layerset.IsMeasured)
                {
                    //we assume tolerance is the same unit as target here
                    double dMax = layerset.Target.Value + layerset.Tolerance.Value;
                    double dMin = layerset.Target.Value - layerset.Tolerance.Value;
                    double dValue = MetroHelper.StaticRandom.Instance.NextDouble() * (dMax - dMin) + dMin;
                    res.Add(new ThicknessLengthResult() { Name = layerset.Name, Length = new Length(dValue, layerset.Target.Unit) });
                }
            }
            ThicknessLengthResult waferthickness = null;
            if (settings.HasWaferThicknesss)
            {
                //we assume tolerance is the same unit as target here
                double dMax = settings.TotalTarget.Value + settings.TotalTolerance.Value;
                double dMin = settings.TotalTarget.Value - settings.TotalTolerance.Value;
                double dValue = MetroHelper.StaticRandom.Instance.NextDouble() * (dMax - dMin) + dMin;
                waferthickness = new ThicknessLengthResult() { Name = "WaferThickness", Length = new Length(dValue, settings.TotalTarget.Unit) };
            }
            return new Tuple<List<ThicknessLengthResult>, ThicknessLengthResult>(res, waferthickness);
        }

        private void AssertThicknessResult(ThicknessResult expected, ThicknessResult actual, string testName)
        {
            Assert.AreEqual(expected.Name, actual.Name, $"{testName} Name");
            Assert.AreEqual(expected.Information, actual.Information, $"{testName} Information");
            Assert.AreEqual(expected.State, actual.State, $"{testName} State");

            Assert.AreEqual(expected.Settings.ThicknessLayers.Count, actual.Settings.ThicknessLayers.Count, $"{testName} Settings.ThicknessLayers.Count");
            for (int i = 0; i < expected.Settings.ThicknessLayers.Count; i++)
            {
                Assert.AreEqual(expected.Settings.ThicknessLayers[i].Name, actual.Settings.ThicknessLayers[i].Name, $"{testName} Settings.ThicknessLayers[{i}].Name");
                Assert.AreEqual(expected.Settings.ThicknessLayers[i].Target.ToString(), actual.Settings.ThicknessLayers[i].Target.ToString(), $"{testName} Settings.ThicknessLayers[{i}].Target");
                Assert.AreEqual(expected.Settings.ThicknessLayers[i].Tolerance?.ToString(), actual.Settings.ThicknessLayers[i].Tolerance?.ToString(), $"{testName} Settings.ThicknessLayers[{i}].Tolerance");
                Assert.AreEqual(expected.Settings.ThicknessLayers[i].LayerColor, actual.Settings.ThicknessLayers[i].LayerColor, $"{testName} Settings.ThicknessLayers[{i}].LayerColor");
                Assert.AreEqual(expected.Settings.ThicknessLayers[i].IsMeasured, actual.Settings.ThicknessLayers[i].IsMeasured, $"{testName} Settings.ThicknessLayers[{i}].IsMeasured");
            }
            Assert.AreEqual(expected.Settings.TotalTarget.ToString(), actual.Settings.TotalTarget.ToString(), $"{testName} Settings.TotalTarget");
            Assert.AreEqual(expected.Settings.TotalTolerance.ToString(), actual.Settings.TotalTolerance.ToString(), $"{testName} Settings.TotalTolerance");

            Assert.AreEqual(expected.Settings.HasWaferThicknesss, actual.Settings.HasWaferThicknesss, $"{testName} Settings.HasWaferThicknesss");

            Assert.AreEqual(expected.Settings.HasWarpMeasure, actual.Settings.HasWarpMeasure, $"{testName} Settings.HasWarpMeasure");
            if (expected.Settings.HasWarpMeasure)
            {
                Assert.AreEqual(expected.Settings.WarpTargetMax.ToString(), actual.Settings.WarpTargetMax.ToString(), $"{testName} Settings.WarpTargetMax");
            }
            Assert.AreEqual(expected.Wafer.ToString(), actual.Wafer.ToString(), $"{testName} Wafer");
            MetroHelper.AreEqualAutomInfo(expected.AutomationInfo, actual.AutomationInfo, testName);

            if (expected.Points != null)
            {
                Assert.AreEqual(expected.Points.Count, actual.Points.Count, $"{testName} Points.Count");
                for (int i = 0; i < expected.Points.Count; i++)
                {
                    Assert.AreEqual(expected.Points[i].XPosition, actual.Points[i].XPosition, $"{testName} Points[{i}].XPosition");
                    Assert.AreEqual(expected.Points[i].YPosition, actual.Points[i].YPosition, $"{testName} Points[{i}].YPosition");
                    Assert.AreEqual(expected.Points[i].State, actual.Points[i].State, $"{testName} Points[{i}].State");

                    Assert.AreEqual(expected.Points[i].Datas.Count, actual.Points[i].Datas.Count, $"{testName} Points[i].Datas.Count");
                    for (int j = 0; j < expected.Points[i].Datas.Count; j++)
                    {
                        var expectedData = (expected.Points[i].Datas[j] as ThicknessPointData);
                        var actualData = (actual.Points[i].Datas[j] as ThicknessPointData);

                        Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Points[{i}].Datas[{j}].IndexRepeta ");
                        Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Points[{i}].Datas[{j}].State ");
                        Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Points[{i}].Datas[{j}].QualityScore ");

                        Assert.AreEqual(expectedData.ThicknessLayerResults.Count, actualData.ThicknessLayerResults.Count, $"{testName} Points[{i}].Datas[{j}].ThicknessLayerResults.Count");
                        for (int k = 0; k < expectedData.ThicknessLayerResults.Count; k++)
                        {
                            Assert.AreEqual(expectedData.ThicknessLayerResults[k].Name, actualData.ThicknessLayerResults[k].Name, $"{testName} Points[{i}].Datas[{j}].ThicknessLayerResults[{k}].Name");
                            Assert.AreEqual(expectedData.ThicknessLayerResults[k].Length.ToString(), actualData.ThicknessLayerResults[k].Length.ToString(), $"{testName} Points[{i}].Datas[{j}].ThicknessLayerResults[{k}]");
                        }

                        Assert.IsTrue((expectedData.WaferThicknessResult != null) && (actualData.WaferThicknessResult != null));
                        if (expectedData.WaferThicknessResult != null)
                        {
                            Assert.AreEqual(expectedData.WaferThicknessResult.Name, actualData.WaferThicknessResult.Name, $"{testName} Points[{i}].Datas[{j}].WaferThicknessResult.Name");
                            Assert.AreEqual(expectedData.WaferThicknessResult.Length.ToString(), actualData.WaferThicknessResult.Length.ToString(), $"{testName} Points[{i}].Datas[{j}].WaferThicknessResult.Length");
                        }

                        if (expected.Settings.HasWarpMeasure)
                        {
                            Assert.AreEqual(expectedData.WarpResult.RPD.ToString(), actualData.WarpResult.RPD.ToString(), $"{testName} Points[{i}].Datas[{j}].WarpResult.RPD ");
                        }

                    }
                }
            }

            if (expected.Dies != null)
            {
                Assert.AreEqual(expected.DiesMap.RotationAngle.ToString(), actual.DiesMap.RotationAngle.ToString(), $"{testName} DiesMap RotationAngle");
                Assert.AreEqual(expected.DiesMap.DieSizeWidth.ToString(), actual.DiesMap.DieSizeWidth.ToString(), $"{testName} DiesMap DieSizeWidth");
                Assert.AreEqual(expected.DiesMap.DieSizeHeight.ToString(), actual.DiesMap.DieSizeHeight.ToString(), $"{testName} DiesMap DieSizeHeight");
                Assert.AreEqual(expected.DiesMap.DiePitchWidth.ToString(), actual.DiesMap.DiePitchWidth.ToString(), $"{testName} DiesMap DiePitchWidth");
                Assert.AreEqual(expected.DiesMap.DiePitchHeight.ToString(), actual.DiesMap.DiePitchHeight.ToString(), $"{testName} DiesMap DiePitchHeight");
                Assert.AreEqual(expected.DiesMap.DieGridTopLeftXPosition.ToString(), actual.DiesMap.DieGridTopLeftXPosition.ToString(), $"{testName} DiesMap DieGridTopLeftXPosition");
                Assert.AreEqual(expected.DiesMap.DieGridTopLeftYPosition.ToString(), actual.DiesMap.DieGridTopLeftYPosition.ToString(), $"{testName} DiesMap DieGridTopLeftYPosition");
                Assert.AreEqual(expected.DiesMap.DieReferenceColumnIndex, actual.DiesMap.DieReferenceColumnIndex, $"{testName} DiesMap DieReferenceColumnIndex");
                Assert.AreEqual(expected.DiesMap.DieReferenceRowIndex, actual.DiesMap.DieReferenceRowIndex, $"{testName} DiesMap DieReferenceRowIndex");

                Assert.AreEqual(expected.DiesMap.DiesPresence.Count, actual.DiesMap.DiesPresence.Count, $"{testName} DiesMap.DiesPresence.Count");
                for (int i = 0; i < expected.DiesMap.DiesPresence.Count; i++)
                {
                    Assert.AreEqual(expected.DiesMap.DiesPresence[i].RowIndex, actual.DiesMap.DiesPresence[i].RowIndex, $"{testName} DiesMap.DiesPresence[{i}].RowIndex");
                    Assert.AreEqual(expected.DiesMap.DiesPresence[i].Dies, actual.DiesMap.DiesPresence[i].Dies, $"{testName} DiesMap.DiesPresence[{i}].Dies");
                }

                Assert.AreEqual(expected.Dies.Count, actual.Dies.Count, $"{testName} Dies.Count");
                for (int i = 0; i < expected.Dies.Count; i++)
                {
                    Assert.AreEqual(expected.Dies[i].State, actual.Dies[i].State, $"{testName} Dies[{i}].State");
                    Assert.AreEqual(expected.Dies[i].RowIndex, actual.Dies[i].RowIndex, $"{testName} Dies[{i}].RowIndex");
                    Assert.AreEqual(expected.Dies[i].ColumnIndex, actual.Dies[i].ColumnIndex, $"{testName} Dies[{i}].ColumnIndex");

                    var expdie = expected.Dies[i];
                    var actdie = actual.Dies[i];

                    Assert.AreEqual(expected.Dies[i].Points.Count, actual.Dies[i].Points.Count, $"{testName} Dies[{i}].Points.Count");
                    for (int j = 0; j < expected.Dies[i].Points.Count; j++)
                    {
                        var exp = expected.Dies[i].Points[j];
                        var act = expected.Dies[i].Points[j];

                        Assert.AreEqual(exp.XPosition, act.XPosition, $"{testName} Dies[{i}].Points[{j}].XPosition");
                        Assert.AreEqual(exp.YPosition, act.YPosition, $"{testName} Dies[{i}].Points[{j}].YPosition");
                        Assert.AreEqual(exp.State, act.State, $"{testName} Dies[{i}].Points[{i}].State");

                        Assert.AreEqual(exp.Datas.Count, act.Datas.Count, $"{testName} Dies[{i}].Points[{j}].Datas.Count");
                        for (int k = 0; k < exp.Datas.Count; k++)
                        {
                            var expectedData = (exp.Datas[k] as ThicknessPointData);
                            var actualData = (act.Datas[k] as ThicknessPointData);

                            Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].IndexRepeta ");
                            Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].State ");
                            Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].QualityScore ");

                            Assert.AreEqual(expectedData.ThicknessLayerResults.Count, actualData.ThicknessLayerResults.Count, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].ThicknessLayerResults.Count");
                            for (int kk = 0; kk < expectedData.ThicknessLayerResults.Count; kk++)
                            {
                                Assert.AreEqual(expectedData.ThicknessLayerResults[kk].Name, actualData.ThicknessLayerResults[kk].Name, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].ThicknessLayerResults[{kk}].Name");
                                Assert.AreEqual(expectedData.ThicknessLayerResults[kk].Length.ToString(), actualData.ThicknessLayerResults[kk].Length.ToString(), $"{testName} Dies[{i}].Points[{j}].Datas[{k}].ThicknessLayerResults[{kk}].Value");
                            }

                            Assert.IsTrue((expectedData.WaferThicknessResult != null) && (actualData.WaferThicknessResult != null));
                            if (expectedData.WaferThicknessResult != null)
                            {
                                Assert.AreEqual(expectedData.WaferThicknessResult.Name, actualData.WaferThicknessResult.Name, $"{testName}  Dies[{i}].Points[{j}].Datas[{k}]..WaferThicknessResult.Name");
                                Assert.AreEqual(expectedData.WaferThicknessResult.Length.ToString(), actualData.WaferThicknessResult.Length.ToString(), $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].WaferThicknessResult.Length");
                            }
                        }
                    }
                }
            }

            if (expected.Settings.HasWarpMeasure)
            {
                Assert.IsNotNull(actual.WarpWaferResults, "WarpWaferResults should be not null");
                Assert.IsTrue(actual.WarpWaferResults.Count > 0, "WarpWaferResults should have some results");

                for (int i = 0; i < expected.WarpWaferResults.Count; i++)
                {
                    Assert.AreEqual(expected.WarpWaferResults[i].ToString(), actual.WarpWaferResults[i].ToString(), $"{testName} WarpWaferResults[{i}]");
                }
            }
            else
            {
                Assert.IsTrue(actual.WarpWaferResults == null || actual.WarpWaferResults?.Count == 0, "WarpWaferResults should have no results");

            }
        }
    }
}
