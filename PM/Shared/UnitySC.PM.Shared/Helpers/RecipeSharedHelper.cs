using System;
using System.IO;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Helpers
{
    public static class RecipeSharedHelper
    {
        public const int MaxRecipeNameLengthInDB = 50;

        public static string FindNewRecipeName(int stepId, string recipeFilePath, ActorType actorType, ServiceInvoker<IDbRecipeService> dbRecipeService)
        {
            string importedRecipeName = Path.GetFileNameWithoutExtension(recipeFilePath);
            var newRecipeName = importedRecipeName;
            if (newRecipeName.Length > MaxRecipeNameLengthInDB)
            {
                throw new Exception($"Recipe file name {newRecipeName} is too long (more than {MaxRecipeNameLengthInDB} characters)");
            }            
            var recipe = dbRecipeService.Invoke(x => x.GetRecipe(actorType, stepId, newRecipeName, false, false));

            int i = 1;
            while (recipe != null)
            {
                var maxRecipeNameLenght = MaxRecipeNameLengthInDB - (int)Math.Floor(Math.Log10(i) + 1) - 1;
                if (newRecipeName.Length > maxRecipeNameLenght)
                {
                    importedRecipeName = importedRecipeName.Substring(0, maxRecipeNameLenght);
                }

                newRecipeName = importedRecipeName + $"_{i}";
                recipe = dbRecipeService.Invoke(x => x.GetRecipe(actorType, stepId, newRecipeName, false, false));
                i++;
            }
            return newRecipeName;
        }
    }
}
