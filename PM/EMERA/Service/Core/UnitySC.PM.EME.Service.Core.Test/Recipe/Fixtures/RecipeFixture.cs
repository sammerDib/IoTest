using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Core.Test.Calibration;
using UnitySC.PM.EME.Service.Core.Test.Camera;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe.Fixtures
{
    public static class RecipeFixture
    {
        public static Core.Recipe.RecipeAdapter CreateRecipe(RemoteProductionInfo remoteProductionInfo = null, bool runStitchFullImage = false)
        {
            var configuration = ClassLocator.Default.GetInstance<RecipeConfiguration>();
            var emeRecipe = EmeRecipeFixture.CreateRecipe(runStitchFullImage);
            EmeHardwareManager hardware = new FakeHardwareManager(emeRecipe);
            var recipe = new Core.Recipe.RecipeAdapter(emeRecipe, configuration, hardware, new FakeEmeraCamera(),
                new FakeCalibrationManager(), remoteProductionInfo);
            if (remoteProductionInfo != null)
            {
                recipe.RemoteProductionInfo = remoteProductionInfo;
            }

            return recipe;
        }
    }
}
