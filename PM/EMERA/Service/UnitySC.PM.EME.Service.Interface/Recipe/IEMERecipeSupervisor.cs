using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.EME.Service.Interface.Recipe
{
    public interface IEMERecipeSupervisor : IEMERecipeService, IEMERecipeServiceCallback
    {
        EMERecipe GetRecipe(Guid recipeKey);
        
        void SaveRecipe(EMERecipe recipe, bool incrementVersion = true);
    }
}
