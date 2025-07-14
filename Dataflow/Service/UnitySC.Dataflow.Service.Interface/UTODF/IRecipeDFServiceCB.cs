using System;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Dataflow.Service.Interface
{
    // DF Serveur -> UTO Client
    [ServiceContract]
    public interface IRecipeDFServiceCB
    {
        // DataFlow
        [OperationContract(IsOneWay = true)]
        void DFRecipeProcessStarted(DataflowRecipeInfo dfRecipeInfo);

        [OperationContract(IsOneWay = true)]
        void DFRecipeProcessComplete(DataflowRecipeInfo dfRecipeInfo, Material wafer, String status);

        [OperationContract(IsOneWay = true)]
        void DFRecipeAdded(DataflowRecipeInfo dfRecipeInfo);

        [OperationContract(IsOneWay = true)]
        void DFRecipeDeleted(DataflowRecipeInfo dfRecipeInfo);

        // PM
        [OperationContract(IsOneWay = true)]
        void PMRecipeProcessStarted(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer);

        [OperationContract(IsOneWay = true)]
        void PMRecipeProcessComplete(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer, String status);
    }
}
