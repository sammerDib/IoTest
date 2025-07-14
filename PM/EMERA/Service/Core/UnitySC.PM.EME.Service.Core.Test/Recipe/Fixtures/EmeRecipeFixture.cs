using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe.Fixtures
{
    public static class EmeRecipeFixture
    {
        public static EMERecipe CreateRecipe(bool runStitchFullImage = false)
        {
            return new EMERecipe
            {
                ActorType = ActorType.EMERA,
                Acquisitions =
                    new List<Acquisition>
                    {
                        new Acquisition { Filter = EMEFilter.NoFilter, LightDeviceId = "3" },
                        new Acquisition { Filter = EMEFilter.BandPass450nm50, LightDeviceId = "4" }
                    },
                Execution =
                    new ExecutionSettings
                    {
                        Strategy = AcquisitionStrategy.Serpentine,
                        ReduceResolution = true,
                        ConvertTo8Bits = true,
                        NormalizePixelValue = true,
                        CorrectDistortion = true,
                        RunStitchFullImage = runStitchFullImage,
                        RunAutoFocus = true
                    },
                Step = new Step
                {
                    Product = new Product
                    {
                        WaferCategory = new WaferCategory
                        {
                            DimentionalCharacteristic = new WaferDimensionalCharacteristic
                            {
                                Diameter = 100.Millimeters()
                            }
                        }
                    }
                }
            };
        }

        public static EMERecipe CreateRecipeWithInvalidFilter()
        {
            var recipe = CreateRecipe();
            recipe.Acquisitions[0].Filter = EMEFilter.Unknown;
            return recipe;
        }

        public static EMERecipe CreateRecipeWithInvalidLight()
        {
            var recipe = CreateRecipe();
            recipe.Acquisitions[0].LightDeviceId = "Unknown";
            return recipe;
        }
    }
}
