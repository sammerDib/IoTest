using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base.Acquisition;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeEMERecipeSupervisor : IEMERecipeSupervisor
    {
        public Response<EMERecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0)
        {
            return new Response<EMERecipe>
            {
                Result = new EMERecipe() { Name = "EMERA Recipe", ActorType = ActorType.EMERA, UserId = 1 },
            };       
        }

        public void ExecutionStatusUpdated(RecipeExecutionMessage message)
        {
            throw new NotImplementedException();
        }

        public EMERecipe GetRecipe(Guid recipeKey)
        {
            if(!recipeKey.Equals(new Guid("1fdd197c-ff50-4e22-a4ef-b713c4f0a5f8")))
            {
                return new EMERecipe() {Name ="EMERecipe123", ActorType = ActorType.EMERA, UserId = 1 };
            }
            return null;            
        }

        public Response<EMERecipe> GetRecipeFromKey(Guid recipeKey)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> PauseRecipe()
        {
            throw new NotImplementedException();
        }

        public void RecipeFinished(List<FullImageResult> results)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> ResumeRecipe()
        {
            throw new NotImplementedException();
        }

        public void SaveRecipe(EMERecipe recipe, bool incrementVersion = true)
        {
           
        }

        public Response<int> SaveRecipe(EMERecipe recipe, bool incrementVersion, int userId)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartRecipe(EMERecipe recipe, string customSavePath)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StopRecipe()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SubscribeToRecipeChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> UnsubscribeToRecipeChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartCycling(EMERecipe recipe, string customSavePath)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StopCycling()
        {
            throw new NotImplementedException();
        }
    }
}
