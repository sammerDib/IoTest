using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Format.Base
{
    public interface IResultDataFactory
    {
        /// <summary>
        /// Create result data object
        /// </summary>
        /// <param name="resType">Result data kind</param>
        /// <param name="databaseResultId">Result Database id</param>
        /// <returns> result data object  </returns>
        IResultDataObject Create(ResultType resType, long databaseResultId);

        /// <summary>
        /// Create Result data object
        /// </summary>
        /// <param name="resType"></param>
        /// <param name="databaseResultId"></param>
        /// <param name="resFilePath"></param>
        /// <returns>result data object </returns>
       IResultDataObject CreateFromFile(ResultType resType, long databaseResultId, string resFilePath);

        /// <summary>
        /// Restreive View format according to result kind
        /// </summary>
        /// <param name="resType">Result data kind</param>
        /// <returns> IResultDisplay result view format object </returns>
        IResultDisplay GetDisplayFormat(ResultType resType);

        /// <summary>
        /// Generate Thumbnail of this result, save PNG in target LotThumbnail directory
        /// </summary>
        /// <param name="data">result data object </param>
        /// <param name="inprm"> various input parameters depending of result type </param>
        /// <returns>true if success, false otherwise</returns>
        bool GenerateThumbnailFile(IResultDataObject data, params object[] inprm);

        /// <summary>
        /// generate Statistics value to Insert in DB
        /// </summary>
        /// <param name="data"></param>
        /// <param name="innprm"></param>
        /// <returns></returns>
        List<ResultDataStats> GenerateStatisticsValues(IResultDataObject data, params object[] innprm);

        /// <summary>
        /// Klarf display Size Bins store in DB
        /// </summary>
        /// <returns></returns>
        SizeBins KlarfDisplay_SizeBins();

        /// <summary>
        /// Klarf display Defect Bins store in DB
        /// </summary>
        /// <returns></returns>
        DefectBins KlarfDisplay_DefectBins();
    }
}
