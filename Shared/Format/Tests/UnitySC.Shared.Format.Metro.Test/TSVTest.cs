using System;
using System.Collections;
using System.Collections.Generic;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    [TestClass]
    public class TSVTest
    {
        [TestMethod]
        public void TestTSVFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.anatsv";
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ANALYSE_TSV, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void ReadWriteTSVBlankWaferResult()
        {
            var tsvResult = new TSVResult();
            tsvResult.Name = "TSVTest1";
            tsvResult.Settings.DepthTarget = 20.Micrometers();
            tsvResult.Settings.DepthTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            tsvResult.Settings.WidthTarget = 20.Micrometers();
            tsvResult.Settings.WidthTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            tsvResult.Settings.LengthTarget = 20.Micrometers();
            tsvResult.Settings.LengthTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            tsvResult.Information = "Test blank wafer result";
            var rnd = new Random();
            tsvResult.State = (GlobalState)rnd.Next(0, 5);
            tsvResult.Wafer = MetroHelper.CreateWafer();

            tsvResult.BestFitPlan = new BestFitPlan
            {
                CoeffA = new Length(12, LengthUnit.Nanometer),
                CoeffB = new Length(15, LengthUnit.Nanometer),
                CoeffC = new Length(0.8, LengthUnit.Nanometer)
            };

            tsvResult.Points = new List<MeasurePointResult>();
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());
            tsvResult.Points.Add(CreateTestTSVPointResult());

            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_TSV);
            metroResWrite.MeasureResult = tsvResult;

            // Test WCF Serialize
            XML.DatacontractSerialize(tsvResult, "TSVBlankWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<TSVResult>("TSVBlankWafer.testwcf", true);

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"TSVBlankWafer.xml", out error), "Bad file name extension for TSV results");

            bool bRes = metroResWrite.WriteInFile(@"TSVBlankWafer.anatsv", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));


            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_TSV);
            metroRes.ReadFromFile(@"TSVBlankWafer.anatsv", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as TSVResult;
            AssertTSVResult(tsvResult, resXML, "XML Serialize");

            AssertTSVResult(tsvResult, resWCF, "WCF Serialize");
        }

        [TestMethod]
        public void ReadWriteTSVPatternedWaferResult()
        {
            var tsvResult = new TSVResult();
            tsvResult.Name = "TSVTest2";
            tsvResult.Settings.DepthTarget = 20.Micrometers();
            tsvResult.Settings.DepthTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            tsvResult.Settings.WidthTarget = 20.Micrometers();
            tsvResult.Settings.WidthTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            tsvResult.Settings.LengthTarget = 20.Micrometers();
            tsvResult.Settings.LengthTolerance = new LengthTolerance(2, Tools.Tolerances.LengthToleranceUnit.Micrometer);
            tsvResult.Information = "Test patterned wafer result";
            tsvResult.State = GlobalState.Success;
            tsvResult.Wafer = MetroHelper.CreateWafer();
            tsvResult.DiesMap = MetroHelper.CreateWaferMap();
            tsvResult.AutomationInfo = MetroHelper.CreateAutomResultInfo();

            tsvResult.BestFitPlan = new BestFitPlan
            {
                CoeffA = new Length(12, LengthUnit.Nanometer),
                CoeffB = new Length(15, LengthUnit.Nanometer),
                CoeffC = new Length(0.8, LengthUnit.Nanometer)
            };

            tsvResult.Dies = new List<MeasureDieResult>();
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());
            tsvResult.Dies.Add(CreateTSVDieResult());

            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_TSV);
            metroResWrite.MeasureResult = tsvResult;
            // Test WCF Serialize
            XML.DatacontractSerialize(tsvResult, "TSVDieWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<TSVResult>("TSVDieWafer.testwcf", true);

            string error;
            metroResWrite.WriteInFile(@"TSVPatternedWafer.anatsv", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_TSV);
            metroRes.ReadFromFile(@"TSVPatternedWafer.anatsv", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as TSVResult;
            AssertTSVResult(tsvResult, resXML, "XML Serialize");

            AssertTSVResult(tsvResult, resWCF, "WCF Serialize");
        }

        private TSVPointResult CreateTestTSVPointResult(bool isInDie = false)
        {
            var point = new TSVPointResult();
            point.CoplaInWaferValue = (MetroHelper.StaticRandom.Instance.Next(0, 200) * 0.1).Micrometers();
            if (isInDie)
            {
                point.CoplaInDieValue = (MetroHelper.StaticRandom.Instance.Next(0, 200) * 0.1).Micrometers();
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
            var pointData1 = new TSVPointData();
            pointData1.Length = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData1.Depth = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData1.Width = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData1.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData1.ResultImageFileName = $"/0/tsvthumb{NumIdRnd}.png";
            point.Datas.Add(pointData1);
            var pointData2 = new TSVPointData();
            pointData2.Length = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData2.Depth = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData2.Width = (MetroHelper.StaticRandom.Instance.Next(0, 2000) * 0.01).Micrometers();
            pointData2.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData2.IndexRepeta = 1;
            pointData2.ResultImageFileName = $"/1/tsvthumb{NumIdRnd}.png";

            var mem = new float[2*4096];
            var rnd = new Random(1235);
            for(int i=0; i < mem.Length; i++)
                mem[i] = (float) (10.0f * rnd.NextDouble());

            var membyte = new byte[sizeof(float) * mem.Length + 2];
            membyte[0] = (byte) 0;  // version
            membyte[1] = (byte) 75; // 0-100 pct saturation
            Buffer.BlockCopy(mem, 0, membyte, 2, membyte.Length -2 );
            pointData2.DepthRawSignal = membyte;

            point.Datas.Add(pointData2);
            return point;
        }

        private TSVDieResult CreateTSVDieResult()
        {
            var die = new TSVDieResult();
            die.State = (GlobalState)MetroHelper.StaticRandom.Instance.Next(0, 5);
            die.RowIndex = MetroHelper.StaticRandom.Instance.Next(0, 100) - 50;
            die.ColumnIndex = MetroHelper.StaticRandom.Instance.Next(0, 100) - 50;
            die.BestFitPlan = new BestFitPlan
            {
                CoeffA = new Length(12, LengthUnit.Nanometer),
                CoeffB = new Length(15, LengthUnit.Nanometer),
                CoeffC = new Length(0.8, LengthUnit.Nanometer)
            };
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            die.Points.Add(CreateTestTSVPointResult());
            return die;
        }

        private void AssertTSVResult(TSVResult expected, TSVResult actual, string testName)
        {
            Assert.AreEqual(expected.Name, actual.Name, $"{testName} Name");
            Assert.AreEqual(expected.Information, actual.Information, $"{testName} Information");
            Assert.AreEqual(expected.State, actual.State, $"{testName} State");

            Assert.AreEqual(expected.Settings.DepthTarget.ToString(), actual.Settings.DepthTarget.ToString(), $"{testName} Settings DepthTarget");
            Assert.AreEqual(expected.Settings.DepthTolerance.ToString(), actual.Settings.DepthTolerance.ToString(), $"{testName} Settings DepthTolerance");
            Assert.AreEqual(expected.Settings.WidthTarget.ToString(), actual.Settings.WidthTarget.ToString(), $"{testName} Settings WidthTarget");
            Assert.AreEqual(expected.Settings.WidthTolerance.ToString(), actual.Settings.WidthTolerance.ToString(), $"{testName} Settings WidthTolerance");
            Assert.AreEqual(expected.Settings.LengthTarget.ToString(), actual.Settings.LengthTarget.ToString(), $"{testName} Settings LengthTarget");
            Assert.AreEqual(expected.Settings.LengthTolerance.ToString(), actual.Settings.LengthTolerance.ToString(), $"{testName} Settings LengthTolerance");

            Assert.AreEqual(expected.Wafer.ToString(), actual.Wafer.ToString(), $"{testName} Wafer");
            MetroHelper.AreEqualAutomInfo(expected.AutomationInfo, actual.AutomationInfo, testName);

            Assert.AreEqual(expected.BestFitPlan.CoeffA.ToString(), actual.BestFitPlan.CoeffA.ToString(), $"{testName} BestFitPlan.CoeffA");
            Assert.AreEqual(expected.BestFitPlan.CoeffB.ToString(), actual.BestFitPlan.CoeffB.ToString(), $"{testName} BestFitPlan.CoeffB");
            Assert.AreEqual(expected.BestFitPlan.CoeffC.ToString(), actual.BestFitPlan.CoeffC.ToString(), $"{testName} BestFitPlan.CoeffC");

            if (expected.Points != null)
            {
                Assert.AreEqual(expected.Points.Count, actual.Points.Count, $"{testName} Points.Count");
                for (int i = 0; i < expected.Points.Count; i++)
                {
                    Assert.AreEqual(expected.Points[i].XPosition, actual.Points[i].XPosition, $"{testName} Points[{i}].XPosition");
                    Assert.AreEqual(expected.Points[i].YPosition, actual.Points[i].YPosition, $"{testName} Points[{i}].YPosition");
                    Assert.AreEqual(expected.Points[i].State, actual.Points[i].State, $"{testName} Points[{i}].State");

                    Assert.AreEqual((expected.Points[i] as TSVPointResult).CoplaInWaferValue.ToString(), (actual.Points[i] as TSVPointResult).CoplaInWaferValue.ToString(), $"{testName} Points[{i}].CoplaInWaferValue");

                    Assert.AreEqual(expected.Points[i].Datas.Count, actual.Points[i].Datas.Count, $"{testName} Points[i].Datas.Count");
                    for (int j = 0; j < expected.Points[i].Datas.Count; j++)
                    {
                        var expectedData = (expected.Points[i].Datas[j] as TSVPointData);
                        var actualData = (actual.Points[i].Datas[j] as TSVPointData);

                        Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Points[{i}].Datas[{j}].IndexRepeta ");
                        Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Points[{i}].Datas[{j}].State ");
                        Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Points[{i}].Datas[{j}].QualityScore ");

                        Assert.AreEqual(expectedData.Length.ToString(), actualData.Length.ToString(), $"{testName} Points[{i}].Datas[{j}].Length ");
                        Assert.AreEqual(expectedData.Width.ToString(), actualData.Width.ToString(), $"{testName} Points[{i}].Datas[{j}].Width ");
                        Assert.AreEqual(expectedData.Depth.ToString(), actualData.Depth.ToString(), $"{testName} Points[{i}].Datas[{j}].Depth ");

                        Assert.AreEqual(expectedData.ResultImageFileName, actualData.ResultImageFileName, $"{testName} Points[{i}].Datas[{j}].ResultImageFileName ");

                        if (expectedData.DepthRawSignal == null)
                            Assert.IsNull(actualData.DepthRawSignal, $"{testName} Points[{i}].Datas[{j}] DepthDepthRawSignal should be null");
                        else
                        {
                            Assert.AreEqual(expectedData.DepthRawSignal.Length, actualData.DepthRawSignal.Length, $"{testName} Points[{i}].Datas[{j}].DepthRawSignal.Length ");
                            for(int kk=0; kk < expectedData.DepthRawSignal.Length; kk++)
                                Assert.AreEqual(expectedData.DepthRawSignal[kk], actualData.DepthRawSignal[kk], $"{testName} Points[{i}].Datas[{j}].DepthRawSignal[{kk}] ");
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

                    var expdie = expected.Dies[i] as TSVDieResult;
                    var actdie = actual.Dies[i] as TSVDieResult;
                    Assert.AreEqual(expdie.BestFitPlan.CoeffB.ToString(), actdie.BestFitPlan.CoeffB.ToString(), $"{testName} Dies[{i}].BestFitPlan.CoeffB");
                    Assert.AreEqual(expdie.BestFitPlan.CoeffC.ToString(), actdie.BestFitPlan.CoeffC.ToString(), $"{testName} Dies[{i}].BestFitPlan.CoeffC");
                    Assert.AreEqual(expdie.BestFitPlan.CoeffA.ToString(), actdie.BestFitPlan.CoeffA.ToString(), $"{testName} Dies[{i}].BestFitPlan.CoeffA");

                    Assert.AreEqual(expected.Dies[i].Points.Count, actual.Dies[i].Points.Count, $"{testName} Dies[{i}].Points.Count");
                    for (int j = 0; j < expected.Dies[i].Points.Count; j++)
                    {
                        var exp = expected.Dies[i].Points[j] as TSVPointResult;
                        var act = expected.Dies[i].Points[j] as TSVPointResult;

                        Assert.AreEqual(exp.XPosition, act.XPosition, $"{testName} Dies[{i}].Points[{j}].XPosition");
                        Assert.AreEqual(exp.YPosition, act.YPosition, $"{testName} Dies[{i}].Points[{j}].YPosition");
                        Assert.AreEqual(exp.State, act.State, $"{testName} Dies[{i}].Points[{i}].State");

                        Assert.AreEqual(exp.CoplaInWaferValue.ToString(), act.CoplaInWaferValue.ToString(), $"{testName} Dies[{i}].Points[{i}].CoplaInWaferValue");

                        Assert.AreEqual(exp.Datas.Count, act.Datas.Count, $"{testName} Dies[{i}].Points[{j}].Datas.Count");
                        for (int k = 0; k < exp.Datas.Count; k++)
                        {
                            var expectedData = (exp.Datas[k] as TSVPointData);
                            var actualData = (act.Datas[k] as TSVPointData);

                            Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].IndexRepeta ");
                            Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].State ");
                            Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Dies[{i}].Points[{j}].Datas[{k}].QualityScore ");

                            Assert.AreEqual(expectedData.Length.ToString(), actualData.Length.ToString(), $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].Length ");
                            Assert.AreEqual(expectedData.Width.ToString(), actualData.Width.ToString(), $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].Width ");
                            Assert.AreEqual(expectedData.Depth.ToString(), actualData.Depth.ToString(), $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].Depth ");

                            Assert.AreEqual(expectedData.ResultImageFileName, actualData.ResultImageFileName, $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].ResultImageFileName ");

                            if (expectedData.DepthRawSignal == null)
                                Assert.IsNull(actualData.DepthRawSignal, $"{testName} Points[{i}].Datas[{j}] DepthDepthRawSignal should be null");
                            else
                            {
                                Assert.AreEqual(expectedData.DepthRawSignal.Length, actualData.DepthRawSignal.Length, $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].DepthRawSignal.Length ");
                                for (int kk = 0; kk < expectedData.DepthRawSignal.Length; kk++)
                                    Assert.AreEqual(expectedData.DepthRawSignal[kk], actualData.DepthRawSignal[kk], $"{testName}  Dies[{i}].Points[{j}].Datas[{k}].DepthRawSignal[{kk}] ");
                            }
                        }
                    }
                }
            }
        }


    }
}
