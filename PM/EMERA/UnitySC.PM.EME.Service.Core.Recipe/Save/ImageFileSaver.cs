using System.IO;

using UnitySC.Shared.Image;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class ImageFileSaver : IImageFileSaver
    {
        private readonly IRecipeAcquisitionTemplateComposer _composer;
        private readonly ILogger<ImageFileSaver> _logger;

        public ImageFileSaver(IRecipeAcquisitionTemplateComposer composer, ILogger<ImageFileSaver> logger)
            => (_composer, _logger) = (composer, logger);

        public string Save(RecipeAdapter recipe, AcquisitionSettings acquisition, ServiceImage image,
            (int, int)? positionInGrid = null)
        {
            string directory = string.IsNullOrEmpty(recipe.CustomSavePath)
                ? _composer.GetImageDirectory(recipe, acquisition)
                : recipe.CustomSavePath;
            Directory.CreateDirectory(directory);
            string fileName;
            if (recipe.RunOptions.RunStitchFullImages)
            {
                fileName = _composer.GetFullImageFileName(recipe, acquisition);
            }
            else
            {
                fileName = _composer.GetVignetteImageFileName(recipe, acquisition, positionInGrid.Value);
            }
            string path = Path.Combine(directory, fileName);
            _logger.Information($"Saving image to path : {path}");
            image.SaveToFile(path);
            return Path.Combine(directory, fileName);
        }

        public ImageFolderAndBaseName GetFolderAndBaseName(RecipeAdapter recipe, AcquisitionSettings acquisition)
        {
            string directory = _composer.GetImageDirectory(recipe, acquisition);
            return new ImageFolderAndBaseName(directory,
                _composer.GetImageBaseName(recipe, acquisition));
        }
    }
}
