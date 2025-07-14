using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.ResultScanner.Implementation;
using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.Shared.Data.Enum;

using UnitySC.Shared.Data.Enum.Module;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnitySC.Shared.Data.Composer;

namespace UnitTestResultScanner
{
    [TestClass]
    public class UnitTestPath
    {
      
        [TestMethod]
        public void TestPathComposer()
        {
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

            string fmtModelDirectory = @"\\localhost\{ToolName}_{ToolId}\{LotName}\{ChamberName}_{ChamberId}\{RecipeName}\{StartProcessDate}_{JobId}\";
            string fmtModelFileName = @"{WaferName}_S{Slot}_{RunIter}.{ResultTypeExt}";

            var cmp1 = new PathComposer(fmtModelFileName, fmtModelDirectory);

            string DirPath1 = cmp1.GetDirPath(prm);
            Assert.AreEqual(@"\\localhost\MyTool_564\MyLOTName\MyChamberName_78\MyRECIPENAME\20190626_162636_5983\",
                DirPath1, "DirPath1");
            string FileName1 = cmp1.GetFileName(prm);
            Assert.AreEqual(@"My-Wafer-Name_S13_1.001",
                FileName1, "FileName1");
            prm.ResultType = ResultType.ADC_GlobalTopo;
            prm.RunIter = 0;
            prm.Slot = 4;
            FileName1 = cmp1.GetFileName(prm);
            Assert.AreEqual(@"My-Wafer-Name_S04_0.gtr",
                FileName1, "FileName1Bis");

            prm.ResultType = ResultType.ADC_ASO;

            fmtModelDirectory = @"\\toto\{ChamberName}\ADC\{LotName}\{RecipeName}\{ResultType}";
            fmtModelFileName = @"{ToolName}_{StartProcessDate}{WaferName}_{Slot}.{ResultTypeExt}";
            var cmp2 = new PathComposer(fmtModelFileName, fmtModelDirectory);
            string DirPath2 = cmp2.GetDirPath(prm);
            Assert.AreEqual(@"\\toto\MyChamberName\ADC\MyLOTName\MyRECIPENAME\ADC_ASO",
                DirPath2, "DirPath1");
            string FileName2 = cmp2.GetFileName(prm);
            Assert.AreEqual(@"MyTool_20190626_162636My-Wafer-Name_04.aso",
                FileName2, "FileName2");

          
            fmtModelDirectory = @"\\titi\{ActorType}\{JobName}\{ResultFmt}\{Label}";
            fmtModelFileName = @"{WaferName}_S{Slot}_{RunIter}#{Index}.{ResultTypeExt}";
            var cmp3 = new PathComposer(fmtModelFileName, fmtModelDirectory);

            prm.ResultType = ResultType.ANALYSE_TSV;
            prm.Index = 0;
            string DirPath3 = cmp3.GetDirPath(prm);
            Assert.AreEqual(@"\\titi\ANALYSE\MyJobName\Metrology\MyLabel",
                DirPath3, "DirPath3");
            string FileName3 = cmp3.GetFileName(prm);
            Assert.AreEqual(@"My-Wafer-Name_S04_0#0.anatsv",
                FileName3, "FileName3-1");

            prm.ResultType = ResultType.ANALYSE_CD;
            prm.Index = 2;
            FileName3 = cmp3.GetFileName(prm);
            Assert.AreEqual(@"My-Wafer-Name_S04_0#2.anacd",
                FileName3, "FileName3-2");

            var prmempty = new ResultPathParams();
            string FileName4 = cmp2.GetFileName(prmempty);
            Assert.AreEqual(@"_00010101_000000_00.ase",
                FileName4, "FileName4");

            fmtModelDirectory = @"\\USP\{ToolName}_{ToolKey}";
            fmtModelFileName = @"{ChamberKey}_{WaferName}_S{Slot}.{ResultTypeExt}";
            var cmp4 = new PathComposer(fmtModelFileName, fmtModelDirectory);

            prm.ResultType = ResultType.ADC_Haze;
            string DirPath4 = cmp4.GetDirPath(prm);
            Assert.AreEqual(@"\\USP\MyTool_4",
                DirPath4, "DirPath4");
            string FileName5 = cmp4.GetFileName(prm);
            Assert.AreEqual(@"1_My-Wafer-Name_S04.haze",
                FileName5, "FileName5");
        }

        [TestMethod]
        public void TestUnicityofResultTypeExtension()
        {
            var ResultTypeIdList = new List<int>();
            var ExtensionIdList = new List<int>();

            int cnt = 0;
            foreach (int id in Enum.GetValues(typeof(ResultType)))
            {
                if (cnt != 0)
                    Assert.IsFalse(ResultTypeIdList.Contains(id), $"found duplicated in ResultType Enum : {(ResultType)id}");
                ResultTypeIdList.Add(id);
                ExtensionIdList.Add(PMEnumHelper.GetResultExtensionId((ResultType)id));
                cnt++;
            }

            var duplicatesidx = ExtensionIdList
                            .Select((t, i) => new { Index = i, Id = t })
                            .GroupBy(g => g.Id)
                            .Where(g => g.Count() > 1);

            foreach (var group in duplicatesidx)
            {
                Console.WriteLine("Duplicates ResultExtensionId of {0}:", group.Key);
                foreach (var x in group)
                {
                    Console.WriteLine("- Index {0} from restype = {1}", x.Index, (ResultType)ResultTypeIdList[x.Index]);
                }
            }
//            Assert.AreEqual(0, duplicatesidx.ToList().Count);

        }
    }
}
