using System.ServiceModel;

using UnitySC.PM.EME.Service.Interface.Recipe.Execution;

namespace UnitySC.PM.EME.Service.Interface.Recipe
{
    [ServiceContract]
    public interface IEMERecipeServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ExecutionStatusUpdated(RecipeExecutionMessage message);
    }
}
