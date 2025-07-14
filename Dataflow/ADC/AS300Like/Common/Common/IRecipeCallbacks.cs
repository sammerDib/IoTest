using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public interface IRecipeCallbacks
    {
        void RecipeValueChange(String pFileName, String pSection, String pKey, String pNewKeyValue, String pLastKeyValue);
    }
}
