using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract]
    public interface IToolService
    {
        #region Tool

        [OperationContract]
        Response<int> SetTool(Tool tool, int? userId);

        [OperationContract]
        Response<Tool> GetTool(int toolKey, bool includeChambers = false);

        [OperationContract]
        Response<List<Tool>> GetAllTools(bool includeChambers = false);

        #endregion Tool

        #region Chamber

        [OperationContract]
        Response<int> SetChamber(Chamber chamber, int? userId);

        [OperationContract]
        Response<List<Chamber>> GetAllChambers();

        /// <summary>
        /// Get chamber with tool linked
        /// </summary>
        /// <param name="chamberID"></param>
        /// <returns></returns>
        [Obsolete("Use GetChamberFromKeys instead", false)]
        [OperationContract]
        [PreserveReferences]
        Response<Chamber> GetChamber(int chamberId);

        [OperationContract]
        [PreserveReferences]
        Response<Chamber> GetChamberFromKeys(int toolKey, int chamberKey);

        #endregion Chamber

        #region Product and Step

        [OperationContract]
        [PreserveReferences]
        Response<int> SetProduct(Product product, int userId);

        [OperationContract]
        [PreserveReferences]
        Response<int> SetStep(Step step, int userId);

        [OperationContract]
        [PreserveReferences]
        Response<Step> GetStep(int stepId);

        [OperationContract]
        Response<VoidResult> ArchiveAProduct(int productId, int userId);

        [OperationContract]
        Response<VoidResult> RestoreAProduct(int productId, int userId);

        [OperationContract]
        Response<VoidResult> ArchiveAStep(int stepId, int userId);

        [OperationContract]
        Response<VoidResult> RestoreAStep(int stepId, int userId);

        [OperationContract]
        [PreserveReferences]
        Response<List<Product>> GetProductAndSteps(bool takeArchived = false);

        [OperationContract]
        Response<List<WaferCategory>> GetWaferCategories();

        [OperationContract]
        Response<int> SetWaferCategory(WaferCategory waferCategory, int userId);

        [OperationContract]
        Response<List<Dto.Material>> GetMaterials(bool takeArchived = false);

        [OperationContract]
        Response<int> SetMaterial(Dto.Material material, int userId);

        [OperationContract]
        Response<VoidResult> ArchiveMaterial(int materialId, int userId);

        [OperationContract]
        Response<VoidResult> RestoreMaterial(int materialId, int userId);

        #endregion Product and Step

        #region Vid

        [OperationContract]
        Response<int> FirstLowerFreeVid();
       
        [OperationContract]
        Response<int> FirstUpperFreeVid();
        
        [OperationContract]
        Response<int> SetVid(Vid vid, int userId);

        [OperationContract]
        Response<List<Vid>> GetAllVid();

        [OperationContract]
        Response<VoidResult> SetAllVid(List<Vid> vid, int userId);

        [OperationContract]
        Response<VoidResult> RemoveVid(Vid vid, int userId);

        #endregion

        #region other
        [OperationContract]
        Response<bool> CheckDatabaseVersion();
        #endregion
    }
}
