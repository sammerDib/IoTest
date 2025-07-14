using System;
using System.Collections.Generic;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    /// <summary>
    /// Summary description for StepHeightTest
    /// </summary>
    [TestClass]
    public class StepHeightTest
    {
        [TestMethod]
        public void TesStepFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.anastp";
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ANALYSE_Step, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void ReadWriteStepBlankWaferResult()
        {
            var StepResult = new StepResult();
            StepResult.Name = "StepTest1";
            StepResult.Settings.StepHeightTarget = 20.Micrometers();
            StepResult.Settings.StepHeightTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            StepResult.Settings.StepUp = (MetroHelper.StaticRandom.Instance.Next(0, 10) % 2) == 0;
            StepResult.Information = "Test blank wafer result";
            var rnd = new Random();
            StepResult.State = (GlobalState)rnd.Next(0, 5);
            StepResult.Wafer = MetroHelper.CreateWafer();
            StepResult.Points = new List<MeasurePointResult>();

            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));
            StepResult.Points.Add(CreateTestStepPointResult(StepResult.Settings.StepUp));


            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_Step);
            metroResWrite.MeasureResult = StepResult;

            // Test WCF Serialize
            XML.DatacontractSerialize(StepResult, "StepBlankWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<StepResult>("StepBlankWafer.testwcf");

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"StepBlankWafer.xml", out error), "Bad file name extension for Step results");

            bool bRes = metroResWrite.WriteInFile(@"StepBlankWafer.anastp", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_Step);
            metroRes.ReadFromFile(@"StepBlankWafer.anastp", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as StepResult;
            AssertSTPResult(StepResult, resXML, "XML Serialize");

            AssertSTPResult(StepResult, resWCF, "WCF Serialize");
        }

        private MeasurePointResult CreateTestStepPointResult(bool stepup, bool isInDie = false)
        {
            var point = new StepPointResult();
            point.ScanAngle = new Angle((double)(MetroHelper.StaticRandom.Instance.Next(0, 18) * 10), AngleUnit.Degree);
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
            var pointData1 = new StepPointData();
            pointData1.StepHeight = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData1.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;

            // RAW PROFILE
            pointData1.RawProfileScan = createRawProfileScan(10.Micrometers(), pointData1.StepHeight);

            point.Datas.Add(pointData1);
            var pointData2 = new StepPointData();
            pointData2.StepHeight = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData2.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData2.IndexRepeta = 1;

            // RAW PROFILE
            pointData2.RawProfileScan = createRawProfileScan(15.Micrometers(), pointData2.StepHeight, stepup);

            point.Datas.Add(pointData2);
            return point;
        }

        private void AssertSTPResult(StepResult expected, StepResult actual, string testName)
        {
            Assert.AreEqual(expected.Name, actual.Name, $"{testName} Name");
            Assert.AreEqual(expected.Information, actual.Information, $"{testName} Information");
            Assert.AreEqual(expected.State, actual.State, $"{testName} State");

            Assert.AreEqual(expected.Settings.StepHeightTarget.ToString(), actual.Settings.StepHeightTarget.ToString(), $"{testName} Settings StepHeightTarget");
            Assert.AreEqual(expected.Settings.StepHeightTolerance.ToString(), actual.Settings.StepHeightTolerance.ToString(), $"{testName} Settings StepHeightTolerance");
            Assert.AreEqual(expected.Settings.StepUp, actual.Settings.StepUp, $"{testName} Settings StepUp");

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

                    Assert.AreEqual(((expected.Points[i]) as StepPointResult).ScanAngle.ToString(), ((actual.Points[i]) as StepPointResult).ScanAngle.ToString(), $"{testName} Points[{i}].ScanAngle");

                    Assert.AreEqual(expected.Points[i].Datas.Count, actual.Points[i].Datas.Count, $"{testName} Points[i].Datas.Count");
                    for (int j = 0; j < expected.Points[i].Datas.Count; j++)
                    {
                        var expectedData = (expected.Points[i].Datas[j] as StepPointData);
                        var actualData = (actual.Points[i].Datas[j] as StepPointData);

                        Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Points[{i}].Datas[{j}].IndexRepeta ");
                        Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Points[{i}].Datas[{j}].State ");
                        Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Points[{i}].Datas[{j}].QualityScore ");

                        Assert.AreEqual(expectedData.StepHeight.ToString(), actualData.StepHeight.ToString(), $"{testName} Points[{i}].Datas[{j}].StepHeight ");

                        // RAW PRofile
                        AssertRAwProfile(expectedData.RawProfileScan, actualData.RawProfileScan, $"{testName} Points[{i}].Datas[{j}].RawProfileScan");
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

                        Assert.AreEqual(((exp) as StepPointResult).ScanAngle.ToString(), ((act) as StepPointResult).ScanAngle.ToString(), $"{testName} Points[{i}].ScanAngle");


                        Assert.AreEqual(exp.Datas.Count, act.Datas.Count, $"{testName} Dies[{i}].Points[{j}].Datas.Count");
                        for (int k = 0; k < exp.Datas.Count; k++)
                        {
                            var expectedData = (exp.Datas[k] as StepPointData);
                            var actualData = (act.Datas[k] as StepPointData);

                            Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].IndexRepeta ");
                            Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].State ");
                            Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].QualityScore ");

                            // RAW profile
                            AssertRAwProfile(expectedData.RawProfileScan, actualData.RawProfileScan, $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].RawProfileScan");
                            Assert.AreEqual(expectedData.StepHeight.ToString(), actualData.StepHeight.ToString(), $"{testName} Dies[{i}].Points[{j}].Datas[{k}].StepHeight ");
                        }
                    }
                }
            }
        }
        private RawProfile createRawProfileScan(Length X, Length Z, bool stepUp = true)
        {
            RawProfile rawscan = new RawProfile() { XUnit = X.Unit, ZUnit = Z.Unit, RawPoints = new List<RawProfilePoint>((int)X.Micrometers + 1) };
            double Zlo_nm = MetroHelper.StaticRandom.Instance.Next(10, (int)(Z.Nanometers / 4.0) + 10);
            double Zlo_um = Zlo_nm / 1000.0;
            int half = (int)X.Micrometers / 2;
            int neg = stepUp ? 1 : -1;
            for (int i = 0; i <= (int)X.Micrometers; i++)
            {
                double zc = ((neg * (i - half) < 0.0)) ? Zlo_um : Z.Micrometers;
                zc += (double)(MetroHelper.StaticRandom.Instance.Next(-250, 250)) / 1000.0;
                rawscan.RawPoints.Add(new RawProfilePoint() { X = (double)i, Z = zc });
            }
            return rawscan;
        }

        private void AssertRAwProfile(RawProfile expected, RawProfile actual, string testName)
        {
            if (expected == null)
                Assert.IsNull(actual, $"{testName} RawProfile is NOT null");
            else
            {
                Assert.AreEqual(expected.XUnit, actual.XUnit, $"{testName} XUnit differs");
                Assert.AreEqual(expected.ZUnit, actual.ZUnit, $"{testName} ZUnit differs");
                Assert.AreEqual(expected.RawPoints.Count, actual.RawPoints.Count, $"{testName} Count");
                for (int i = 0; i < expected.RawPoints.Count; i++)
                {
                    Assert.AreEqual(expected.RawPoints[i].X, actual.RawPoints[i].X, $"{testName} RawPoint[{i}].X");
                    Assert.AreEqual(expected.RawPoints[i].Z, actual.RawPoints[i].Z, $"{testName} RawPoint[{i}].Z");
                }
            }
        }


    }
}
