namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public interface IRecipeAcquisitionTemplateComposer
    {
        string GetVignetteImageFileName(RecipeAdapter recipe, AcquisitionSettings acquisition, (int, int) positionInGrid);
        string GetFullImageFileName(RecipeAdapter recipe, AcquisitionSettings acquisition);
        string GetImageBaseName(RecipeAdapter recipe, AcquisitionSettings acquisition);
        string GetImageDirectory(RecipeAdapter recipe, AcquisitionSettings acquisition);
        string GetAdaFileName(RecipeAdapter recipe);
    }
}
