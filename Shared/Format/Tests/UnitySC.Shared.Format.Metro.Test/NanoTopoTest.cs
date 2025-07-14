using System;
using System.Collections.Generic;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    [TestClass]
    public class NanoTopoTest
    {
        [TestMethod]
        public void TestNanoTopoFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.anantp";
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ANALYSE_NanoTopo, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void ReadWriteNanoTopoBlankWaferResult()
        {
            var nanoTopoResult = new NanoTopoResult();
            nanoTopoResult.Name = "NanoTopoTest1";
            nanoTopoResult.Settings.RoughnessTarget = 20.Micrometers();
            nanoTopoResult.Settings.RoughnessTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            nanoTopoResult.Settings.StepHeightTarget = 20.Micrometers();
            nanoTopoResult.Settings.StepHeightTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            nanoTopoResult.Settings.ExternalProcessingOutputs = new List<ExternalProcessingOutput>();
            nanoTopoResult.Settings.ExternalProcessingOutputs.Add(new ExternalProcessingOutput() { Name = "DigitalSurfOutput1", OutputTarget = 10, OutputTolerance = new Tolerance(10, ToleranceUnit.Percentage) });
            nanoTopoResult.Settings.ExternalProcessingOutputs.Add(new ExternalProcessingOutput() { Name = "DigitalSurfOutput2", OutputTarget = 20, OutputTolerance = new Tolerance(15, ToleranceUnit.Percentage) });
            nanoTopoResult.Information = "Test blank wafer result";
            var rnd = new Random();
            nanoTopoResult.State = (GlobalState)rnd.Next(0, 5);
            nanoTopoResult.Wafer = MetroHelper.CreateWafer();
            nanoTopoResult.AutomationInfo = MetroHelper.CreateAutomResultInfo();
            nanoTopoResult.Points = new List<MeasurePointResult>();

            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());
            nanoTopoResult.Points.Add(CreateTestNanoTopoPointResult());


            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_NanoTopo);
            metroResWrite.MeasureResult = nanoTopoResult;

            // Test WCF Serialize
            XML.DatacontractSerialize(nanoTopoResult, "NanoTopoBlankWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<NanoTopoResult>("NanoTopoBlankWafer.testwcf");

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"NanoTopoBlankWafer.xml", out error), "Bad file name extension for NanoTopo results");

            bool bRes = metroResWrite.WriteInFile(@"NanoTopoBlankWafer.anantp", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_NanoTopo);
            metroRes.ReadFromFile(@"NanoTopoBlankWafer.anantp", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as NanoTopoResult;
            AssertNTPResult(nanoTopoResult, resXML, "XML Serialize");

            AssertNTPResult(nanoTopoResult, resWCF, "WCF Serialize");
        }

        private MeasurePointResult CreateTestNanoTopoPointResult(bool isInDie = false)
        {
            var point = new NanoTopoPointResult();
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
            int NumIdRnd = MetroHelper.StaticRandom.Instance.Next(0, 500);
            var pointData1 = new NanoTopoPointData();
            pointData1.StepHeight = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData1.Roughness = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData1.ExternalProcessingResults = CreateExternalProccessingResults();
            pointData1.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData1.ResultImageFileName = $"/0/ntpthumb{NumIdRnd}.png";
            point.Datas.Add(pointData1);
            var pointData2 = new NanoTopoPointData();
            pointData2.StepHeight = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData2.Roughness = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData2.ExternalProcessingResults = CreateExternalProccessingResults();
            pointData2.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData2.IndexRepeta = 1;
            pointData2.ResultImageFileName = $"/1/ntpthumb{NumIdRnd}.png";
            pointData2.ReportFileName = $"/1/ntpreport{NumIdRnd}.pdf";
            point.Datas.Add(pointData2);
            return point;
        }

        private List<ExternalProcessingResult> CreateExternalProccessingResults()
        {
            var res = new List<ExternalProcessingResult>();
            res.Add(new ExternalProcessingResult() { Name = "DigitalSurfOutput1", Value = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01) });
            res.Add(new ExternalProcessingResult() { Name = "DigitalSurfOutput2", Value = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01) });
            return res;
        }

        private void AssertNTPResult(NanoTopoResult expected, NanoTopoResult actual, string testName)
        {
            Assert.AreEqual(expected.Name, actual.Name, $"{testName} Name");
            Assert.AreEqual(expected.Information, actual.Information, $"{testName} Information");
            Assert.AreEqual(expected.State, actual.State, $"{testName} State");

            Assert.AreEqual(expected.Settings.RoughnessTarget.ToString(), actual.Settings.RoughnessTarget.ToString(), $"{testName} Settings RoughnessTarget");
            Assert.AreEqual(expected.Settings.RoughnessTolerance.ToString(), actual.Settings.RoughnessTolerance.ToString(), $"{testName} Settings RoughnessTolerance");
            Assert.AreEqual(expected.Settings.StepHeightTarget.ToString(), actual.Settings.StepHeightTarget.ToString(), $"{testName} Settings StepHeightTarget");
            Assert.AreEqual(expected.Settings.StepHeightTolerance.ToString(), actual.Settings.StepHeightTolerance.ToString(), $"{testName} Settings StepHeightTolerance");
            Assert.AreEqual(expected.Settings.ExternalProcessingOutputs.Count, actual.Settings.ExternalProcessingOutputs.Count, $"{testName} Settings ExternalProcessingOutputs.Count");
            for (int k = 0; k < expected.Settings.ExternalProcessingOutputs.Count; k++)
            {
                Assert.AreEqual(expected.Settings.ExternalProcessingOutputs[k].Name, actual.Settings.ExternalProcessingOutputs[k].Name, $"{testName} Settings ExternalProcessingOutputs[{k}].Name");
                Assert.AreEqual(expected.Settings.ExternalProcessingOutputs[k].OutputTarget, actual.Settings.ExternalProcessingOutputs[k].OutputTarget, $"{testName} Settings ExternalProcessingOutputs[{k}].OutputTarget");
                Assert.AreEqual(expected.Settings.ExternalProcessingOutputs[k].OutputTolerance.Value, actual.Settings.ExternalProcessingOutputs[k].OutputTolerance.Value, $"{testName} Settings ExternalProcessingOutputs[{k}].OutputTolerance.Value");
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
                        var expectedData = (expected.Points[i].Datas[j] as NanoTopoPointData);
                        var actualData = (actual.Points[i].Datas[j] as NanoTopoPointData);

                        Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Points[{i}].Datas[{j}].Index ");
                        Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Points[{i}].Datas[{j}].State ");
                        Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Points[{i}].Datas[{j}].QualityScore ");

                        Assert.AreEqual(expectedData.Roughness.ToString(), actualData.Roughness.ToString(), $"{testName} Points[{i}].Datas[{j}].Roughness ");
                        Assert.AreEqual(expectedData.StepHeight.ToString(), actualData.StepHeight.ToString(), $"{testName} Points[{i}].Datas[{j}].StepHeight ");
                        Assert.AreEqual(expectedData.ResultImageFileName, actualData.ResultImageFileName, $"{testName} Points[{i}].Datas[{j}].ResultImageFileName ");
                        Assert.AreEqual(expectedData.ReportFileName, actualData.ReportFileName, $"{testName} Points[{i}].Datas[{j}].ReportFileName ");

                        Assert.AreEqual(expectedData.ExternalProcessingResults.Count, actualData.ExternalProcessingResults.Count, $"{testName} Points[{i}].Datas[{j}].ExternalProcessingResults.Count");
                        for (int k = 0; k < expectedData.ExternalProcessingResults.Count; k++)
                        {
                            Assert.AreEqual(expectedData.ExternalProcessingResults[k].Name, actualData.ExternalProcessingResults[k].Name, $"{testName} Points[{i}].Datas[{j}].ExternalProcessingResults[{k}].Name");
                            Assert.AreEqual(expectedData.ExternalProcessingResults[k].Value, actualData.ExternalProcessingResults[k].Value, $"{testName} Points[{i}].Datas[{j}].ExternalProcessingResults[{k}].Value");
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
                            var expectedData = (exp.Datas[k] as NanoTopoPointData);
                            var actualData = (act.Datas[k] as NanoTopoPointData);

                            Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].IndexRepeta ");
                            Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].State ");
                            Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].QualityScore ");

                            Assert.AreEqual(expectedData.ResultImageFileName, actualData.ResultImageFileName, $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].ResultImageFileName ");
                            Assert.AreEqual(expectedData.ReportFileName, actualData.ReportFileName, $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].ReportFileName ");

                            Assert.AreEqual(expectedData.Roughness.ToString(), actualData.Roughness.ToString(), $"{testName} Dies[{i}].Points[{j}].Datas[{k}].Roughness ");
                            Assert.AreEqual(expectedData.StepHeight.ToString(), actualData.StepHeight.ToString(), $"{testName} Dies[{i}].Points[{j}].Datas[{k}].StepHeight ");

                            Assert.AreEqual(expectedData.ExternalProcessingResults.Count, actualData.ExternalProcessingResults.Count, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].ExternalProcessingResults.Count");
                            for (int kk = 0; kk < expectedData.ExternalProcessingResults.Count; kk++)
                            {
                                Assert.AreEqual(expectedData.ExternalProcessingResults[kk].Name, actualData.ExternalProcessingResults[kk].Name, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].ExternalProcessingResults[{kk}].Name");
                                Assert.AreEqual(expectedData.ExternalProcessingResults[kk].Value, actualData.ExternalProcessingResults[kk].Value, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].ExternalProcessingResults[{kk}].Value");
                            }
                        }
                    }
                }
            }
        }
    }


}
