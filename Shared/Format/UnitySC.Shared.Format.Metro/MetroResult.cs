using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.Format.Metro
{
    [Serializable]
    public class MetroResult : IResultDataObject
    {
        public MeasureResultBase MeasureResult { get; set; }

        [XmlIgnore]
        public string ResFilePath { get; set; }

        [XmlIgnore]
        public long DBResId { get; set; }

        [XmlAttribute("FileVersion")]
        public string FileVersion { get; set; } = "1.0.0";
           
        protected MetroResult() : this(ResultType.Empty,-1,string.Empty)
        {
            // default constructor mandatory for XML.Deserialize should not be used as it
        }

        public MetroResult(ResultType resType) : this(resType, -1, string.Empty) { }

        public MetroResult(ResultType resType, long dBResId) : this(resType, dBResId, string.Empty) { }

        public MetroResult(ResultType resType, long dBResId, string resFilePath)
        {
            ResType = resType;
            if(ResType != ResultType.Empty)
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
                throw new ArgumentException($"Bad result category in DataHaze : category=<{ResType.GetResultCategory()}> (expected {ResultCategory.Result})");
            if (ResType.GetResultFormat() != ResultFormat.Metrology)
                throw new ArgumentException($"Bad result format in DataHaze : format=<{ResType.GetResultFormat()}> (expected {ResultFormat.Metrology})");

            switch (ResType)
            {             
                case ResultType.ANALYSE_Overlay:
                case ResultType.ANALYSE_CD:
                case ResultType.ANALYSE_EBR:                        
                case ResultType.ANALYSE_Roughness:
                    throw new NotImplementedException($"<{ResType}> is not yet implemented in {ResultFormat.Metrology} format");
                
                case ResultType.ANALYSE_TSV:
                case ResultType.ANALYSE_NanoTopo:
                case ResultType.ANALYSE_Thickness:
                case ResultType.ANALYSE_Topography:
                case ResultType.ANALYSE_Step:
                case ResultType.ANALYSE_EdgeTrim:
                case ResultType.ANALYSE_Trench:
                case ResultType.ANALYSE_Pillar:
                case ResultType.ANALYSE_PeriodicStructure:
                case ResultType.ANALYSE_Bow:
                case ResultType.ANALYSE_Warp:
                default:
                    break;
            }
        }
        
        [XmlIgnore]
        public ResultType ResType { get; set; }

        public object InternalTableToUpdate(object table)
        {
            throw new NotImplementedException();
        }

        public bool ReadFromFile(string resFilePath, out string sError)
        {
            bool res = false;
            sError = string.Empty;
            try
            {
                MeasureResult = XML.Deserialize<MetroResult>(resFilePath)?.MeasureResult;
                res = ResultFormatExtension.CheckResultTypeFileConsistency(ResType, resFilePath);
                if (!res)
                {
                    sError = $"Result Extension id is not consistent : ResType=<{ResType}> in <{resFilePath}>"; 
                }
               
            }
            catch (Exception ex)
            {
                sError = ex.ToString();
            }
            return res;
        }

        public bool WriteInFile(string resFilePath, out string sError)
        {
            bool res = false;
            sError = string.Empty;
            try
            {
                res = ResultFormatExtension.CheckResultTypeFileConsistency(ResType, resFilePath);
                if (!res)
                {
                    sError = $"Result file name Extension does not match result type : ResType=<{ResType}> in <{resFilePath}>";
                }
                else
                {
                    this.Serialize(resFilePath);
                }
            }
            catch (Exception ex)
            {
                sError = ex.ToString();
            }
            return res;
        }

        public List<ResultDataStats> GenerateStatisticsValues()
        {
            return MeasureResult.GenerateStatisticsValues(DBResId);
        }
    }
}
