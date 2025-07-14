using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.AGS.Data;
using UnitySC.PM.AGS.Data.Enum;
using UnitySC.PM.Shared.Data;

namespace UnitySC.PM.AGS.Service.Interface.RecipeService
{
    [DataContract(Namespace = "")]
    [Serializable]
    public class ArgosRecipe : PmRecipe
    {
        #region Default

        public static ArgosRecipe Default()
        {
            var recipe = new ArgosRecipe { Frequency_Hz = 25000, StartAngle_deg = 0.0 };

            recipe.SensorRecipes.Add(SensorID.Top, new SensorRecipe());

            return recipe;
        }

        #endregion

        [DataMember] public bool ChuckBernouilliEnable { get; set; } = false;

        [DataMember] public uint Frequency_Hz { get; set; } // Hertz
        
        [DataMember] public uint WaferRadius_mm { get; set; } // millimeter

        [DataMember] public double StartAngle_deg { get; set; } // degree

        [DataMember]
        public Dictionary<SensorID, SensorRecipe> SensorRecipes { get; set; } =
            new Dictionary<SensorID, SensorRecipe>();
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class SensorRecipe
    {
        [DataMember] public bool Enable { get; set; }

        [DataMember] public double ScanStartRadialPosition { get; set; }

        [DataMember] public double HeightPosition { get; set; }

        [DataMember]
        public uint RevolutionCount { get; set; } =
            1; // Number of Revolutions to do, for each Revolution, we move the sensor a bit more in the wafer

        [DataMember] public uint ExposureTime_us { get; set; }

        [DataMember] public double Gain_db { get; set; }

        #region Default

        public static SensorRecipe Default()
        {
            return new SensorRecipe
            {
                Enable = true,
                ScanStartRadialPosition = 5.0,
                HeightPosition = 5.0,
                ExposureTime_us = 13,
                Gain_db = 7.0
            };
        }

        #endregion
    }
}
