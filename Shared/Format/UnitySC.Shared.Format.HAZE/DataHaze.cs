using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.HAZE
{
    public class DataHaze : IResultDataObject
    {
        private const int FORMAT_VERSION = 1;

        #region Constructors

        public DataHaze(ResultType resType) : this(resType, -1, string.Empty) { }

        public DataHaze(ResultType resType, long dBResId) : this(resType, dBResId, string.Empty) { }

        public DataHaze(ResultType resType, long dBResId, string resFilePath)
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
                throw new ArgumentException($"Bad result category in DataHaze : category=<{ResType.GetResultCategory()}> (expected {ResultCategory.Result})");
            if (ResType.GetResultFormat() != ResultFormat.Haze)
                throw new ArgumentException($"Bad result format in DataHaze : format=<{ResType.GetResultFormat()}> (expected {ResultFormat.Haze})");

            int fmtextid = ResultFormatExtension.GetExtIdFromExtension("haze");
            if (ResType.GetResultExtensionId() != fmtextid)
                throw new ArgumentException($"Result Extension id not matched in DataHaze : extid=<{ResType.GetResultExtensionId()}> (expected {fmtextid})");
        }
        
        #endregion Constructors


        #region IResultDataObject.Implementation

        public ResultType ResType { get; set; }

        public string ResFilePath { get; set; }

        public long DBResId { get; set; }

        public object InternalTableToUpdate(object table)
        {
            throw new NotImplementedException();
        }

        public bool ReadFromFile(string resFilePath, out string sError)
        {
            if (!ResultFormatExtension.CheckResultTypeFileConsistency(ResType, resFilePath))
            {
                sError = $"Result Extension id is not constitent in DataHaze : ResType=<{ResType}> in <{resFilePath}>";
                return false;
            }

            bool bSuccess = false;
            sError = "";
            FileStream lStream = null;
            BinaryReader lBinaryReader = null;

            try
            {
                lStream = new FileStream(resFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                lBinaryReader = new BinaryReader(lStream);
                int lVersion = lBinaryReader.ReadInt32();
                if (lVersion < 0 || lVersion > FORMAT_VERSION)
                    throw new Exception("Bad file format version number. DataHaze Results Reading failed.");

                HazeMaps = new List<HazeMap>(3);
                int nbData = lBinaryReader.ReadInt32();
                for (int i = 0; i < nbData; i++)
                {
                    var dt = new HazeMap();
                    dt.Read(lBinaryReader);
                    HazeMaps.Add(dt);
                }
                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (lBinaryReader != null)
                    lBinaryReader.Close();
                if (lStream != null)
                    lStream.Close();
            }
            return bSuccess;
        }

        public bool WriteInFile(string resFilePath, out string sError)
        {
            sError = "";
            FileStream lStream = null;
            BinaryWriter lBinaryWriter = null;
            bool bSuccess = false;
            try
            {
                lStream = new FileStream(resFilePath, FileMode.Create);
                lBinaryWriter = new BinaryWriter(lStream);
                lBinaryWriter.Write(DataHaze.FORMAT_VERSION);
                lBinaryWriter.Write(HazeMaps.Count);
                foreach (var map in HazeMaps)
                {
                    map.Write(lBinaryWriter);
                }
                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (lBinaryWriter != null)
                    lBinaryWriter.Close();
                if (lStream != null)
                    lStream.Close();
            }
            return bSuccess;
        }

        #endregion IResultDataObject.Implementation



        #region HazeDataContents

        public string LocalPath { set; get; }
        public int Version { set; get; } = FORMAT_VERSION;

        public List<HazeMap> HazeMaps = new List<HazeMap>(3);


        public List<ResultDataStats> GetStats()
        {
            var lStats = new List<ResultDataStats>();

            for (int i = 0; i < HazeMaps.Count; i++)
            {
                var hazeMap = HazeMaps[i];
                var hazeType = (HazeType)i;

                // TODO [TLA] UnitType PPM ?
                // Stats to generate and to be store in DB to be defined
                // Note RTI - I think of Haze Mean, Max, Min, StdDev 
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.Min, hazeType.ToString(), hazeMap.Min_ppm, (int)UnitType.Nb));
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.Max, hazeType.ToString(), hazeMap.Max_ppm, (int)UnitType.Nb));
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.Mean, hazeType.ToString(), hazeMap.Mean_ppm, (int)UnitType.Nb));
                lStats.Add(new ResultDataStats(DBResId, (int)ResultValueType.StdDev, hazeType.ToString(), hazeMap.Stddev_ppm, (int)UnitType.Nb));
            }
            
            return lStats;
        }

        #endregion HazeDataContents
    }
}
