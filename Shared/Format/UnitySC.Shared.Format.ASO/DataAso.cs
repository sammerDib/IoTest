using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.ASO
{
    public class DataAso : IResultDataObject
    {
        #region Constructors

        public DataAso(ResultType resType) : this(resType, -1, string.Empty) { }

        public DataAso(ResultType resType, long dBResId) : this(resType, dBResId, string.Empty) { }

        public DataAso(ResultType resType, long dBResId, string resFilePath)
        {
            ResType = resType;
            CheckResultTypeConsistency();

            DBResId = dBResId;
            ResFilePath = resFilePath;
            HasNoThumbnails = true;

            _clusterList = new List<ClusterReport>(1024);
            _reportDetailList = new List<DetailReport>(16);
            _dicoCategoryColor = new Dictionary<string, Color>();

            if (!string.IsNullOrEmpty(ResFilePath))
            {
                if (!ReadFromFile(ResFilePath, out string sError))
                    throw new Exception(sError);
            }
        }

        internal void CheckResultTypeConsistency()
        {
            if (ResType.GetResultCategory() != ResultCategory.Result)
                throw new ArgumentException($"Bad result category in DataAso : category=<{ResType.GetResultCategory()}> (expected {ResultCategory.Result})");
            if (ResType.GetResultFormat() != ResultFormat.ASO)
                throw new ArgumentException($"Bad result format in DataAso : format=<{ResType.GetResultFormat()}> (expected {ResultFormat.ASO})");

            int fmtextid = ResultFormatExtension.GetExtIdFromExtension("aso");
            if (ResType.GetResultExtensionId() != fmtextid)
                throw new ArgumentException($"Result Extension id not matched in DataAso : extid=<{ResType.GetResultExtensionId()}> (expected {fmtextid})");
        }
        
        #endregion Constructors

        #region IResultDataObject.Implementation

        public ResultType ResType { get; set; }

        public string ResFilePath { get; set; }

        public long DBResId { get; set; }

        public object InternalTableToUpdate(object table)
        {
            foreach (var cr in ClusterList)
                FeedColorCategory(cr);
            return _dicoCategoryColor;
        }

        public bool ReadFromFile(string resFilePath, out string sError)
        {
            _clusterList.Clear();
            _reportDetailList.Clear();

            HasNoThumbnails = true;
            sError = string.Empty;
            ResFilePath = resFilePath;
            if (!File.Exists(resFilePath))
            {
                sError = "This file path {" + resFilePath + "} doesn't exist !";
                return false;
            }

            if (!ResultFormatExtension.CheckResultTypeFileConsistency(ResType, resFilePath))
            {
                sError = $"Result Extension id is not constitent in DataASO : ResType=<{ResType}> in <{resFilePath}>";
                return false;
            }

            // retrieve local path need to retreaive thumbnail
            LocalPath = Path.GetDirectoryName(resFilePath);
            if (LocalPath.EndsWith("\\") == false)
                LocalPath += "\\";

            bool bSuccess = true;
            try
            {
                using (var sr = new StreamReader(resFilePath))
                {
                    string sASOFileDataContents = sr.ReadToEnd();
                    sASOFileDataContents = Regex.Replace(sASOFileDataContents, @"\t|\n|\r", "");

                    if (!ParseASO(sASOFileDataContents, out var parseErrors))
                    {
                        sError = $"Error occurs while parsing {resFilePath} : =>\n";
                        sError += parseErrors.ToString();
                        bSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                sError = $"Uncatch Exception thrown in ReadFromFile : {resFilePath} => {ex.Message}";
                bSuccess = false;
            }
            return bSuccess;
        }

        public bool WriteInFile(string resFilePath, out string sError)
        {
            sError = string.Empty;
            bool bSuccess = true;
            try
            {
                using (var sw = new StreamWriter(resFilePath))
                {
                    // Header
                    //.......
                    WriteHeaders(sw);

                    // Write each cluster
                    //...................
                    foreach (var cluster in ClusterList.OrderBy(cl => cl.ClusterNumber))
                    {
                        cluster.WriteCluster(sw, Sep, Eol);
                    }
                }
            }
            catch (Exception ex)
            {
                sError = $"Error while writing aso {resFilePath} - msg = {ex.Message}";
                bSuccess = false;
            }
            return bSuccess;
        }

        public Color GetColorCategory(string sCategoryName)
        {
            if (_dicoCategoryColor.ContainsKey(sCategoryName))
                return _dicoCategoryColor[sCategoryName];
            return Color.Transparent;
        }

        public bool HasNoThumbnails { get; set; } 

        #endregion IResultDataObject.Implementation

        #region AsoDataContents

        public static string[] ParametersSectionName = new string[]{
            "REPORT_GLOBAL", //0
            "REPORT_HEADER", //1
            "REPORT_DETAIL", //2
            "CLUSTER_DESCR", //3
            "REPORT_DIEGRID",//4
            "REPORT_VERSION" //5
        };

        private const char Sep = ';';    // Separator
        private const char Eol = '#';   // End of Line

        public enum OrientationMark
        {
            en_NOTCH,
            en_FLAT,
            en_DOUBLE_FLAT
        }

        private readonly List<ClusterReport> _clusterList;
        public List<ClusterReport> ClusterList { get => _clusterList; }

        private readonly List<DetailReport> _reportDetailList;
        public List<DetailReport> ReportDetailList { get => _reportDetailList; }

        private readonly Dictionary<string, Color> _dicoCategoryColor;

        public int NumberOfBloc { set; get; }
        public int NumberOfCluster { set; get; }
        public int NumberOfDefect { set; get; }

        public string ADCRecipeFile { set; get; }
        public OrientationMark OrientationMarkType { set; get; }

        public string WaferID { set; get; }
        public int SlotID { set; get; }
        public string ToolRecipe { set; get; }
        public string LotID { set; get; }
        public string ToolName { set; get; }

        public bool IsSquareWafer { set; get; }

        public int WaferSizeX_mm { set; get; }
        public int WaferSizeY_mm { set; get; }

        // Die origin et die pitch en µm idem que Klarf - pour display die grid en cas de die to die
        public double DieOriginX { set; get; }

        public double DieOriginY { set; get; }
        public double DiePitchX { set; get; }
        public double DiePitchY { set; get; }
        public bool UseDieGridDisplay { set; get; }

        public string LocalPath { set; get; }
        public int Version { set; get; } = 0;

        #endregion AsoDataContents

        private bool ParseASO(string datafileContent, out StringBuilder errorMsgList)
        {
            bool bParseWithoutErrors = true;

            errorMsgList = new StringBuilder();
            string[] sLines = datafileContent.Split(Eol);

            int LineNum = 1;
            foreach (string strLine in sLines)
            {
                string[] sTab = strLine.Split(Sep);
                if (strLine.ToUpper().Contains(ParametersSectionName[3].ToUpper())) // CLUSTER_DESCR
                {
                    // CLUSTER_DESCR;num_cluster; num_bloc;BlocSelect;userlabel;nb_defect;totalclustersize; maxclustersize; micronposx; micronposy;micronsizex; micronsizey; unit;
                    // pixelposx; pixelposy; pictureposx; pictureposy;pixelsizex; pixelsizey; thumbnailgreylevel; thumbnailbinary; columnnumber; linenumber; virtualblocnumber;
                    // colorname; diex; diey; cutumerlabel; typereportsize; iskilling; [allcaract]
                    // répété N fois (pour chaque type classe de defaut)

                    try
                    {
                        var Cluster = new ClusterReport();
                        if (Cluster.ParseCluster(sTab)) // return true if cluster is selected for report
                        {
                            ClusterList.Add(Cluster);  // renvoi que de la couche significative.
                            FeedColorCategory(Cluster);

                            if (HasNoThumbnails)
                            {
                                //Method B : check if all thumbnail path are null or empty
                                bool ThumbExist = false;
                                if (!string.IsNullOrEmpty(Cluster.ThumbnailGreyLevelFilePath) || !string.IsNullOrWhiteSpace(Cluster.ThumbnailGreyLevelFilePath))
                                    ThumbExist = true;
                                else if (!string.IsNullOrEmpty(Cluster.ThumbnailBinaryFilePath) || !string.IsNullOrWhiteSpace(Cluster.ThumbnailBinaryFilePath))
                                    ThumbExist = true;
               
                                //// Method A :  to Heavy since its check s all file.exists that are not null
                                ////check if all thumbnails are empty
                                //if (!String.IsNullOrEmpty(Cluster.ThumbnailGreyLevelFilePath) || !String.IsNullOrWhiteSpace(Cluster.ThumbnailGreyLevelFilePath))
                                //{
                                //    ThumbExist = File.Exists(LocalPath + Cluster.ThumbnailGreyLevelFilePath);
                                //}
                                //if (!ThumbExist)
                                //{
                                //    if (!String.IsNullOrEmpty(Cluster.ThumbnailBinaryFilePath) || !String.IsNullOrWhiteSpace(Cluster.ThumbnailBinaryFilePath))
                                //    {
                                //        ThumbExist = File.Exists(LocalPath + Cluster.ThumbnailBinaryFilePath);
                                //    }
                                //}
                                //// attention dont use else if here need to check binary

                                if (ThumbExist)
                                    HasNoThumbnails = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsgList.AppendLine($"Exception in CLUSTER_DESCR line no {LineNum} - msg = {ex.Message}");
                        bParseWithoutErrors = false;
                    }
                    continue;
                }

                if (strLine.ToUpper().Contains(ParametersSectionName[5].ToUpper())) //REPORT_VERSION
                {
                    // REPORT_VERSION;version_num
                    if (sTab.Length == 2)
                    {
                        try
                        {
                            Version = Convert.ToInt32(sTab[1], CultureInfo.InvariantCulture.NumberFormat);
                        }
                        catch (Exception ex)
                        {
                            errorMsgList.AppendLine($"Exception in REPORT_VERSION line no {LineNum} - msg = {ex.Message}");
                            bParseWithoutErrors = false;
                        }
                    }
                    else
                    {
                        errorMsgList.AppendLine($"Invalid data format in REPORT_VERSION line no {LineNum} - Items numbers seen differs from expected");
                        bParseWithoutErrors = false;
                    }
                    continue;
                }

                if (strLine.ToUpper().Contains(ParametersSectionName[2].ToUpper())) //REPORT_DETAIL
                {
                    // REPORT_DETAIL;Label;Count;Size répété N fois (pour chaque type classe de defaut)
                    if (sTab.Length == 4)
                    {
                        try
                        {
                            var pDetail = new DetailReport
                            {
                                Label = sTab[1],
                                Number = Convert.ToInt32(sTab[2], CultureInfo.InvariantCulture.NumberFormat),
                                Size = Convert.ToDouble(sTab[3], CultureInfo.InvariantCulture.NumberFormat)
                            };

                            _reportDetailList.Add(pDetail);
                        }
                        catch (Exception ex)
                        {
                            errorMsgList.AppendLine($"Exception in REPORT_DETAIL line no {LineNum} - msg = {ex.Message}");
                            bParseWithoutErrors = false;
                        }
                    }
                    else
                    {
                        errorMsgList.AppendLine($"Invalid data format in REPORT_DETAIL line no {LineNum} - Items numbers seen differs from expected");
                        bParseWithoutErrors = false;
                    }

                    continue;
                }

                if (strLine.ToUpper().Contains(ParametersSectionName[0].ToUpper())) //REPORT_GLOBAL
                {
                    // REPORT_GLOBAL;bloc_count;cluster_count;defect_count;recipe_file;orientation_mark
                    if (sTab.Length == 6)
                    {
                        try
                        {
                            NumberOfBloc = Convert.ToInt32(sTab[1], CultureInfo.InvariantCulture.NumberFormat);
                            NumberOfCluster = Convert.ToInt32(sTab[2], CultureInfo.InvariantCulture.NumberFormat);
                            NumberOfDefect = Convert.ToInt32(sTab[3], CultureInfo.InvariantCulture.NumberFormat);
                            ADCRecipeFile = sTab[4];
                            OrientationMarkType = (OrientationMark)Convert.ToInt32(sTab[5], CultureInfo.InvariantCulture.NumberFormat);
                        }
                        catch (Exception ex)
                        {
                            errorMsgList.AppendLine($"Exception in REPORT_GLOBAL line no {LineNum} - msg = {ex.Message}");
                            bParseWithoutErrors = false;
                        }
                    }
                    else
                    {
                        errorMsgList.AppendLine($"Invalid data format in REPORT_GLOBAL line no {LineNum} - Items numbers seen differs from expected");
                        bParseWithoutErrors = false;
                    }
                    continue;
                }

                if (strLine.ToUpper().Contains(ParametersSectionName[1].ToUpper())) //REPORT_HEADER
                {
                    // REPORT_HEADER;WaferID;SlotID;RecipeID;LotID;ToolsName
                    if (sTab.Length >= 6) // gestion des anciennes versions
                    {
                        try
                        {
                            WaferID = sTab[1];
                            SlotID = Convert.ToInt32(sTab[2], CultureInfo.InvariantCulture.NumberFormat);
                            ToolRecipe = sTab[3];
                            LotID = sTab[4];
                            ToolName = sTab[5];

                            if (sTab.Length >= 9) // gestion des anciennes versions
                            {
                                IsSquareWafer = Convert.ToBoolean(sTab[6]);
                                WaferSizeX_mm = Convert.ToInt32(sTab[7], CultureInfo.InvariantCulture.NumberFormat);
                                WaferSizeY_mm = Convert.ToInt32(sTab[8], CultureInfo.InvariantCulture.NumberFormat);
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMsgList.AppendLine($"Exception in REPORT_HEADER line no {LineNum} - msg = {ex.Message}");
                            bParseWithoutErrors = false;
                        }
                    }
                    else
                    {
                        errorMsgList.AppendLine($"Invalid data format in REPORT_HEADER line no {LineNum} - Items numbers seen differs from expected");
                        bParseWithoutErrors = false;
                    }

                    continue;
                }

                if (strLine.ToUpper().Contains(ParametersSectionName[4].ToUpper())) //REPORT_DIEGRID
                {
                    // en µm
                    // REPORT_DIEGRID;DieOriginX;DieOriginY;DiePitchX;DiePitchY#
                    if (sTab.Length >= 4)
                    {
                        try
                        {
                            DieOriginX = Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat);
                            DieOriginY = Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat);
                            DiePitchX = Convert.ToDouble(sTab[3], CultureInfo.InvariantCulture.NumberFormat);
                            DiePitchY = Convert.ToDouble(sTab[4], CultureInfo.InvariantCulture.NumberFormat);
                            UseDieGridDisplay = DiePitchX > 0.0 && DiePitchY > 0.0;
                        }
                        catch (Exception ex)
                        {
                            errorMsgList.AppendLine($"Exception in REPORT_DIEGRID line no {LineNum} - msg = {ex.Message}");
                            bParseWithoutErrors = false;
                        }
                    }
                    else
                    {
                        errorMsgList.AppendLine($"Invalid data format in REPORT_DIEGRID line no {LineNum} - Items numbers seen differs from expected");
                        bParseWithoutErrors = false;
                    }
                    continue;
                }

                // increment line
                LineNum++;
            }

            return bParseWithoutErrors;
        }

        private void WriteHeaders(StreamWriter sw)
        {
            //REPORT_VERSION; version_num
            sw.WriteLine(ParametersSectionName[5] + Sep + Version + Eol);

            //REPORT_GLOBAL;bloc_count;cluster_count;defect_count;recipe_file;orientation_mark #
            sw.WriteLine(ParametersSectionName[0] + Sep + NumberOfBloc + Sep + NumberOfCluster + Sep + NumberOfDefect + Sep + ADCRecipeFile + Sep + ((int)OrientationMarkType).ToString() + Eol);

            //REPORT_HEADER;WaferID;SlotID;RecipeID;LotID;ToolsName;isSquareWafer;valueWaferSizeX_mm;valueWaferSizeY_mm#
            sw.WriteLine(ParametersSectionName[1] + Sep + WaferID + Sep + SlotID + Sep + ToolRecipe + Sep + LotID + Sep + ToolName + Sep + IsSquareWafer + Sep + WaferSizeX_mm + Sep + WaferSizeY_mm + Eol);

            //REPORT_DIEGRID; DieOriginX; DieOriginY; DiePitchX; DiePitchY#
            sw.WriteLine(ParametersSectionName[4] + Sep + DieOriginX + Sep + DieOriginY + Sep + DiePitchX + Sep + DiePitchY + Eol);

            //-------------------------------------------------------------
            // REPORT_DETAIL;Label;Count;Size répété N fois (pour chaque type classe de defaut)
            //-------------------------------------------------------------
            foreach (var detailrep in _reportDetailList)
            {
                sw.WriteLine(ParametersSectionName[2] + Sep + detailrep.Label + Sep + detailrep.Number + Sep + detailrep.Size + Eol);
            }
        }

        private void FeedColorCategory(ClusterReport cr)
        {
            FeedColorCategory(cr.UserLabel, cr.Color);
        }

        private void FeedColorCategory(string category, Color clr)
        {
            if (!_dicoCategoryColor.ContainsKey(category))
            {
                _dicoCategoryColor.Add(category, clr);
            }
            //else { _dicoCategoryColor[category]=clr} // first add no update
        }



        #region Specific

        public RcpxItemList<AsoDefect> DefectViewItemList { get; set; } = null;

        public List<ResultDataStats> GetStats()
        {
            var lStats = new List<ResultDataStats>(_reportDetailList.Count * 2);

            foreach (var report in _reportDetailList)
            {
                int catclr = GetColorCategory(report.Label).ToArgb();
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.Color, report.Label, catclr, (int)UnitType.NoUnit));
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.Count, report.Label, report.Number, (int)UnitType.Nb));
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.AreaSize, report.Label, report.Size, (int)UnitType.um2));
            }
            return lStats;
        }

        #endregion Specific
    }
}
