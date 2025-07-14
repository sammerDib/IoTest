using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

using UnitySC.Shared.Tools;

namespace UnitySC.ADCAS300Like.Common
{
    public enum EventID { AllPostProcess_Complete, AllPostProcess_Failed , ResultsError } 

    /// <summary>
    ///  Définition du fichier XML de configuration 
    /// </summary>
    [Serializable]
    public class ADCsConfigs
    {
        public String ADCResultsLogPrefix;
        public List<ADCItemConfig> ADCItemsConfig;
        public int PostProcessCompleteDVID;
        public bool ADCPostProcessCompleteEnable;
        public int WaitingPostProcessResultsTimeout_sec;

        public static ADCsConfigs Init(string path)
        {
            ADCsConfigs adcConfig;
            if (File.Exists(path))
                adcConfig = XML.Deserialize<ADCsConfigs>(path);
            else
            {
                adcConfig = new ADCsConfigs();
            }
            return adcConfig;

        }
    }
}
