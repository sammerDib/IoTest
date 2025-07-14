using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    [Serializable]
    [XmlInclude(typeof(AcquisitionConfiguration))]
    public class RecipeConfiguration
    {
        public static RecipeConfiguration Init(IEMEServiceConfigurationManager configManager)
        {
            RecipeConfiguration recipeConfiguration;
            if (File.Exists(configManager.FlowsConfigurationFilePath))
            {
                recipeConfiguration = XML.Deserialize<RecipeConfiguration>(configManager.RecipeConfigurationFilePath);
            }
            else
            {
                recipeConfiguration = new RecipeConfiguration();
                recipeConfiguration.AcquisitionConfiguration200mm = new AcquisitionConfiguration();
                recipeConfiguration.AcquisitionConfiguration150mm = new AcquisitionConfiguration();
                recipeConfiguration.AcquisitionConfiguration100mm = new AcquisitionConfiguration();
            }

            return recipeConfiguration;
        }

        [XmlAttribute]
        public string Version { get; set; } = "1.0.0";

        [DataMember]
        public AcquisitionConfiguration AcquisitionConfiguration200mm {  get; set; }

        [DataMember]
        public AcquisitionConfiguration AcquisitionConfiguration150mm { get; set; }

        [DataMember]
        public AcquisitionConfiguration AcquisitionConfiguration100mm { get; set; }

        public AcquisitionConfiguration GetAcquisitionConfiguration(double diameterMillimeters)
        {
            switch (diameterMillimeters)
            {
                case 200.0:
                    return AcquisitionConfiguration200mm;
                case 150.0:
                    return AcquisitionConfiguration150mm;
                case 100.0:
                    return AcquisitionConfiguration100mm;
                default:
                    throw new NotImplementedException(
                        $"Recipe acquisition for a {diameterMillimeters}mm wafer not implemented ");
            }
        }
    }

    public class AcquisitionConfiguration
    {
        public int NbImagesX { get; set; }

        public int NbImagesY { get; set; }

        public double MarginPercentage { get; set; } = 0.005;

        public double DDFLightPower { get; set; } = 6.0;
        public double UVLightPower { get; set; } = 10.0;

        public Length DefaultZFocus { get; set; } = 6.Millimeters();
    }
}
