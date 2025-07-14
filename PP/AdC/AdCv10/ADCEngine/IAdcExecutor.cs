using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

using AcquisitionAdcExchange;

namespace ADCEngine
{
    [DataContract]
    public class ModuleStat
    {
        [DataMember] public int ModuleId;
        [DataMember] public eModuleState State;
        [DataMember] public int nbObjectsIn;
        [DataMember] public int nbObjectsOut;
        [DataMember] public bool HasError;
        [DataMember] public string ErrorMessage;
    }

    [DataContract]
    public class RecipeStat
    {
        [DataMember] public Dictionary<int, ModuleStat> ModuleStat = new Dictionary<int, ModuleStat>();
        [DataMember] public bool IsRunning;
        [DataMember] public bool HasError;
        [DataMember] public int FaultyModuleId = -1;
    }

    [ServiceContract]
    public interface IAdcExecutor
    {
        [OperationContract] RecipeId ReprocessRecipe(byte[] RecipeData);
        [OperationContract] void AbortRecipe(RecipeId recipeId);
        //[OperationContract]
        RecipeId ExecuteRecipe(Recipe recipe);
        [OperationContract] RecipeId GetCurrentRecipeId();
        [OperationContract] WaferInfo GetCurrentWaferInfo();
        [OperationContract] byte[] GetRecipe(RecipeId recipeId);
        [OperationContract] RecipeStat GetRecipeStat(RecipeId recipeId);
        [OperationContract] void SetLocalRecipePath(string localRecipePath);
    }
}
