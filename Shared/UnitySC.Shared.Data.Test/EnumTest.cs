using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;


namespace UnitySC.Shared.Data.Test
{
    [TestClass]
    public class EnumTest
    {
        [TestMethod]
        public void TestActorType_Unicity()
        {
            var idList = new List<int>();
            int cnt = 0;
            foreach (int id in System.Enum.GetValues(typeof(ActorType)))
            {
                if (cnt != 0)
                    Assert.IsFalse(idList.Contains(id), $"found duplicated in ActortType Enum : {(ActorType)id}");
                idList.Add(id);
                cnt++;
            }
        }

        [TestMethod]
        public void TestResultType_Unicity()
        {
            var ResultTypeIdList = new List<int>();

            int cnt = 0;
            foreach (int id in System.Enum.GetValues(typeof(ResultType)))
            {
                if (cnt != 0)
                    Assert.IsFalse(ResultTypeIdList.Contains(id), $"found duplicated in ResultType Enum : {(ResultType)id}");
                ResultTypeIdList.Add(id);
                cnt++;
            }
        }

        [TestMethod]
        public void TestResultTypeExtensionIds_CheckDuplicity()
        {
            var ResultTypeIdList = new List<int>();
            var ExtensionIdList = new List<int>();

            int cnt = 0;
            foreach (int id in System.Enum.GetValues(typeof(ResultType)))
            { 
                if (cnt != 0)
                    Assert.IsFalse(ResultTypeIdList.Contains(id), $"found duplicated in ResultType Enum : {(ResultType)id}  id={id}  "); 
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
                    Console.WriteLine($"- Index {x.Index,-3} from restype = {(ResultType)ResultTypeIdList[x.Index]}");

                    //var strbits = String.Join(" ", Convert.ToString(ResultTypeIdList[x.Index], 2).ToList());
                    //Console.WriteLine($"\t\t| Hex: 0x{ResultTypeIdList[x.Index].ToString("X8"), -10} | {strbits, 65}"); // for debug dev purpose, let thoses lines commented
                }
                Console.WriteLine("-----");
            }

            foreach (var group in duplicatesidx)
            {
                int groupKey = group.Key;
                foreach (var x in group)
                {
                    var XresType = (ResultType)ResultTypeIdList[x.Index];
                    if (XresType == ResultType.NotDefined)
                        break;

                    var Xactortype = XresType.GetActorType();
                    var Xcategory = XresType.GetResultCategory();
                    foreach (var y in group)
                    {
                        var YresType = (ResultType)ResultTypeIdList[y.Index];
                        var Yactortype = YresType.GetActorType();
                        var Ycategory = YresType.GetResultCategory();

                        Assert.AreEqual(Xactortype, Yactortype, $"{XresType} & {YresType} Duplicates Same Extension ID ({groupKey}) without having same actor type");
                        Assert.AreEqual(Xcategory, Ycategory, $"{XresType} & {YresType} Duplicates Same Extension ID ({groupKey}) without having same category");
                    }
                }
            }
        }

        [TestMethod]
        public void TestSomeResultTypeHelper()
        {
            var restype = ResultType.DMT_CurvatureY_Back;
            var actor = PMEnumHelper.GetActorType(restype);
            var Side = PMEnumHelper.GetSide(restype);
            var category = PMEnumHelper.GetResultCategory(restype);
            var resfmt = PMEnumHelper.GetResultFormat(restype);
            int specificID = PMEnumHelper.GetSpecificModuleId(restype);
            int resextID = PMEnumHelper.GetResultExtensionId(restype);

            Assert.AreEqual(ActorType.DEMETER, actor);
            Assert.AreEqual(Side.Back, Side);
            Assert.AreEqual(ResultCategory.Acquisition, category);
            Assert.AreEqual(ResultFormat.FullImage, resfmt);
            Assert.AreEqual(2, specificID);
            Assert.AreEqual((2 << 24 | 64 << 16) >> 16, resextID); //576  --- 24 == PMEnumHelper.ResultSpecificShift & 64 == FullImage coreId & 16 == PMEnumHelper.ResultFormatShift

            restype = ResultType.ADC_ASO;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);

            Assert.AreEqual(ActorType.ADC, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Result, category);
            Assert.AreEqual(ResultFormat.ASO, resfmt);
            Assert.AreEqual(1, specificID);
            Assert.AreEqual((1 << 24 | 3 << 16) >> 16, resextID); //259 --- 24 == PMEnumHelper.ResultSpecificShift & 3 == ASO format coreId & 16 == PMEnumHelper.ResultFormatShift

            restype = ResultType.HLS_Haze_WideFW;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);
            var hlsdirection = HLSResultHelper.GetDirection(restype);
            int hlsacqid = HLSResultHelper.GetHLSAcquisitionTypeId(restype);

            Assert.AreEqual(ActorType.HeLioS, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Acquisition, category);
            Assert.AreEqual(ResultFormat.FullImage_3D, resfmt);
            Assert.AreEqual(HLSDirection.FW_Wide, hlsdirection);
            Assert.AreEqual(3, hlsacqid);
            Assert.AreEqual(hlsacqid | (int)LSDirection.Wide << HLSResultHelper.HLSResultSize, specificID); //67
            Assert.AreEqual((specificID << 24 | ((int)resfmt)) >> 16, resextID); //3393 --- 24 == PMEnumHelper.ResultSpecificShift  & 16 == PMEnumHelper.ResultFormatShift


            restype = ResultType.ANALYSE_TSV;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);

            Assert.AreEqual(ActorType.ANALYSE, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Result, category);
            Assert.AreEqual(ResultFormat.Metrology, resfmt);
            Assert.AreEqual(2, specificID);
            Assert.AreEqual((specificID << 24 | (int)resfmt) >> 16, resextID); //527 --- 24 == PMEnumHelper.ResultSpecificShift & 16 == PMEnumHelper.ResultFormatShift

            restype = ResultType.ANALYSE_Overlay;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);

            Assert.AreEqual(ActorType.ANALYSE, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Result, category);
            Assert.AreEqual(ResultFormat.Metrology, resfmt);
            Assert.AreEqual(8, specificID);
            Assert.AreEqual((specificID << 24 | (int)resfmt) >> 16, resextID); //2063 --- 24 == PMEnumHelper.ResultSpecificShift & 16 == PMEnumHelper.ResultFormatShift

        }

        [TestMethod]
        public void TestEMEHelper()
        {
            // for debug prupose
            //foreach (var emeres in System.Enum.GetValues(typeof(EMEResult)))
            //{
            //    var sbit = String.Join(" ", Convert.ToString(((int)emeres), 2).ToList());
            //    Console.WriteLine($"restype = {(EMEResult)emeres,-20} id = {(int)emeres,-15} bit = |{sbit,64}");
            //}

            /// EME_Visible90
            var restype = ResultType.EME_Visible90;
            var actor = PMEnumHelper.GetActorType(restype);
            var Side = PMEnumHelper.GetSide(restype);
            var category = PMEnumHelper.GetResultCategory(restype);
            var resfmt = PMEnumHelper.GetResultFormat(restype);
            var specificID = PMEnumHelper.GetSpecificModuleId(restype);
            var resextID = PMEnumHelper.GetResultExtensionId(restype);
            var emefilter = EMEResultHelper.GetEMEFilter(restype);
            var emeSource = EMEResultHelper.GetEMELightSource(restype);
            var emeSpeResult = EMEResultHelper.GetEMEAcquisitionTypeId(restype);


            Assert.AreEqual(ActorType.EMERA, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Acquisition, category);
            Assert.AreEqual(ResultFormat.MosaicImage, resfmt);
            Assert.AreEqual(42, specificID);
            Assert.AreEqual((specificID << 24 | (int)resfmt) >> 16, resextID); //10880 --- 24 == PMEnumHelper.ResultSpecificShift & 16 == PMEnumHelper.ResultFormatShift

            Assert.AreEqual(EMEFilter.NoFilter, emefilter);
            Assert.AreEqual(EMELightSource.Visible, emeSource);
            Assert.AreEqual(2, emeSpeResult);

            /// EME_Visible0_LowRes
            restype = ResultType.EME_Visible0_LowRes;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);
            emefilter = EMEResultHelper.GetEMEFilter(restype);
            emeSource = EMEResultHelper.GetEMELightSource(restype);
            emeSpeResult = EMEResultHelper.GetEMEAcquisitionTypeId(restype);


            Assert.AreEqual(ActorType.EMERA, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Acquisition, category);
            Assert.AreEqual(ResultFormat.FullImage, resfmt);
            Assert.AreEqual(41, specificID);
            Assert.AreEqual((specificID << 24 | (int)resfmt) >> 16, resextID); //10560 --- 24 == PMEnumHelper.ResultSpecificShift & 16 == PMEnumHelper.ResultFormatShift

            Assert.AreEqual(EMEFilter.NoFilter, emefilter);
            Assert.AreEqual(EMELightSource.Visible, emeSource);
            Assert.AreEqual(1, emeSpeResult);

            /// EME_UV_BandPass450nm50_LowRes
            restype = ResultType.EME_UV_BandPass450nm50_LowRes;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);
            emefilter = EMEResultHelper.GetEMEFilter(restype);
            emeSource = EMEResultHelper.GetEMELightSource(restype);
            emeSpeResult = EMEResultHelper.GetEMEAcquisitionTypeId(restype);


            Assert.AreEqual(ActorType.EMERA, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Acquisition, category);
            Assert.AreEqual(ResultFormat.FullImage, resfmt);
            Assert.AreEqual(115, specificID);
            Assert.AreEqual((specificID << 24 | (int)resfmt) >> 16, resextID); //29504 --- 24 == PMEnumHelper.ResultSpecificShift & 16 == PMEnumHelper.ResultFormatShift

            Assert.AreEqual(EMEFilter.BandPass450nm50, emefilter);
            Assert.AreEqual(EMELightSource.UV, emeSource);
            Assert.AreEqual(3, emeSpeResult);

            /// EME_UV2_LowPass650nm
            restype = ResultType.EME_UV2_LowPass650nm;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);
            emefilter = EMEResultHelper.GetEMEFilter(restype);
            emeSource = EMEResultHelper.GetEMELightSource(restype);
            emeSpeResult = EMEResultHelper.GetEMEAcquisitionTypeId(restype);


            Assert.AreEqual(ActorType.EMERA, actor);
            Assert.AreEqual(Side.Unknown, Side);
            Assert.AreEqual(ResultCategory.Acquisition, category);
            Assert.AreEqual(ResultFormat.MosaicImage, resfmt);
            Assert.AreEqual(-44, specificID);
            Assert.AreEqual((specificID << 24 | (int)resfmt) >> 16, resextID); //-19328 --- 24 == PMEnumHelper.ResultSpecificShift & 16 == PMEnumHelper.ResultFormatShift

            Assert.AreEqual(EMEFilter.LowPass650nm, emefilter);
            Assert.AreEqual(EMELightSource.UV, emeSource);
            Assert.AreEqual(4, emeSpeResult);

            /// Handle Not a Emera result
            restype = ResultType.DMT_ObliqueLight_Front;
            actor = PMEnumHelper.GetActorType(restype);
            Side = PMEnumHelper.GetSide(restype);
            category = PMEnumHelper.GetResultCategory(restype);
            resfmt = PMEnumHelper.GetResultFormat(restype);
            specificID = PMEnumHelper.GetSpecificModuleId(restype);
            resextID = PMEnumHelper.GetResultExtensionId(restype);
            emefilter = EMEResultHelper.GetEMEFilter(restype);
            emeSource = EMEResultHelper.GetEMELightSource(restype);
            emeSpeResult = EMEResultHelper.GetEMEAcquisitionTypeId(restype);

            Assert.AreEqual(ActorType.DEMETER, actor);
            Assert.AreEqual(Side.Front, Side);
            Assert.AreEqual(ResultCategory.Acquisition, category);
            Assert.AreEqual(ResultFormat.FullImage, resfmt);
            Assert.AreEqual(5, specificID);
            Assert.AreEqual((specificID << 24 | (int)resfmt) >> 16, resextID); //1344 --- 24 == PMEnumHelper.ResultSpecificShift & 16 == PMEnumHelper.ResultFormatShift

            Assert.AreEqual(EMEFilter.Unknown, emefilter);
            Assert.AreEqual(EMELightSource.Unknown, emeSource);
        }
    }
}
