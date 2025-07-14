using System;
using System.Collections.Generic;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    /// <summary>
    /// Summary description for PillarTest
    /// </summary>
    [TestClass]
    public class PillarTest
    {
        [TestMethod]
        public void TestPillarFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.anapil";
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ANALYSE_Pillar, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void ReadWritePillarBlankWaferResult()
        {
            var PillarResult = new PillarResult();
            PillarResult.Name = "PillarTest1";
            PillarResult.Settings.HeightTarget = 72.Micrometers();
            PillarResult.Settings.HeightTolerance = new LengthTolerance(12, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            PillarResult.Settings.WidthTarget = 100.Micrometers();
            PillarResult.Settings.WidthTolerance = new LengthTolerance(8, Tools.Tolerances.LengthToleranceUnit.Micrometer);


            PillarResult.Information = "Test blank wafer result";
            var rnd = new Random();
            PillarResult.State = (GlobalState)rnd.Next(0, 5);
            PillarResult.Wafer = MetroHelper.CreateWafer();
            PillarResult.Points = new List<MeasurePointResult>();

            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());
            PillarResult.Points.Add(CreateTestPillarPointResult());


            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_Pillar);
            metroResWrite.MeasureResult = PillarResult;

            PillarResult.FillNonSerializedProperties(true, true);

            // Test WCF Serialize
            XML.DatacontractSerialize(PillarResult, "PillarBlankWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<PillarResult>("PillarBlankWafer.testwcf");

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"PillarBlankWafer.xml", out error), "Bad file name extension for Pillar results");

            bool bRes = metroResWrite.WriteInFile(@"PillarBlankWafer.anapil", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_Pillar);
            metroRes.ReadFromFile(@"PillarBlankWafer.anapil", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as PillarResult;
            AssertResult(PillarResult, resXML, "XML Serialize");

            AssertResult(PillarResult, resWCF, "WCF Serialize");
        }

        private MeasurePointResult CreateTestPillarPointResult(bool isInDie = false)
        {
            var point = new PillarPointResult();
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
            var pointData1 = new PillarPointData();
            pointData1.Width = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData1.Height = (MetroHelper.StaticRandom.Instance.Next(0, 500) * 0.01).Micrometers();
            pointData1.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;

            // RAW PROFILE
            pointData1.RawProfileScan = createRawProfileScan(pointData1.Width, pointData1.Height);

            point.Datas.Add(pointData1);
            var pointData2 = new PillarPointData();

            pointData2.Width = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData2.Height = (MetroHelper.StaticRandom.Instance.Next(0, 500) * 0.01).Micrometers();
            pointData2.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData2.IndexRepeta = 1;

            // RAW PROFILE
            pointData2.RawProfileScan = createRawProfileScan(pointData2.Width, pointData2.Height);

            point.Datas.Add(pointData2);
            return point;
        }

        private void AssertResult(PillarResult expected, PillarResult actual, string testName)
        {
            Assert.AreEqual(expected.Name, actual.Name, $"{testName} Name");
            Assert.AreEqual(expected.Information, actual.Information, $"{testName} Information");
            Assert.AreEqual(expected.State, actual.State, $"{testName} State");

            Assert.AreEqual(expected.Settings.HeightTarget.ToString(), actual.Settings.HeightTarget.ToString(), $"{testName} Settings HeightTarget");
            Assert.AreEqual(expected.Settings.HeightTolerance.ToString(), actual.Settings.HeightTolerance.ToString(), $"{testName} Settings HeightTolerance");

            Assert.AreEqual(expected.Settings.WidthTarget.ToString(), actual.Settings.WidthTarget.ToString(), $"{testName} Settings WidthTarget");
            Assert.AreEqual(expected.Settings.WidthTolerance.ToString(), actual.Settings.WidthTolerance.ToString(), $"{testName} Settings WidthTolerance");

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

                    Assert.AreEqual(((expected.Points[i]) as PillarPointResult).ScanAngle.ToString(), ((actual.Points[i]) as PillarPointResult).ScanAngle.ToString(), $"{testName} Points[{i}].ScanAngle");

                    Assert.AreEqual(expected.Points[i].Datas.Count, actual.Points[i].Datas.Count, $"{testName} Points[i].Datas.Count");
                    for (int j = 0; j < expected.Points[i].Datas.Count; j++)
                    {
                        var expectedData = (expected.Points[i].Datas[j] as PillarPointData);
                        var actualData = (actual.Points[i].Datas[j] as PillarPointData);

                        Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Points[{i}].Datas[{j}].IndexRepeta ");
                        Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Points[{i}].Datas[{j}].State ");
                        Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Points[{i}].Datas[{j}].QualityScore ");

                        Assert.AreEqual(expectedData.Height.ToString(), actualData.Height.ToString(), $"{testName} Points[{i}].Datas[{j}].Height ");
                        Assert.AreEqual(expectedData.Width.ToString(), actualData.Width.ToString(), $"{testName} Points[{i}].Datas[{j}].Width ");

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

                        Assert.AreEqual(((exp) as PillarPointResult).ScanAngle.ToString(), ((act) as PillarPointResult).ScanAngle.ToString(), $"{testName} Points[{i}].ScanAngle");


                        Assert.AreEqual(exp.Datas.Count, act.Datas.Count, $"{testName} Dies[{i}].Points[{j}].Datas.Count");
                        for (int k = 0; k < exp.Datas.Count; k++)
                        {
                            var expectedData = (exp.Datas[k] as PillarPointData);
                            var actualData = (act.Datas[k] as PillarPointData);

                            Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].IndexRepeta ");
                            Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].State ");
                            Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].QualityScore ");

                            // RAW profile
                            AssertRAwProfile(expectedData.RawProfileScan, actualData.RawProfileScan, $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].RawProfileScan");
                            Assert.AreEqual(expectedData.Height.ToString(), actualData.Height.ToString(), $"{testName} Dies[{i}].Points[{j}].Datas[{k}].Height ");
                            Assert.AreEqual(expectedData.Width.ToString(), actualData.Width.ToString(), $"{testName} Dies[{i}].Points[{j}].Datas[{k}].Width ");
                        }
                    }
                }
            }
        }
        private RawProfile createRawProfileScan(Length X, Length Z)
        {
            RawProfile rawscan = new RawProfile() { XUnit = X.Unit, ZUnit = Z.Unit, RawPoints = new List<RawProfilePoint>((int)X.Micrometers + 1) };
            double Zlo_nm = MetroHelper.StaticRandom.Instance.Next(0, 10);
            double Zlo_um = Zlo_nm / 1000.0;
            int xfullscan_um = (int)(3.0 * X.Micrometers);
            double xstart_um = X.Micrometers / 3.0;
            double xend_um = 2.0 * X.Micrometers / 3.0;
            for (int i = 0; i <= xfullscan_um; i++)
            {
                double di = (double)i;
                bool isLo = (di < xstart_um) || (di > xend_um);
                double zc = isLo ? Zlo_um : Z.Micrometers;
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
