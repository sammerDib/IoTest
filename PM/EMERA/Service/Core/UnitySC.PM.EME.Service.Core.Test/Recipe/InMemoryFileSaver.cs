using System.Collections.Concurrent;

using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Core.Recipe.Save;
using UnitySC.Shared.Image;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    public class InMemoryFileSaver : IImageFileSaver
    {
        public string Save(Core.Recipe.RecipeAdapter recipe, AcquisitionSettings acquisition,
            ServiceImage image, (int, int)? position = null)
        {
            SavedImages.Add(image);
            return "";
        }

        public ImageFolderAndBaseName GetFolderAndBaseName(Core.Recipe.RecipeAdapter recipe,
            AcquisitionSettings acquisition)
        {
            return new ImageFolderAndBaseName("", "");
        }

        // Thread-safe list (concurrent add)
        public ConcurrentBag<ServiceImage> SavedImages { get; } = new ConcurrentBag<ServiceImage>();
    }
}
