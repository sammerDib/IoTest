using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data.Composer;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Data.Test
{
   
    [TestClass]
    public class TemplateComposerTests
    {

        public class TestParamsObject : IParamComposerObject
        {
            public static readonly TestParamsObject Empty = new TestParamsObject();
            private readonly string[] _labels;
            public TestParamsObject() { _labels = new string[] { "ToolName", "Slot" }; }
            public string[] ToParamLabels() { return _labels; }
            public object[] ToParamObjects() { return new object[] { ToolName, Slot.ToString("00") }; }
            public string ToolName { get; set; } = string.Empty;
            public int Slot { get; set; } = 99;
        }

        [TestMethod]
        public void TestComposerWithNullModel()
        {
            string fmtNullModel = null;
            var cmpTest = new TemplateComposer(fmtNullModel, TestParamsObject.Empty);
            Assert.IsNotNull(cmpTest);

            var testprm = new TestParamsObject() { ToolName = "tooltest", Slot = 5 };
            var res1 = cmpTest.ComposeWith(testprm);
            Assert.IsTrue(String.IsNullOrEmpty(res1));
        }

        [TestMethod]
        public void TestComposerWithEmptyModel()
        {
            string fmtNullModel = string.Empty;
            var cmpTest = new TemplateComposer(fmtNullModel, TestParamsObject.Empty);
            Assert.IsNotNull(cmpTest);

            var testprm = new TestParamsObject() { ToolName = "tooltest", Slot = 5 };
            var res1 = cmpTest.ComposeWith(testprm);
            Assert.IsTrue(String.IsNullOrEmpty(res1));
        }

        [TestMethod]
        public void TestComposerWithConsticiency()
        {
            string fmtModel = @"this Template toolName is {ToolName} for S{Slot}";

            var cmpTest = new TemplateComposer(fmtModel, TestParamsObject.Empty);
            Assert.IsNotNull(cmpTest);
            var cmpResult = new TemplateComposer(fmtModel, ResultPathParams.Empty);
            Assert.IsNotNull(cmpResult);

            var testprm = new TestParamsObject() { ToolName = "tooltest", Slot = 5 };
            var res1 = cmpTest.ComposeWith(testprm);
            Assert.AreEqual("this Template toolName is tooltest for S05", res1, "res1");
            Assert.ThrowsException<ArgumentException>(() => cmpResult.ComposeWith(testprm), "cmpResult with testprm should throw argument ex");
       
            var resprm = new ResultPathParams() { ToolName = "toolResult", Slot = 16 };
            var res2 = cmpResult.ComposeWith(resprm);
            Assert.AreEqual("this Template toolName is toolResult for S16", res2, "res2");
            Assert.ThrowsException<ArgumentException>(() => cmpTest.ComposeWith(resprm), "cmptTest with resprm should throw argument ex");
        }

        [TestMethod]
        public void TestPathTemplateComposerUsage()
        {
            string fmtModelDirectory = @"\\localhost\{ToolName}_{ToolId}\{LotName}\{ChamberName}_{ChamberId}\{RecipeName}\{StartProcessDate}_{JobId}\";
            var cmpDir1 = new TemplateComposer(fmtModelDirectory, ResultPathParams.Empty);
            Assert.IsNotNull(cmpDir1);

            string fmtModelFileName = @"{WaferName}_S{Slot}_{RunIter}.{ResultTypeExt}";
            var cmpFil1 = new TemplateComposer(fmtModelFileName, ResultPathParams.Empty);
            Assert.IsNotNull(cmpFil1);

            var prm = new ResultPathParams
            {
                ToolName = "MyTool",
                ToolId = 564,
                ToolKey = 4,
                ChamberName = "MyChamberName",
                ChamberId = 78,
                ChamberKey = 1,
                JobName = "MyJobName",
                JobId = 5983,
                LotName = "MyLOTName",
                RecipeName = "MyRECIPENAME",
                WaferName = "My-Wafer-Name",
                Slot = 13
            };
            var dt = new DateTime(2019, 06, 26, 16, 26, 36);
            prm.StartProcessDate = dt;
            prm.RunIter = 1;
            prm.ResultType = ResultType.ADC_Klarf;
            prm.Index = 1;
            prm.Label = "MyLabel";      
       
            string DirPath1 = cmpDir1.ComposeWith(prm);
            Assert.AreEqual(@"\\localhost\MyTool_564\MyLOTName\MyChamberName_78\MyRECIPENAME\20190626_162636_5983\", DirPath1, "DirPath1");
            
            string FileName1 = cmpFil1.ComposeWith(prm);
            Assert.AreEqual(@"My-Wafer-Name_S13_1.001", FileName1, "FileName1");

            prm.ResultType = ResultType.ADC_GlobalTopo;
            prm.RunIter = 0;
            prm.Slot = 4;
            FileName1 = cmpFil1.ComposeWith(prm);
            Assert.AreEqual(@"My-Wafer-Name_S04_0.gtr", FileName1, "FileName1Bis");

            prm.ResultType = ResultType.ADC_ASO;

            fmtModelDirectory = @"\\toto\{ChamberName}\ADC\{LotName}\{RecipeName}\{ResultType}";
            var cmpDir2 = new TemplateComposer(fmtModelDirectory, prm);
            Assert.IsNotNull(cmpDir2);

            fmtModelFileName = @"{ToolName}_{StartProcessDate}{WaferName}_{Slot}.{ResultTypeExt}";
            var cmpFil2 = new TemplateComposer(fmtModelFileName, prm);
            Assert.IsNotNull(cmpFil2);

            string DirPath2 = cmpDir2.ComposeWith(prm);
            Assert.AreEqual(@"\\toto\MyChamberName\ADC\MyLOTName\MyRECIPENAME\ADC_ASO",  DirPath2, "DirPath1");
            string FileName2 = cmpFil2.ComposeWith(prm);
            Assert.AreEqual(@"MyTool_20190626_162636My-Wafer-Name_04.aso", FileName2, "FileName2");

            fmtModelDirectory = @"\\titi\{ActorType}\{JobName}\{ResultFmt}\{Label}";
            var cmpDir3 = new TemplateComposer(fmtModelDirectory, prm);
            Assert.IsNotNull(cmpDir3);

            fmtModelFileName = @"{WaferName}_S{Slot}_{RunIter}#{Index}.{ResultTypeExt}";
            var cmpFil3 = new TemplateComposer(fmtModelFileName, prm);
            Assert.IsNotNull(cmpFil3);

            prm.ResultType = ResultType.ANALYSE_TSV;
            prm.Index = 0;
            string DirPath3 = cmpDir3.ComposeWith(prm);
            Assert.AreEqual(@"\\titi\ANALYSE\MyJobName\Metrology\MyLabel", DirPath3, "DirPath3");
            string FileName3 = cmpFil3.ComposeWith(prm);
            Assert.AreEqual(@"My-Wafer-Name_S04_0#0.anatsv", FileName3, "FileName3-1");

            prm.ResultType = ResultType.ANALYSE_CD;
            prm.Index = 2;
            FileName3 = cmpFil3.ComposeWith(prm);
            Assert.AreEqual(@"My-Wafer-Name_S04_0#2.anacd", FileName3, "FileName3-2");

            var prmempty = ResultPathParams.Empty;
            string FileName4 = cmpFil2.ComposeWith(prmempty);
            Assert.AreEqual(@"_00010101_000000_00.ase", FileName4, "FileName4");

            fmtModelDirectory = @"\\USP\{ToolName}_{ToolKey}";
            var cmpDir4 = new TemplateComposer(fmtModelDirectory, prmempty);
            Assert.IsNotNull(cmpDir4);

            fmtModelFileName = @"{ChamberKey}_{WaferName}_S{Slot}.{ResultTypeExt}";
            var cmpFil4 = new TemplateComposer(fmtModelFileName, prmempty);
            Assert.IsNotNull(cmpFil4);

            prm.ResultType = ResultType.ADC_Haze;
            string DirPath4 = cmpDir4.ComposeWith(prm);
            Assert.AreEqual(@"\\USP\MyTool_4",  DirPath4, "DirPath4");
            string FileName5 = cmpFil4.ComposeWith(prm);
            Assert.AreEqual(@"1_My-Wafer-Name_S04.haze", FileName5, "FileName5");
        }

        [TestMethod]
        public void TestResultPathParamsObjectStructure()
        {
            var prm = new ResultPathParams();

            var labels = prm.ToParamLabels();
            Assert.IsNotNull(labels, "Not Null Labels");
            var objects = prm.ToParamObjects();
            Assert.IsNotNull(objects, "Not Null Objects");

            Assert.AreEqual(labels.Count(), objects.Count(), "Labels should have the same count as objects");

            Assert.IsTrue(labels.ToList().Distinct().Count() == labels.Count(), "labels should be unique");
        }

        [TestMethod]
        public void TestResultPathParamsObject()
        {
            var prm = new ResultPathParams();

            Assert.AreEqual(string.Empty, prm.ToolName);
            Assert.AreEqual(string.Empty, prm.ChamberName);
            Assert.AreEqual(string.Empty, prm.JobName);
            Assert.AreEqual(string.Empty, prm.LotName);
            Assert.AreEqual(string.Empty, prm.RecipeName);
            Assert.AreEqual(string.Empty, prm.WaferName);

            Assert.AreEqual(string.Empty, prm.Label);
            Assert.AreEqual(string.Empty, prm.ProductName);
            Assert.AreEqual(string.Empty, prm.StepName);

            Assert.AreEqual(DateTime.MinValue, prm.StartProcessDate);

            Assert.AreEqual(-1, prm.ToolId);
            Assert.AreEqual(-1, prm.ToolKey);
            Assert.AreEqual(-1, prm.ChamberId);
            Assert.AreEqual(-1, prm.ChamberKey);
            Assert.AreEqual(-1, prm.JobId);
            Assert.AreEqual(-1, prm.LoadPort);

            Assert.AreEqual(0, prm.Slot);
            Assert.AreEqual(0, prm.RunIter);
            Assert.AreEqual(0, prm.Index);

            Assert.AreEqual(ResultType.NotDefined, prm.ResultType); // a debattre
            Assert.AreEqual(ActorType.Unknown, prm.ActorType);
            Assert.AreEqual(ResultFormat.ASE, prm.ResultFmt);

            Assert.AreEqual("ase", prm.ResultTypeExt);
            Assert.AreEqual(Side.Unknown, prm.Side);


            Assert.AreEqual(0, prm.Index);

            prm.ToolName = "MyTool";
            prm.ChamberName = "MyChamberName";
            prm.ToolId = 564;
            prm.ToolKey = 3;
            prm.ChamberId = 78;
            prm.ChamberKey = 2;
            prm.Slot = 13;
            var dt = new DateTime(2019, 06, 26, 16, 26, 36);
            prm.StartProcessDate = dt;
            prm.LoadPort = 2;

            Assert.AreEqual("MyTool", prm.ToolName);
            Assert.AreEqual("MyChamberName", prm.ChamberName);
            Assert.AreEqual(564, prm.ToolId);
            Assert.AreEqual(3, prm.ToolKey);
            Assert.AreEqual(78, prm.ChamberId);
            Assert.AreEqual(2, prm.ChamberKey);
            Assert.AreEqual(13, prm.Slot);
            Assert.AreEqual(dt, prm.StartProcessDate);
            Assert.AreEqual(2, prm.LoadPort);

            prm.RunIter++;
            Assert.AreEqual(1, prm.RunIter);

            Assert.AreEqual("ase", prm.ResultTypeExt);
            prm.ResultType = ResultType.ADC_ASO;
            Assert.AreEqual(ActorType.ADC, prm.ActorType);
            Assert.AreEqual(ResultFormat.ASO, prm.ResultFmt);
            Assert.AreEqual("aso", prm.ResultTypeExt);

            prm.ResultType = ResultType.ADC_Klarf;
            Assert.AreEqual(ActorType.ADC, prm.ActorType);
            Assert.AreEqual(ResultFormat.Klarf, prm.ResultFmt);
            Assert.AreEqual("001", prm.ResultTypeExt);

            prm.ResultType = ResultType.ANALYSE_TSV;
            Assert.AreEqual(ActorType.ANALYSE, prm.ActorType);
            Assert.AreEqual(ResultFormat.Metrology, prm.ResultFmt);
            Assert.AreEqual("anatsv", prm.ResultTypeExt);

            prm.ResultType = ResultType.DMT_CurvatureY_Back;
            prm.Label = "CurvatureY";
            prm.ProductName = "Toto";
            prm.StepName = "Steptiti";
            Assert.AreEqual(Side.Back, prm.Side);
            Assert.AreEqual(ActorType.DEMETER, prm.ActorType);
            Assert.AreEqual(ResultFormat.FullImage, prm.ResultFmt);
            Assert.AreEqual(string.Empty, prm.ResultTypeExt);

            Assert.AreEqual("CurvatureY", prm.Label);
            Assert.AreEqual("Toto", prm.ProductName);
            Assert.AreEqual("Steptiti", prm.StepName);

        }

    }
}
