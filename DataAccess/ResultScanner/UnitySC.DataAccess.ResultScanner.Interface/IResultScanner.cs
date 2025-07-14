using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Composer;

namespace UnitySC.DataAccess.ResultScanner.Interface
{
    /// <summary>
    /// Service pour accéder aux Scan de resultats, génrération imagettes/stats et Composition de Path
    /// </summary>
    public interface IResultScanner
    {
        /// <summary>
        /// Start service internal Threads and Initialisation
        /// </summary>
        void Start();

        /// <summary>
        /// Stop service
        /// </summary>
        void Stop();

        /// <summary>
        /// Add a result to the Scan Queue, if this result has already been entered its Priority is raised.
        /// </summary>
        /// <param name="resultDBId"> database result id primary key </param>
        void ResultScanRequest(long resultDBId, bool isAcquisition);

        /// <summary>
        /// Request an existing result already scan to be rescan, result stast values and thumbnail will be orverwrite.
        /// internstatus will be reinitialize to default and result will be added to the scan queue with a signifcant priority
        /// </summary>
        /// <param name="resultDBId"> database result id primary key </param>
        void ResultReScanRequest(long resultDBId, bool isAcquisition);

        /// <summary>
        /// Request an Update of internal defect rough bin data. KlarfRoughSettings has been updated externaly need to refresh it.
        /// </summary>
        void RefreshRoughBinSettings();

        /// <summary>
        /// Build Full Result file Path
        /// </summary>
        /// <param name="resultDBId"> database result id primary key </param>
        /// <returns> path with extension </returns>
        string ResultPathFromResultId(long resultDBId);

        /// <summary>
        /// Build Full Result file Path
        /// </summary>
        /// <param name="ResultDBQueryObj"> DTO Result SQL object return by SQL Query </param>
        /// <returnspath with extension</returns>
        string ResultPathFromResult(object resultDBQueryObj);

        /// <summary>
        /// Update (add, remaove, update) klarf size bins table from a remote ressource
        /// </summary>
        /// <param name="szbins"></param>
        void RemoteUpdateKlarfSizeBins(SizeBins szbins);

        /// <summary>
        /// Update (add, remaove, update) klarf size bins table from a remote ressource
        /// </summary>
        /// <param name="defbins"></param>
        void RemoteUpdateKlarfDefectBins(DefectBins defbins);

        void RemoteUpdateHazeColorMap(string colormapname);

        /// <summary>
        /// Update Paths of result and result thumbnial of ResultDBQueryObj
        /// </summary>
        /// <param name="ResultDBQueryObj"> DTO Result SQL object - modified object member</param>
        void InitResultPathFromResult(object resultDBQueryObj);

        /// <summary>
        /// Build Result file name (without extension) according to model set.
        /// </summary>
        string BuildResultFileName(ResultPathParams prm);

        /// <summary>
        /// Build Result directory name according to model set.
        /// </summary>
        string BuildResultDirectoryName(ResultPathParams prm);

        void SetModel_FileName(string modelFile);

        void SetModel_DirectoryName(string modelDirectory);

        SizeBins GetKlarfSizeBins();

        DefectBins GetKlarfDefectBins();

    }
}
