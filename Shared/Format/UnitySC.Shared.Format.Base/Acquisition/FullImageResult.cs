using System;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Format.Base.Acquisition
{
    public class FullImageResult : IResultDataObject
    {
        #region Constructors

        public FullImageResult(ResultType resType) : this(resType, -1, string.Empty) { }

        public FullImageResult(ResultType resType, long dBResId) : this(resType, dBResId, string.Empty) { }

        public FullImageResult(ResultType resType, long dBResId, string resFilePath)
        {
            ResType = resType;
            CheckResultTypeConsistency();

            DBResId = dBResId;
            ResFilePath = resFilePath;
        }

        internal void CheckResultTypeConsistency()
        {
            if (ResType.GetResultCategory() != ResultCategory.Acquisition)
                throw new ArgumentException($"Bad result category in FullImage : category=<{ResType.GetResultCategory()}> (expected {ResultCategory.Acquisition})");
            if (ResType.GetResultFormat() != ResultFormat.FullImage)
                throw new ArgumentException($"Bad result format in DataHaze : format=<{ResType.GetResultFormat()}> (expected {ResultFormat.FullImage})");
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
            throw new NotImplementedException();
        }

        public bool WriteInFile(string resFilePath, out string sError)
        {
            throw new NotImplementedException();
        }

        #endregion IResultDataObject.Implementation
    }
}
