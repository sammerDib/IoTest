using System;
using System.ServiceModel;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{

    [ServiceContract]
    public interface IRegisterResultService
    {

        #region Results (Inspection / Metrology)

        // Post Process / Metrology result register (ADC / Metrology)
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        [OperationContract]
        Response<OutPreRegister> PreRegisterResult(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, ResultType resultType, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None, string resultLabelName = null);

        // Post Process / Metrology result register (ADC / Metrology) - with same parent (eg. restype or idx different but same jobid or wafer/recipe/chamber)
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        [OperationContract]
        Response<OutPreRegister> PreRegisterResult_SameParent(long parentResultId, DateTime moduleStartRecipeTime, ResultType resultType, byte idx = 0, ResultFilterTag tag = ResultFilterTag.None, string resultLabelName = null);

        // Post Process / Metrology result register (ADC / Metrology)
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        // InPreRegister object may be modified after registration
        [OperationContract]
        Response<OutPreRegister> PreRegisterResultWithPreRegisterObject(InPreRegister preRegister); 

        // Post Process result update (ADC, ResultScanner, Analyse)
        // WARNING : before this call, result should have been copied to its target folder and available for reading 
        // - resultItemId is the Database ResulItem primary key id - same as OutPreRegister.InternalDBResId
        [OperationContract]
        Response<bool> UpdateResultState(long resultItemId, ResultState resultState);
        #endregion

        #region Acquisitions Maps/Images (2D/3D - full)


        // Acquisition result map (PM) 
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        [OperationContract]
        Response<OutPreRegisterAcquisition> PreRegisterAcquisition(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, string filename, string pathname, ResultType resultType, byte idx = 0, string acqLabelName = null, ResultFilterTag tag = ResultFilterTag.None);

        // Acquisition result map (PM) - with same parent eg. same Parent Root Folder
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        [OperationContract]
        Response<OutPreRegisterAcquisition> PreRegisterAcquisition_SameParent(long parentResultId, DateTime moduleStartRecipeTime, string filename, ResultType resultType, byte idx = 0, string acqLabelName = null, ResultFilterTag tag = ResultFilterTag.None);

        // Acquisition result map (PM)
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        // InPreRegisterAcquisition object may be modified after registration
        [OperationContract]
        Response<OutPreRegisterAcquisition> PreRegisterAcquisitionWithPreRegisterObject(InPreRegisterAcquisition preRegisterAcq);

        // Acquisition result update (PM)
        // WARNING : before this call, acq results should have been copied to its target folder and available for reading 
        // - resulAcqItemAcqId is th Database ResulAcqItem primary key id - same as OutPreRegisterAcquisition.InternalDBResId
        [OperationContract]
        Response<bool> UpdateResultAcquisitionState(long resulAcqItemAcqId, ResultState resultacqState, string thumbnailExtension = null);
        #endregion

        [OperationContract]
        Response<bool> IsDatabaseConnectionOk();

        [OperationContract]
        Response<bool> CheckDatabaseVersion();
    }
}
