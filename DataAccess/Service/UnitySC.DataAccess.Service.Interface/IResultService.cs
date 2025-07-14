using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{
    /// <summary>
    /// Service to access to result data
    /// </summary>

    [ServiceContract(CallbackContract = typeof(IResultServiceCallback))]
    public interface IResultService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<List<ResultQuery>> GetProducts();

        [OperationContract]
        Response<List<string>> GetRecipes(int? pToolKey);

        [OperationContract]
        Response<List<ResultChamberQuery>> GetChambers(int? pToolKey, int? pToolId = null, bool HasAnyResult = true);

        [OperationContract]
        Response<List<Job>> GetSearchJobs(SearchParam pQuery);

        [OperationContract]
        [PreserveReferences]
        Response<List<ProcessModuleResult>> GetJobProcessModulesResults(int pToolId, int pJobId, bool bQueryAcqData = false);

        [OperationContract]
        Response<List<string>> GetLots(int? pToolKey);

        [OperationContract]
        Response<List<ResultQuery>> GetTools();

        [OperationContract]
        Response<List<ResultQuery>> GetToolsKey();

        /// <summary>
        /// Add/Increase result scan request priority in scanner queue.
        /// </summary>
        /// <param name="resultDBId"> database result id primary key </param>
        [OperationContract]
        Response<VoidResult> ResultScanRequest(long resultDBId, bool isAcquisition);

        [OperationContract]
        Response<VoidResult> ResultReScanRequest(long resultDBId, bool isAcquisition);

        /// <summary>
        /// Get klarfs settings synchronyze with result scanner data.
        /// </summary>
        [OperationContract]
        Response<KlarfSettingsData> GetKlarfSettingsFromScanner();

        /// <summary>
        /// Get klarfs settings directlty from databse tables.
        /// </summary>
        [OperationContract]
        Response<KlarfSettingsData> GetKlarfSettingsFromTables();

        /// <summary>
        /// Get colormap name for haze result
        /// </summary>
        [OperationContract]
        Response<string> GetHazeSettingsFromTables();

        [OperationContract]
        Response<bool> IsConnectionAvailable();

        [OperationContract]
        Response<VoidResult> RemoteUpdateKlarfSizeBins(SizeBins szbins);

        [OperationContract]
        Response<VoidResult> RemoteUpdateKlarfDefectBins(DefectBins defbins);

        [OperationContract]
        Response<VoidResult> RemoteUpdateHazeColorMap(string colormapname);

        [OperationContract]
        Response<bool> CheckDatabaseVersion();

        [OperationContract]
        Response<List<ResultQuery>> RetrieveToolIdFromToolKey(int toolKey);

        [OperationContract]
        Response<ResultQuery> RetrieveToolKeyFromToolId(int toolId);

        [OperationContract]
        Response<int> RetrieveChamberIdFromKeys(int toolKey, int chamberKey);

    }
}
