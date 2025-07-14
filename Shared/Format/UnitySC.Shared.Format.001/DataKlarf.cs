using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format._001
{
    public class DataKlarf : IResultDataObject
    {
        private class KlarfItemStats
        {
            public long TotalCount = 0;
            public double TotalSize_um = 0;
        }

        #region IResultDataObject.Implementation

        public ResultType ResType { get; set; }

        public string ResFilePath { get; set; }

        public long DBResId { get; set; }

        public bool ReadFromFile(string resFilePath, out string error)
        {
            if (!ResultFormatExtension.CheckResultTypeFileConsistency(ResType, resFilePath))
            {
                error = $"Result Extension id is not constitent in DataKlarf : ResType=<{ResType}> in <{resFilePath}>";
                return false;
            }

            bool bSuccess;
            try
            {
                bSuccess = KlarfFile.Read(resFilePath, this, out error);
            }
            catch (Exception ex)
            {
                bSuccess = false;
                error = ex.Message;
            }

            return bSuccess;
        }

        public bool WriteInFile(string resFilePath, out string error)
        {
            bool bSuccess = true;
            error = string.Empty;
            try
            {
                KlarfFile.Write(resFilePath, this);
            }
            catch (Exception ex)
            {
                bSuccess = false;
                error = ex.Message;
            }
            return bSuccess;
        }

        public object InternalTableToUpdate(object dicTable)
        {
            var ListRoughBinToUpdate = new List<int>();
            var TableBinList = (List<int>)dicTable;
            foreach (int rbin in _stats.Keys)
            {
                if (!TableBinList.Contains(rbin))
                    ListRoughBinToUpdate.Add(rbin);
            }
            return ListRoughBinToUpdate;
        }

        #endregion IResultDataObject.Implementation

        #region Constructors

        public DataKlarf(ResultType resType) : this(resType, -1, string.Empty) { }

        public DataKlarf(ResultType resType, long dBResId) : this(resType, dBResId, string.Empty) { }

        public DataKlarf(ResultType resType, long dBResId, string resFilePath)
        {
            ResType = resType;
            CheckResultTypeConsistency();

            DBResId = dBResId;
            ResFilePath = resFilePath;

            if (!string.IsNullOrEmpty(ResFilePath))
            {
                if (!ReadFromFile(ResFilePath, out string sError))
                    throw new Exception(sError);
            }
        }

        internal void CheckResultTypeConsistency()
        {
            if (ResType.GetResultCategory() != ResultCategory.Result)
                throw new ArgumentException($"Bad result category in Dataklarf : category=<{ResType.GetResultCategory()}> (expected {ResultCategory.Result})");
            if (ResType.GetResultFormat() != ResultFormat.Klarf)
                throw new ArgumentException($"Bad result format in Dataklarf : format=<{ResType.GetResultFormat()}> (expected {ResultFormat.Klarf})");

            int fmtextid = ResultFormatExtension.GetExtIdFromExtension("001");
            if (ResType.GetResultExtensionId() != fmtextid)
                throw new ArgumentException($"Result Extension id not matched in Dataklarf : extid=<{ResType.GetResultExtensionId()}> (expected {fmtextid})");
        }

        #endregion Constructors

        #region KLARF_HEADER

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public string FileVersion { get; set; } = "1 2";

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public DateTime FileTimeStamp { get; set; } = DateTime.MinValue;

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public string InspectionStationID { get; set; } = string.Empty;

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public string SampleType { get; set; } = "WAFER";

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public DateTime ResultTimeStamp { get; set; } = DateTime.MinValue;

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public string LotID { get; set; } = string.Empty;

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmSampleSize SampleSize { get; set; } = new PrmSampleSize(200);

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public string DeviceID { get; set; } = "RPCS_DEVICE_ID";

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmSetupId SetupID { get; set; } = new PrmSetupId("000", DateTime.MinValue);

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public string StepID { get; set; } = "RPCS_STEP_ID";

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmSampleOrientationMarkType SampleOrientationMarkType { get; set; } = new PrmSampleOrientationMarkType(0);

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmOrientationMarkValue OrientationMarkLocation { get; set; } = new PrmOrientationMarkValue(0);

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public string WaferID { get; set; } = string.Empty;

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public int SlotID { get; set; } = 0;

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmOrientationInstruction OrientationInstructions { get; set; } = new PrmOrientationInstruction(0);

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmOrientationMarkValue InspectionOrientation { get { return OrientationMarkLocation; } set { OrientationMarkLocation = value; } }

        [Category("Header"), Browsable(true), ReadOnly(false)]
        public string TiffFileName { get; set; } = string.Empty;

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmPtFloat DiePitch { get; set; } = new PrmPtFloat(0.0f, 0.0f);

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmPtFloat DieOrigin { get; set; } = new PrmPtFloat(0.0f, 0.0f);

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmPtFloat SampleCenterLocation { get; set; } = new PrmPtFloat(0.0f, 0.0f);

        [Category("Header"), Browsable(false), ReadOnly(true)]
        public string InspectionTest { get; private set; } = "1";

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmYesNo CoordinatesMirrored { get; set; } = new PrmYesNo(false);

        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmYesNo CoordinatesCentered { get; set; } = new PrmYesNo(false);

        [Category("Header"), Browsable(false), ReadOnly(true)]
        public string ClusterClassificationList { get; private set; } = "1 1 200";

        [Category("Header"), Browsable(false), ReadOnly(true)]
        public double AreaPerTest
        {
            get
            {
                double d = SampleSize.WaferDiameter_mm * 1000.0;
                return d * d * 0.5 * Math.PI;
            }
        }

        #endregion KLARF_HEADER

        #region SAMPLE_TEST_PLAN

        [Category("SampleTestPlan"), Browsable(true), ReadOnly(true)]
        public PrmSampleTestPlan SampleTestPlan { get; set; } = new PrmSampleTestPlan();

        #endregion SAMPLE_TEST_PLAN

        #region DEFECTS

        [Category("Defects"), Browsable(false), ReadOnly(true)]
        public string DefectRecordSpec
        {
            get
            {
                string s = _defectListType.List.Count.ToString();
                s += "\n";
                foreach (var dtype in _defectListType.List)
                {
                    s += dtype.ToString();
                    s += " ";
                }
                // remove last whitespece
                s = s.Remove(s.Length - 1);
                return s;
            }
        }

        public void SortByClusterNum()
        {
            string sCat = "CLUSTERNUMBER";
            if (_defectListType.Parse(sCat) != null)
            {
                _defectList = DefectList.OrderBy(x => (int)x.Get(sCat)).ToList();

                string sCatId = "DEFECTID";
                if (_defectListType.Parse(sCatId) != null)
                {
                    int nStart = DefectList.Count % 2;
                    if (nStart == 1)
                        DefectList[0].Set(sCatId, 0);
                    for (int n = nStart; n < DefectList.Count; n += 2)
                    {
                        DefectList[n].Set(sCatId, n);
                        DefectList[n + 1].Set(sCatId, n + 1);
                    }
                }
            }
        }

        // defect type should be added prior to defect
        private PrmDefectTypeList _defectListType = new PrmDefectTypeList();

        public void ResetDefectListType()
        {
            _defectListType = new PrmDefectTypeList();
        }

        public void AddDefectType(Type type, string name)
        {
            _defectListType.Add(type, name);
        }

        public void AddDefectType(string registeredName)
        {
            _defectListType.Add(registeredName);
        }

        public void AddDefectTypes(string[] registeredNames)
        {
            _defectListType.Add(registeredNames);
        }

        // add defect should be done after all defect types has been added
        private List<PrmDefect> _defectList = new List<PrmDefect>();

        public List<PrmDefect> DefectList
        {
            get { return _defectList; }
        }

        public PrmDefect NewDefect()
        {
            return new PrmDefect(_defectListType);
        }

        public void AddDefect(PrmDefect prm)
        {
            DefectList.Add(prm);

            int rgtbin = (int)prm.Get("ROUGHBINNUMBER");
            if (!_stats.ContainsKey(rgtbin))
            {
                _stats.Add(rgtbin, new KlarfItemStats());
            }
            _stats[rgtbin].TotalCount++;
            _stats[rgtbin].TotalSize_um += (double)prm.Get("DEFECTAREA");
        }

        public void WriteDefectList(StreamWriter wstream)
        {
            wstream.WriteLine("DefectList");
            for (int i = 0; i < DefectList.Count - 1; i++)
                wstream.WriteLine(DefectList[i].ToString());
            if (DefectList.Count > 0)
                wstream.WriteLine(DefectList[DefectList.Count - 1]);
            wstream.WriteLine(";");
        }

        #endregion DEFECTS

        #region FOOTER

        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int TESTNO { get { return 1; } }

        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int NDEFECT { get { return DefectList.Count; } }

        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public double DEFDENSITY { get { return NDEFECT / AreaPerTest; } }

        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int NDIE { get; set; } = 1;

        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int NDEFDIE { get; set; } = 1;

        public void WriteFooter(StreamWriter wstream)
        {
            wstream.WriteLine("SummarySpec 5");
            wstream.WriteLine("TESTNO NDEFECT DEFDENSITY NDIE NDEFDIE;");
            wstream.WriteLine("SummaryList");
            wstream.Write(TESTNO); wstream.Write(" ");
            wstream.Write(NDEFECT); wstream.Write(" ");
            wstream.Write(DEFDENSITY.ToString("E10")); wstream.Write(" ");
            wstream.Write(NDIE); wstream.Write(" ");
            wstream.Write(NDEFDIE);
            wstream.WriteLine(";");
            wstream.WriteLine("EndOfFile;");
        }

        #endregion FOOTER

        #region Specific

        public bool IsSquaredWafer { get; set; } = false;   // wafer with rectangular shape
        public PrmPtFloat SquareSizemm { get; set; } = null;// wafer rectangular Size X&Y in mm (complementary information of SampleSize)
        public RcpxItemList<KlarfDefect> DefectViewItemList { get; set; } = null;

        public List<int> RBinKeys { get { return _stats.Keys.ToList(); } }

        public List<ResultDataStats> GetStats()
        {
            var lStats = new List<ResultDataStats>(_stats.Keys.Count * 2);
            foreach (var kvp in _stats)
            {
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.Count, kvp.Key.ToString(), kvp.Value.TotalCount, (int)UnitType.Nb));
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.AreaSize, kvp.Key.ToString(), kvp.Value.TotalSize_um, (int)UnitType.um2));
            }
            return lStats;
        }

        private readonly Dictionary<int, KlarfItemStats> _stats = new Dictionary<int, KlarfItemStats>();

        public long GetCategoryNbDefect(int roughbin)
        {
            long nbdefects = 0;
            if (_stats.ContainsKey(roughbin))
                nbdefects = _stats[roughbin].TotalCount;
            return nbdefects;
        }

        #endregion Specific
    }
}
