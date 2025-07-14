using UnitySC.Shared.Image;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public interface IImageFileSaver
    {
        string Save(RecipeAdapter recipe, AcquisitionSettings acquisition, ServiceImage image, (int, int)? xYNewPos = null);

        ImageFolderAndBaseName GetFolderAndBaseName(RecipeAdapter recipe, AcquisitionSettings acquisition);
    }
}
