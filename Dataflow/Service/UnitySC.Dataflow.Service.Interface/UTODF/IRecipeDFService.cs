using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Dataflow.Service.Interface
{
    // UTO Client -> PM (UTO.API) Serveur
    [ServiceContract]
    public interface IRecipeDFService
    {
        [OperationContract]
        Response<VoidResult> SelectRecipe(DataflowRecipeInfo dfRecipeInfo);

        [OperationContract]
        Response<VoidResult> AbortRecipe(DataflowRecipeInfo dfRecipeInfo);

        [OperationContract]
        Response<UTOJobProgram> StartRecipeDF(DataflowRecipeInfo dfRecipeInfo, string processJobID, List<Guid> wafersGuid);

        [OperationContract]
        Response<List<DataflowRecipeInfo>> GetAllDataflowRecipes(List<ActorType> actors); // A la connexion

        [OperationContract]
        Response<VoidResult> StartJob_Material(DataflowRecipeInfo dfRecipeInfo, Material wafer);

        [OperationContract]
        Response<VoidResult> AbortJobID(string jobID);

        [OperationContract]
        Response<VoidResult> Initialize();

        [OperationContract]
        Response<UTOJobProgram> GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo);
    }
}
