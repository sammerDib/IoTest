using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.AGS.Data.Enum;
using UnitySC.PM.AGS.Service.Implementation.Proxy;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.AGS.Service.Test.Implementation.Proxy
{
    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public void Must_Map_ArgosRecipe_To_Dto_Then_To_ArgosRecipe()
        {
            var mapper = ClassLocator.Default.GetInstance<Mapper>();

            const uint givenFrequency = 30000;
            const double givenStartAngle = 1.0;
            const bool givenChuckBernouilliEnable = false;
            const int givenGain = -10;
            
            var rawRecipe = new ArgosRecipe
                {
                    Frequency_Hz = givenFrequency, 
                    StartAngle_deg = givenStartAngle, 
                    ChuckBernouilliEnable = givenChuckBernouilliEnable
                };
            rawRecipe.SensorRecipes.Add(SensorID.Top, SensorRecipe.Default());
            rawRecipe.SensorRecipes[SensorID.Top].Gain_db = givenGain;
            
            // Mapping to Dto format (Recipe is serialize into a xml string)
            var mappedRecipe = mapper.AutoMap.Map<DataAccess.Dto.Recipe>(rawRecipe);
            
            // Mapping from the Dto format to the PM recipe Format
            var backToRecipe = mapper.AutoMap.Map<ArgosRecipe>(mappedRecipe);
            
            // Then we check that we didn't lose any information
            Assert.AreEqual(givenFrequency, backToRecipe.Frequency_Hz);
            Assert.AreEqual(givenStartAngle, backToRecipe.StartAngle_deg);
            Assert.AreEqual(givenChuckBernouilliEnable, backToRecipe.ChuckBernouilliEnable);
            Assert.AreEqual(givenGain, backToRecipe.SensorRecipes[SensorID.Top].Gain_db);
        }
    }
}
