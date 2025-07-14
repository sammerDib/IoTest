using System;
using System.Collections.Generic;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.FDC
{
    [Serializable]
    public class FDCsConfiguration
    {
        public List<FDCItemConfig> FDCItemsConfig { get; set; }

        static public FDCsConfiguration Load(string fdcConfigFilePath)
        {
            try
            {
                return XML.Deserialize<FDCsConfiguration>(fdcConfigFilePath);
            }
            catch (Exception e)
            {
                var _logger = ClassLocator.Default.GetInstance<ILogger>();
                _logger.Error($"Unable to load the FDCs configuration file : {fdcConfigFilePath} : {e.Message}");
                return null;
            }
        }

        public void Save(string fdcConfigFilePath)
        {
   
            this.Serialize(fdcConfigFilePath);
        }
    }
}
